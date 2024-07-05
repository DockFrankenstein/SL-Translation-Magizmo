using Project;
using Project.Translation;
using qASIC;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Project.Translation.Comparison;
using System.IO;

namespace Assets.Scripts.GUI
{
    public class ComparisonManagerUI : MonoBehaviour
    {
        [SerializeField] UIDocument document;
        [SerializeField] TranslationManager manager;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
            manager = FindObjectOfType<TranslationManager>();
        }

        Button closeButton;
        Button loadButton;
        Button useButton;

        VisualElement listContent;

        VisualElement standardContent;
        Foldout slFoldout;
        VisualElement slContent;
        Foldout loadedFoldout;
        VisualElement loadedContent;

        Button _selected;
        string _selectedPath;

        private void Awake()
        {
            var root = document.rootVisualElement;
            root.ChangeDispaly(false);

            closeButton = root.Q<Button>("close");
            loadButton = root.Q<Button>("load");
            useButton = root.Q<Button>("use");
            listContent = root.Q("list");

            closeButton.clicked += () =>
            {
                root.ChangeDispaly(false);
            };

            useButton.clicked += () =>
            {
                manager.ComparisonManager.ChangeCurrent(_selectedPath);
                root.ChangeDispaly(false);
            };

            standardContent = new VisualElement();
            slFoldout = new Foldout() { text = "From SL Translation Folder" };
            slContent = new VisualElement();
            loadedFoldout = new Foldout() { text = "Loaded" };
            loadedContent = new VisualElement();

            var noneButton = new Button()
            {
                text = "None",
            };

            standardContent.Add(noneButton);

            listContent.Add(standardContent);
            listContent.Add(slFoldout);
            listContent.Add(slContent);
            listContent.Add(loadedFoldout);
            listContent.Add(loadedContent);

            _selected = noneButton;
            InitializeTreeItem(noneButton, "");

            InitializeFoldout(slFoldout, slContent);
            InitializeFoldout(loadedFoldout, loadedContent);
        }

        void InitializeFoldout(Foldout foldout, VisualElement content)
        {
            foldout.RegisterValueChangedCallback(args =>
            {
                if (args.target == foldout)
                    content.ChangeDispaly(args.newValue);
            });

            content.ChangeDispaly(foldout.value);
        }

        void CreateItemForPath(VisualElement container, string path) =>
            CreateItemForPath(container, path, path);

        void CreateItemForPath(VisualElement container, string displayName, string path)
        {
            var button = new Button()
            {
                text = displayName,
            };

            container.Add(button);
            InitializeTreeItem(button, path);
        }

        void InitializeTreeItem(Button button, string path)
        {
            button.clicked += () =>
            {
                if (button == _selected) return;
                _selected?.RemoveFromClassList("hierarchy-selected");
                _selected = button;
                _selected.AddToClassList("hierarchy-selected");
                _selectedPath = path;
            };

            if (button == _selected)
                _selected.AddToClassList("hierarchy-selected");
        }

        public void Open()
        {
            Refresh();
            document.rootVisualElement.ChangeDispaly(true);
        }



        void Refresh()
        {
            slContent.Clear();
            loadedContent.Clear();

            var slItems = manager.ComparisonManager.AvaliableTranslations
                .Where(x => x.Replace('\\', '/').StartsWith($"{ComparisonTranslationManager.TRANSLATION_FOLDER_PREFIX}/"));
            var loadedItems = manager.ComparisonManager.AvaliableTranslations
                .Except(slItems);

            slFoldout.ChangeDispaly(slItems.Count() > 0);
            loadedFoldout.ChangeDispaly(loadedItems.Count() > 0);

            foreach (var path in slItems)
            {
                var displayName = path.Substring(ComparisonTranslationManager.TRANSLATION_FOLDER_PREFIX.Length + 1, path.Length - ComparisonTranslationManager.TRANSLATION_FOLDER_PREFIX.Length - 1);
                CreateItemForPath(slContent, displayName, path);
            }

            foreach (var path in loadedItems)
                CreateItemForPath(loadedContent, Path.GetFileName(path), path);
        }
    }
}
