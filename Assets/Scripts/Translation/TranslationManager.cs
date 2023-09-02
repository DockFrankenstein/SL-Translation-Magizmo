using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Defines;
using System;
using qASIC;
using SFB;
using qASIC.Files;
using UnityEngine.Events;
using qASIC.Input;

namespace Project.Translation
{
    public class TranslationManager : MonoBehaviour
    {
        public TranslationVersion[] versions;

        public AppFile file = null;
        public string filePath = null;

        [Label("Shortcuts")]
        [SerializeField] InputMapItemReference i_save;
        [SerializeField] InputMapItemReference i_load;
        [SerializeField] InputMapItemReference i_importing;
        [SerializeField] InputMapItemReference i_exporting;

        [Label("Events")]
        public UnityEvent OnImport;
        public UnityEvent OnExport;

        public TranslationVersion CurrentVersion =>
            versions.Length == 0 ?
            null :
            versions[versions.Length - 1];

        private void Awake()
        {
            file = AppFile.Create(CurrentVersion);
        }

        private void Update()
        {
            if (i_save.GetInputDown())
                Save();

            if (i_load.GetInputDown())
                Load();

            if (i_importing.GetInputDown())
                Import();

            if (i_exporting.GetInputDown())
                Export();
        }

        public void Save()
        {
            if (file == null)
            {
                qDebug.LogWarning("Cannot save file, file is null!");
                return;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                var path = StandaloneFileBrowser.SaveFilePanel("Save As", "", "translation", AppFile.FILE_EXTENSION);

                if (string.IsNullOrWhiteSpace(path))
                    return;

                filePath = path;
            }


            FileManager.SaveFileJSON(filePath, file, true);
        }

        public void Load()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Load", "", AppFile.FILE_EXTENSION, false);

            if (paths.Length == 0)
                return;

            filePath = paths[0];

            FileManager.ReadFileJSON(filePath, file);
        }

        public void Export()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", "", false);

            if (paths.Length == 0)
                return;

            foreach (var definesFile in CurrentVersion.defines)
            {
                var txt = definesFile.Export(file);
                FileManager.SaveFileWriter($"{paths[0]}/{definesFile.fileName}", txt);
            }

            OnExport.Invoke();
        }

        public void Import()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", "", false);

            if (paths.Length == 0)
                return;

            foreach (var definesFile in CurrentVersion.defines)
            {
                var txt = FileManager.LoadFileWriter($"{paths[0]}/{definesFile.fileName}");
                definesFile.Import(file, txt);
            }

            OnImport.Invoke();
        }
    }
}