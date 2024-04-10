using Fab.UITKDropdown;
using JetBrains.Annotations;
using Project.UI;
using qASIC.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Project.GUI.Preview
{
    public class SceneDropdownToolkit : MonoBehaviour
    {
        [SerializeField] UIDocument document;
        [FormerlySerializedAs("sceneManager")]
        [SerializeField] PreviewManager preview;

        PopupButton _popupButton;
        Button _backButton;
        ScrollView _scroll;

        public string Path { get; private set; } = string.Empty;
        Dictionary<Button, ItemData> ButtonData { get; set; } = new Dictionary<Button, ItemData>();

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = document.rootVisualElement;

            _popupButton = root.Q<PopupButton>("scene-dropdown");
            _backButton = root.Q<Button>("scene-dropdown-back");
            _scroll = root.Q<ScrollView>("scene-dropdown-items");

            _popupButton.ChangeOpenedState(false);

            _backButton.clicked += () =>
            {
                var parts = Path.Split('/');

                if (parts.Length > 0)
                    parts = parts
                        .SkipLast(1)
                        .ToArray();

                Path = string.Join('/', parts);
                Reload();
            };

            Reload();
        }

        void Reload()
        {
            _popupButton.text = PUtility.GenerateDisplayName(preview.CurrentScene.path.Split('/').Last());

            while (_items.Count > 0)
                RemoveItem(_items[0]);

            _backButton.visible = !string.IsNullOrEmpty(Path);

            var directories = preview.VersionScenes
                .Select(x => x.path)
                .Where(x => x.StartsWith(Path))
                .Select(x => x.Substring(Path.Length, x.Length - Path.Length))
                .Select(x => Path.Length == 0 ? x : (x.Length == 0 ? x : x.Substring(1, x.Length - 1)))
                .Select(x =>
                {
                    var splits = x.Split('/');
                    return new KeyValuePair<string, int>(splits.First(), splits.Length);
                })
                .GroupBy(x => x.Key)
                .Select(x => x.First())
                .ToList();

            foreach (var path in directories)
            {
                var item = GetItem();
                item.text = PUtility.GenerateDisplayName(path.Key);
                ButtonData[item].path = path.Key;

                if (path.Value > 1)
                    item.Add(new VisualElement().WithClass("dir-mark"));
            }
        }

        void ChangePath(string newPath)
        {
            var scenes = preview.VersionScenes
                .Where(x => x.path.StartsWith($"{newPath}/"));

            var isDirection = scenes.Count() != 0;

            //Button leads to another directory
            if (isDirection)
            {
                Path = newPath;
                Reload();
                return;
            }

            var scene = preview.VersionScenes.Where(x => x.path == newPath).FirstOrDefault();
            if (scene != null)
                preview.SelectScene(scene);

            _popupButton.ChangeOpenedState(false);
        }

        #region Item Factory
        List<Button> _items = new List<Button>();

        public void RemoveItem(Button item)
        {
            item.ChangeDispaly(false);
            _items.Remove(item);
            _scroll.Remove(item);

            if (ButtonData.ContainsKey(item))
                ButtonData.Remove(item);
        }

        public Button GetItem()
        {
            var item = CreateItem();

            _items.Add(item);
            _scroll.Add(item);

            if (!ButtonData.ContainsKey(item))
                ButtonData.Add(item, new ItemData());

            return item;
        }

        public Button CreateItem()
        {
            var item = new Button();
            item.clicked += () =>
            {
                if (!ButtonData.TryGetValue(item, out var data)) return;

                var newPath = string.IsNullOrEmpty(Path) ?
                    data.path :
                    $"{Path}/{data.path}";

                ChangePath(newPath);
                Reload();
            };

            return item;
        }
        #endregion

        class ItemData
        {
            public string path;
        }
    }
}