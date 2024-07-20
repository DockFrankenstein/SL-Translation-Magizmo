using SFB;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Translation.ImportAndExport
{
    public class SlImportAndExport : ImportAndExportBase, IImporter, IExporter
    {
        [SerializeField] ErrorWindow error;
        [SerializeField] NotificationManager notifications;

        [Label("Exporting")]
        [SerializeField] UIDocument exportDocument;

        public Action OnImport;
        public Action OnExport;

        public string Name => "SCPSL";

        TextField _exportBlank;

        Button _exportButton;
        Button _exportCloseButton;

        private void Awake()
        {
            var root = exportDocument.rootVisualElement;
            root.ChangeDispaly(false);

            _exportBlank = root.Q<TextField>("blank-entry");
            _exportButton = root.Q<Button>("export-button");
            _exportCloseButton = root.Q<Button>("close");

            _exportCloseButton.clicked += () =>
            {
                root.ChangeDispaly(false);
            };

            _exportButton.clicked += () =>
            {
                var paths = StandaloneFileBrowser.OpenFolderPanel("", Directory.Exists(ExportPath) ?
                    ExportPath :
                    Settings.GeneralSettings.TranslationPath, false);

                if (paths.Length == 0)
                    return;

                ExportPath = paths[0];

                try
                {
                    Export();
                    exportDocument.rootVisualElement.ChangeDispaly(false);
                }
                catch(Exception e)
                {
                    error.CreateExportExceptionPrompt(e);
                }
            };
        }

        public void BeginImport()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", Settings.GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            ImportPath = paths[0];

            try
            {
                Import();
            }
            catch (Exception e)
            {
                error.CreateImportExceptionPrompt(e);
            }
        }

        public void BeginExport()
        {
            exportDocument.rootVisualElement.ChangeDispaly(true);
        }

        public void Import()
        {
            manager.CurrentVersion.Import(manager.File, ImportPath);
            FinalizeImport();
            OnImport?.Invoke();
        }

        public void Export()
        {
            manager.CurrentVersion.Export(manager.File, ExportPath, _exportBlank.value);
            notifications.NotifyExport(ExportPath);
            OnExport?.Invoke();
        }
    }
}