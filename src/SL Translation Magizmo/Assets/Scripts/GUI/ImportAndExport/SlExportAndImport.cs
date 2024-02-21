using SFB;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Translation.ImportAndExport
{
    public class SlExportAndImport : ImportAndExportBase, IImporter, IExporter
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] ErrorWindow error;

        [Label("Exporting")]
        [SerializeField] UIDocument exportDocument;

        public Action OnImport;
        public Action OnExport;

        public string Name => "SCPSL";

        TextField _exportPath;
        Button _exportPathOpen;
        TextField _exportBlank;

        Button _exportButton;
        Button _exportCloseButton;

        private void Awake()
        {
            var root = exportDocument.rootVisualElement;
            root.ChangeDispaly(false);

            _exportPath = root.Q<TextField>("path");
            _exportPathOpen = root.Q<Button>("path-open");
            _exportBlank = root.Q<TextField>("blank-entry");
            _exportButton = root.Q<Button>("export-button");
            _exportCloseButton = root.Q<Button>("close");

            _exportPathOpen.clicked += () =>
            {
                bool exists = Directory.Exists(_exportPath.value);

                var paths = StandaloneFileBrowser.OpenFolderPanel("", exists ? 
                    _exportPath.value : 
                    Settings.GeneralSettings.TranslationPath, false);

                if (paths.Length == 0)
                    return;

                _exportPath.value = paths[0];
            };

            _exportCloseButton.clicked += () =>
            {
                root.ChangeDispaly(false);
            };

            _exportButton.clicked += () =>
            {
                var path = _exportPath.value;

                if (string.IsNullOrWhiteSpace(path))
                {
                    error.CreatePrompt("Invalid Path", "Please set an export path.");
                    return;
                }

                try
                {
                    manager.CurrentVersion.Export(manager.File, path, _exportBlank.value);
                }
                catch (Exception e)
                {
                    error.CreatePrompt("Export Error", $"There was a problem while exporting to SCPSL.\n{e}");
                    return;
                }

                root.ChangeDispaly(false);
                OnExport?.Invoke();
            };
        }

        public void BeginExport()
        {
            exportDocument.rootVisualElement.ChangeDispaly(true);
        }

        public void BeginImport()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", Settings.GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            manager.CurrentVersion.Import(manager.File, paths[0]);
            OnImport?.Invoke();
        }
    }
}