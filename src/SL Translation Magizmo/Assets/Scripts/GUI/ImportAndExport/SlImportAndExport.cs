using Project.Translation;
using Project.Translation.ImportAndExport;
using SFB;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Project.Settings;

namespace Project.GUI.ImportAndExport
{
    public class SlImportAndExport : ImportAndExportBase, IImporter, IExporter
    {
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
                    GeneralSettings.TranslationPath, false);

                if (paths.Length == 0)
                    return;

                ExportPath = paths[0];

                try
                {
                    Export();
                    exportDocument.rootVisualElement.ChangeDispaly(false);
                }
                catch (Exception e)
                {
                    Error.CreateExportExceptionPrompt(e);
                }
            };
        }

        public void BeginImport()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            ImportPath = paths[0];

            try
            {
                Import();
            }
            catch (Exception e)
            {
                Error.CreateImportExceptionPrompt(e);
            }
        }

        public void BeginExport()
        {
            exportDocument.rootVisualElement.ChangeDispaly(true);
        }

        public void Import()
        {
            TranslationManager.CurrentVersion.Import(TranslationManager.File, ImportPath);
            FinalizeImport();
            OnImport?.Invoke();
        }

        public void Export()
        {
            TranslationManager.CurrentVersion.Export(TranslationManager.File, ExportPath, _exportBlank.value);
            FinalizeExport();
            OnExport?.Invoke();
        }
    }
}