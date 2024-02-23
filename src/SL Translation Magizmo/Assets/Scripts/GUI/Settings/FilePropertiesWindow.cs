using Project.Translation;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Settings
{
    public class FilePropertiesWindow : MonoBehaviour
    {
        [SerializeField] UIDocument document;
        [SerializeField] TranslationManager manager;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        Button _closeButton;
        DropdownField _slVersion;

        private void Awake()
        {
            var root = document.rootVisualElement;
            root.ChangeDispaly(false);

            _closeButton = root.Q<Button>("close");
            _slVersion = root.Q<DropdownField>("sl-version");

            _closeButton.clicked += () =>
            {
                root.ChangeDispaly(false);
            };

            _slVersion.choices = manager.versions
                .Select(x => x.version.ToString())
                .Prepend("Newest")
                .ToList();

            _slVersion.RegisterValueChangedCallback(args =>
            {
                if (args.target != _slVersion) return;

                if (_slVersion.index == 0)
                {
                    manager.File.UseNewestSlVersion = true;
                    manager.LoadCurrentVersionFromFile();
                    manager.File.CleanupToVersion(manager.CurrentVersion);
                    manager.MarkFileDirty();
                    return;
                }

                var ver = manager.versions[_slVersion.index - 1];

                manager.File.UseNewestSlVersion = false;
                manager.File.SlVersion = ver.version;
                manager.LoadCurrentVersionFromFile();
                manager.File.CleanupToVersion(ver);
                manager.MarkFileDirty();
            });

            manager.OnFileChanged += OnFileChanged;
        }

        void OnFileChanged()
        {
            _slVersion.index = manager.File.UseNewestSlVersion ?
                0 :
                Array.IndexOf(manager.versions, manager.CurrentVersion) + 1;
        }

        public void Open()
        {
            document.rootVisualElement.ChangeDispaly(true);
            OnFileChanged();
        }
    }
}