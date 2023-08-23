using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Defines;
using System;
using qASIC;
using SFB;
using qASIC.Files;
using System.Text;
using System.Linq;

namespace Project.Translation
{
    public class TranslationManager : MonoBehaviour
    {
        public TranslationVersion[] versions;

        public AppFile file = null;
        public string filePath = null;

        private string _selectedItem = null;
        public string SelectedItem 
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnSelectionChange?.Invoke(value);
            }
        }

        public event Action<string> OnSelectionChange;

        public TranslationVersion CurrentVersion =>
            versions.Length == 0 ?
            null :
            versions[versions.Length - 1];

        private void Awake()
        {
            file = AppFile.Create(CurrentVersion);
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
        }
    }
}