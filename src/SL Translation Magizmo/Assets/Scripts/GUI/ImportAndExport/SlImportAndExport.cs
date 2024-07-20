using Project.Translation;
using Project.Translation.ImportAndExport;
using SFB;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Project.Settings;
using Project.Translation.Data;
using System.Linq;
using Project.Undo;
using System.Collections.Generic;

namespace Project.GUI.ImportAndExport
{
    public class SlImportAndExport : ImportAndExportBase, IImporter, IExporter
    {
        [Label("Exporting")]
        [SerializeField] UIDocument exportDocument;
        [SerializeField] UIDocument importDocument;

        public Action OnImport;
        public Action OnExport;

        public string Name => "SCPSL";

        TextField _exportBlank;
        Button _exportButton;
        Button _exportCloseButton;

        Toggle _importEmpty;
        Toggle _overwriteOld;
        Button _importButton;
        Button _importCloseButton;

        SaveFile _importedFile;

        private void Awake()
        {
            var exportRoot = exportDocument.rootVisualElement;
            exportRoot.ChangeDispaly(false);

            var importRoot = importDocument.rootVisualElement;
            importRoot.ChangeDispaly(false);

            _exportBlank = exportRoot.Q<TextField>("blank-entry");
            _exportButton = exportRoot.Q<Button>("export-button");
            _exportCloseButton = exportRoot.Q<Button>("close");

            _importEmpty = importRoot.Q<Toggle>("import-empty");
            _overwriteOld = importRoot.Q<Toggle>("overwrite-old");
            _importButton = importRoot.Q<Button>("import-button");
            _importCloseButton = importRoot.Q<Button>("close");

            _exportCloseButton.clicked += () =>
            {
                exportRoot.ChangeDispaly(false);
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

            _importCloseButton.clicked += () =>
            {
                importRoot.ChangeDispaly(false);
            };

            _importButton.clicked += () =>
            {
                var entries = _importedFile.Entries
                    .AsEnumerable();

                if (!_importEmpty.value)
                    entries = entries.Where(x => !string.IsNullOrWhiteSpace(x.Value.content));

                if (!_overwriteOld.value)
                    entries = entries.Where(x => !TranslationManager.File.Entries.TryGetValue(x.Key, out var content) || string.IsNullOrWhiteSpace(content.content));

                var oldValues = TranslationManager.File.Entries
                    .ToDictionary(x => x.Key, x => x.Value.content);

                foreach (var item in entries)
                {
                    if (!TranslationManager.File.Entries.ContainsKey(item.Key))
                        TranslationManager.File.Entries.Add(item.Key, new SaveFile.EntryData(item.Key));

                    TranslationManager.File.Entries[item.Key].content = item.Value.content;
                }

                var newValues = TranslationManager.File.Entries
                    .ToDictionary(x => x.Key, x => x.Value.content);

                Undo.AddStep(new UndoStep<Dictionary<string, string>>(oldValues, newValues, a =>
                {
                    foreach (var item in a)
                    {
                        if (!TranslationManager.File.Entries.ContainsKey(item.Key))
                            TranslationManager.File.Entries.Add(item.Key, new SaveFile.EntryData(item.Key));

                        TranslationManager.File.Entries[item.Key].content = item.Value;
                    }

                    var unusedKeys = TranslationManager.File.Entries
                        .Select(x => x.Key)
                        .Except(a.Select(x => x.Key));

                    foreach (var item in unusedKeys)
                        TranslationManager.File.Entries.Remove(item);
                }));

                importRoot.ChangeDispaly(false);
                FinalizeImport();
                OnImport?.Invoke();
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
            _importedFile = new SaveFile(TranslationManager.CurrentVersion);
            TranslationManager.CurrentVersion.Import(_importedFile, ImportPath);
            importDocument.rootVisualElement.ChangeDispaly(true);
        }

        public void Export()
        {
            TranslationManager.CurrentVersion.Export(TranslationManager.File, ExportPath, _exportBlank.value);
            FinalizeExport();
            OnExport?.Invoke();
        }
    }
}