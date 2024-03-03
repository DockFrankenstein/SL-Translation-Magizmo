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
                .Reverse()
                .Prepend("Newest")
                .ToList();

            _slVersion.RegisterValueChangedCallback(args =>
            {
                if (root.style.display == DisplayStyle.None) return;

                if (args.target != _slVersion) return;

                if (_slVersion.index == 0)
                {
                    manager.File.UseNewestSlVersion = true;
                    manager.LoadCurrentVersionFromFile();
                    manager.File.CleanupToVersion(manager.CurrentVersion);
                    manager.MarkFileDirty(this);
                    return;
                }

                var ver = manager.versions[manager.versions.Length - _slVersion.index];

                manager.File.UseNewestSlVersion = false;
                manager.File.SlVersion = ver.version;
                manager.LoadCurrentVersionFromFile();
                manager.File.CleanupToVersion(ver);
                manager.MarkFileDirty(this);
            });

            manager.OnFileChanged += OnFileChanged;
        }

        void OnFileChanged(object fromContext)
        {
            if (fromContext as UnityEngine.Object == this)
                return;

            Load();
        }

        void Load()
        {
            _slVersion.index = manager.File.UseNewestSlVersion ?
                0 :
                manager.versions.Length - Array.IndexOf(manager.versions, manager.CurrentVersion);
        }

        public void Open()
        {
            document.rootVisualElement.ChangeDispaly(true);
            Load();
        }
    }
}