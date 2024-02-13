using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Mapping;
using System;
using qASIC;
using SFB;
using qASIC.Files;
using UnityEngine.Events;
using qASIC.Input;
using System.Threading.Tasks;
using System.Threading;

namespace Project.Translation
{
    public class TranslationManager : MonoBehaviour
    {
        public TranslationVersion[] versions;

        public SaveFile file = null;
        public string filePath = null;

        [Label("Shortcuts")]
        [SerializeField] InputMapItemReference i_save;
        [SerializeField] InputMapItemReference i_load;
        [SerializeField] InputMapItemReference i_importing;
        [SerializeField] InputMapItemReference i_exporting;

        [Label("Events")]
        public UnityEvent OnImport;
        public UnityEvent OnExport;
        public UnityEvent OnSave;
        public UnityEvent OnLoad;

        public TranslationVersion CurrentVersion =>
            versions.Length == 0 ?
            null :
            versions[versions.Length - 1];

        private void Awake()
        {
            foreach (var version in versions)
                version.Initialize();

            file = SaveFile.Create(CurrentVersion);
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
                var path = StandaloneFileBrowser.SaveFilePanel("Save As", "", "translation", SaveFile.FILE_EXTENSION);

                if (string.IsNullOrWhiteSpace(path))
                    return;

                filePath = path;
            }

            FileManager.SaveFileJSON(filePath, file, true);
            OnSave.Invoke();
        }

        public void Load()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Load", "", SaveFile.FILE_EXTENSION, false);

            if (paths.Length == 0)
                return;

            filePath = paths[0];

            FileManager.ReadFileJSON(filePath, file);
            OnLoad.Invoke();
        }

        public void Export()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", Settings.GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            CurrentVersion.Export(file, paths[0]);
            OnExport.Invoke();
        }

        public void Import()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", Settings.GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            CurrentVersion.Import(file, paths[0]);
            OnImport.Invoke();
        }
    }
}