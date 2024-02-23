using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Project.GUI.Preview
{
    public class SceneDropdown : MonoBehaviour
    {
        [SerializeField] PreviewSceneManager sceneManager;
        [SerializeField] SceneDropdownItem backButton;
        [SerializeField] Transform itemParent;
        [SerializeField] SceneDropdownItem template;
        [SerializeField] GameObject foldout;

        List<SceneDropdownItem> _items = new List<SceneDropdownItem>();
        Queue<SceneDropdownItem> _itemPool = new Queue<SceneDropdownItem>();

        public string currentPath = string.Empty;

        private void Awake()
        {
            backButton.button.onClick.AddListener(() =>
            {
                var parts = currentPath.Split('/');

                if (parts.Length > 0)
                    parts = parts
                        .SkipLast(1)
                        .ToArray();

                currentPath = string.Join('/', parts);
                Reload();
            });

            template.gameObject.SetActive(false);
            Reload();
        }

        void Reload()
        {
            var itemsToRemove = new List<SceneDropdownItem>(_items);
            foreach (var item in itemsToRemove)
                RemoveItem(item);

            backButton.gameObject.SetActive(!string.IsNullOrEmpty(currentPath));

            var directories = sceneManager.scenes
                .Select(x => x.path)
                .Where(x => x.StartsWith(currentPath))
                .Select(x => x.Substring(currentPath.Length, x.Length - currentPath.Length))
                .Select(x => currentPath.Length == 0 ? x : x.Substring(1, x.Length - 1))
                .Select(x => x.Split('/').First())
                .Distinct()
                .ToList();

            foreach (var path in directories)
            {
                var item = GetItem();
                item.path = path;
                item.text.text = path;
            }
        }

        void ButtonOnClick(SceneDropdownItem item)
        {
            var newPath = string.IsNullOrEmpty(currentPath) ?
                item.path :
                $"{currentPath}/{item.path}";

            var scenes = sceneManager.scenes
                .Where(x => x.path.StartsWith(newPath));

            var isDirection = true;

            if (scenes.Count() == 1)
            {
                var singleScene = scenes.First();
                var nextPath = singleScene.path.Substring(newPath.Length, singleScene.path.Length - newPath.Length);
                isDirection &= nextPath.Contains('/');
            }

            //Button leads to another directory
            if (isDirection)
            {
                currentPath = newPath;
                Reload();
                return;
            }

            //Button leads to the scene
            var scene = scenes.First();
            sceneManager.SelectScene(scene);
            ToggleFoldout(false);
        }

        public void ToggleFoldout(bool show)
        {
            foldout.SetActive(show);
        }

        public void ToggleFoldout() =>
            ToggleFoldout(!foldout.activeSelf);

        SceneDropdownItem GetItem()
        {
            if (_itemPool.Count == 0)
            {
                var newItem = Instantiate(template, itemParent);
                _itemPool.Enqueue(newItem);
                return GetItem();
            }

            var item = _itemPool.Dequeue();
            item.gameObject.SetActive(true);
            item.button.onClick.AddListener(() => ButtonOnClick(item));
            item.transform.SetAsLastSibling();

            _items.Add(item);
            return item;
        }

        void RemoveItem(SceneDropdownItem item)
        {
            item.gameObject.SetActive(false);
            item.button.onClick.RemoveAllListeners();
            _itemPool.Enqueue(item);
            _items.Remove(item);
        }
    }
}