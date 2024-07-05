using Project.Settings;
using Project.Translation.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Project.Translation.Comparison
{
    public class ComparisonTranslationManager
    {
        public ComparisonTranslationManager(TranslationManager manager)
        {
            Manager = manager;
            Serializer = new SaveFileSerializer(manager);

            RefreshTranslationList();
        }

        public const string TRANSLATION_FOLDER_PREFIX = "TRANS";


        public List<string> AvaliableTranslations = new List<string>();

        public event Action OnChangeCurrent;

        public TranslationManager Manager { get; set; }
        public SaveFileSerializer Serializer { get; set; }

        public string CurrentName { get; private set; } = "IDK";
        public SaveFile Current { get; private set; }

        public bool TryGetEntryData(string id, out string content)
        {
            if (Current != null && Current.Entries.TryGetValue(id, out var data))
            {
                content = data.content;
                return true;
            }

            content = string.Empty;
            return false;
        }

        public void RefreshTranslationList()
        {
            AvaliableTranslations.Clear();

            if (Directory.Exists(GeneralSettings.TranslationPath))
            {
                foreach (var folder in Directory.GetDirectories(GeneralSettings.TranslationPath))
                {
                    var folderName = Path.GetFileName(folder);
                    var files = Directory.GetFiles(folder)
                        .Where(x => Path.GetExtension(x).ToLower() == SaveFile.FILE_EXTENSION);

                    var fileCount = files.Count();
                    if (fileCount != 1)
                    {
                        AvaliableTranslations.Add($"{TRANSLATION_FOLDER_PREFIX}/{folderName}");
                    }            
                    
                    foreach (var file in files)
                    {
                        AvaliableTranslations.Add($"{TRANSLATION_FOLDER_PREFIX}/{folderName}/{Path.GetFileName(file)}");
                    }
                }
            }
        }

        public void ChangeCurrent(string path)
        {
            path = path.Replace('\\', '/');

            if (path.StartsWith($"{TRANSLATION_FOLDER_PREFIX}/"))
                path = $"{GeneralSettings.TranslationPath}/{path.Substring(TRANSLATION_FOLDER_PREFIX.Length + 1, path.Length - TRANSLATION_FOLDER_PREFIX.Length - 1)}";

            SaveFile file = new SaveFile()
            {
                SlVersion = Manager.CurrentVersion.version,
            };

            if (!string.IsNullOrWhiteSpace(path))
            {
                if (Directory.Exists(path))
                {
                    Manager.CurrentVersion.Import(file, path);
                }

                if (File.Exists(path))
                {
                    switch (Path.GetExtension(path).ToLower())
                    {
                        case SaveFile.FILE_EXTENSION:
                            file = Serializer.Load(path);
                            break;
                    }
                }
            }


            Current = file;
            CurrentName = Current.Entries.TryGetValue(Manager.GetVersion(Current.SlVersion).GetNameField().id, out var name) ?
                name.content :
                Path.GetFileNameWithoutExtension(path);
            OnChangeCurrent?.Invoke();
        }
    }
}