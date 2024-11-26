using Project.Translation;
using Project.Undo;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Settings
{
    public class FilePropertiesWindow : MonoBehaviour
    {
        [SerializeField] UIDocument document;
        [SerializeField] TranslationManager manager;
        [SerializeField] UndoManager undo;

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
                .Select(x => x.DisplayName)
                .Reverse()
                .Prepend("Newest")
                .ToList();

            _slVersion.RegisterValueChangedCallback(args =>
            {
                if (root.style.display == DisplayStyle.None) return;

                if (args.target != _slVersion) return;

                if (args.newValue == args.previousValue) return;

                var prevVersion = manager.CurrentVersion;
                var prevUseNewest = manager.File.UseNewestSlVersion;

                if (_slVersion.index == 0)
                {
                    if (manager.File.UseNewestSlVersion)
                        return;

                    manager.File.UseNewestSlVersion = true;
                    manager.LoadCurrentVersionFromFile();
                    manager.File.SlVersion = manager.CurrentVersion.version;

                    Finalize();
                    return;
                }

                var ver = manager.versions[manager.versions.Length - _slVersion.index];

                manager.File.UseNewestSlVersion = false;
                manager.File.SlVersion = ver.version;

                Finalize();


                void Finalize()
                {
                    manager.LoadCurrentVersionFromFile();
                    manager.File.SlVersion = manager.CurrentVersion.version;
                    manager.File.CleanupToVersion(manager.CurrentVersion);
                    manager.File.UpgradeToVersion(prevVersion, manager.CurrentVersion);

                    undo.AddStep(new UndoStep<KeyValuePair<Version, bool>>(
                        new KeyValuePair<Version, bool>(prevVersion.version, prevUseNewest),
                        new KeyValuePair<Version, bool>(manager.CurrentVersion.version, false),
                        a =>
                        {
                            manager.File.SlVersion = a.Key;
                            manager.File.UseNewestSlVersion = a.Value;
                        }),
                        this);
                }
            });

            undo.OnChanged += OnFileChanged;
        }

        void OnFileChanged(object fromContext)
        {
            if (fromContext as UnityEngine.Object == this)
                return;

            Load();
        }

        void Load()
        {
            var index = manager.File.UseNewestSlVersion ?
                0 :
                manager.versions.Length - Array.IndexOf(manager.versions, manager.CurrentVersion);

            _slVersion.SetValueWithoutNotify(_slVersion.choices[index]);
        }

        public void Open()
        {
            document.rootVisualElement.ChangeDispaly(true);
            Load();
        }
    }
}