using Project;
using Project.Translation;
using qASIC;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Project.Translation.Comparison;
using System.IO;
using Project.Settings;
using Project.Translation.Data;
using SFB;

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

            loadButton.clicked += () =>
            {
                var extensions = new ExtensionFilter[]
                {
                    new ExtensionFilter("", SaveFile.FILE_EXTENSION),
                    new ExtensionFilter("", "csv"),
                };

                var results = StandaloneFileBrowser.OpenFilePanel("Select File To Load", GeneralSettings.TranslationPath, extensions, false);

                if (results.Length == 0)
                    return;

                manager.ComparisonManager.AddPath(results[0]);
                Refresh();
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

        Button CreateItemForPath(string path) =>
            CreateItemForPath(path, path);

        void Select(Button button, string path)
        {
            if (button == _selected) return;
            _selected?.RemoveFromClassList("hierarchy-selected");
            _selected = button;
            _selected.AddToClassList("hierarchy-selected");
            _selectedPath = path;
        }

        Button CreateItemForPath(string displayName, string path)
        {
            var button = new Button()
            {
                text = displayName,
            };

            InitializeTreeItem(button, path);
            return button;
        }

        void InitializeTreeItem(Button button, string path)
        {
            button.clicked += () =>
            {
                Select(button, path);
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
                .Where(x => x.Replace('\\', '/').StartsWith($"{ComparisonManager.TRANSLATION_FOLDER_PREFIX}/"))
                .OrderBy(x => x);
            var loadedItems = manager.ComparisonManager.AvaliableTranslations
                .Except(slItems)
                .OrderBy(x => x);

            slFoldout.ChangeDispaly(slItems.Count() > 0);
            loadedFoldout.ChangeDispaly(loadedItems.Count() > 0);

            foreach (var path in slItems)
            {
                var displayName = path.Substring(ComparisonManager.TRANSLATION_FOLDER_PREFIX.Length + 1, path.Length - ComparisonManager.TRANSLATION_FOLDER_PREFIX.Length - 1);
                slContent.Add(CreateItemForPath(displayName, path));
            }

            foreach (var path in loadedItems)
                loadedContent.Add(CreateItemForPath(Path.GetFileName(path), path));
        }
    }
}
