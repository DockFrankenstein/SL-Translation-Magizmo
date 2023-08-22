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
                StringBuilder definesFileContent = new StringBuilder();
                foreach (var define in definesFile.defines)
                {
                    var values = define.fieldIds
                        .Select(x => file.Entries.TryGetValue(x, out var y) ?
                            y.content :
                            string.Empty);

                    switch (definesFile.identificationType)
                    {
                        default:
                            definesFileContent.Append($"{string.Join(definesFile.separationCharacter, values)}\n");
                            break;
                        case TranslationDefines.IdentificationType.FirstItem:
                            definesFileContent.Append($"{define.lineId}{definesFile.separationCharacter}{string.Join(definesFile.separationCharacter, values)}\n");
                            break;
                    }
                }

                FileManager.SaveFileWriter($"{paths[0]}/{definesFile.fileName}", definesFileContent.ToString());
            }
        }

        public void Import()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", "", false);

            if (paths.Length == 0)
                return;

            foreach (var definesFile in CurrentVersion.defines)
            {
                var txtLines = FileManager.LoadFileWriter($"{paths[0]}/{definesFile.fileName}")
                    .Replace("\r\n", "\n")
                    .Split('\n')
                    .ToArray();

                for (int i = 0; i < definesFile.defines.Count; i++)
                {
                    var define = definesFile.defines[i];

                    foreach (var id in define.fieldIds)
                        if (!file.Entries.ContainsKey(id))
                            file.Entries.Add(id, new AppFile.EntryData(id));

                    string line;

                    switch (definesFile.identificationType)
                    {
                        //Search by line index
                        default:
                            if (!txtLines.IndexInRange(i))
                                continue;

                            line = txtLines[i];
                            break;

                        case TranslationDefines.IdentificationType.FirstItem:
                            var startsWithText = $"{define.lineId}{definesFile.separationCharacter}";

                            var targetLines = txtLines
                                .Where(x => x.StartsWith(startsWithText));

                            if (targetLines.Count() == 0)
                                continue;

                            line = targetLines.First().Remove(0, startsWithText.Length);
                            break;
                    }

                    switch (definesFile.useSeparationCharacter)
                    {
                        case true:
                            var splitLine = line.Split(definesFile.separationCharacter);

                            for (int x = 0; x < Mathf.Min(define.fieldIds.Length, splitLine.Length); x++)
                                file.Entries[define.fieldIds[x]] = new AppFile.EntryData(define.fieldIds[x], splitLine[x]);
                            break;
                        case false:
                            file.Entries[define.fieldIds[0]] = new AppFile.EntryData(define.fieldIds[0], line);
                            break;
                    }
                }
            }
        }
    }
}