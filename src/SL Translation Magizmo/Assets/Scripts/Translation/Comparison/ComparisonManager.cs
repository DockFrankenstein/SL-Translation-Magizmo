using Project.Settings;
using Project.Translation.Data;
using qASIC;
using qASIC.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Project.Translation.Comparison
{
    [Serializable]
    public class ComparisonManager
    {
        public void Initialize(TranslationManager manager)
        {
            Manager = manager;
            Serializer = new SaveFileSerializer(manager);

            RefreshTranslationList();

            qApplication.QasicInstance.RegisteredObjects.Register(this);

            //FIXME: this is a temporary fix
            CurrentPath = manager.Options.GetOption("comparison_path", "");

            ChangeCurrent(CurrentPath);
        }

        public const string TRANSLATION_FOLDER_PREFIX = "TRANS";

        [SerializeField] RecentsManager loadedFiles;


        public List<string> AvaliableTranslations { get; private set; } = new List<string>();

        public event Action OnChangeCurrent;

        public TranslationManager Manager { get; private set; }

        public SaveFileSerializer Serializer { get; private set; }

        public string CurrentName { get; private set; } = "IDK";
        public SaveFile Current { get; private set; }

        [Option("comparison_path", "")]
        public string CurrentPath { get; set; } = string.Empty;

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

            loadedFiles.Load();

            foreach (var item in loadedFiles)
            {
                AvaliableTranslations.Add(item);
            }
        }

        public void ChangeCurrent(string path)
        {
            path ??= string.Empty;

            path = path.Replace('\\', '/');

            CurrentPath = path;
            Manager.Options.SetOptionAndApply("comparison_path", CurrentPath);

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
                    var extension = Path.GetExtension(path).ToLower();
                    if (extension.Length > 0)
                        extension = extension.Substring(1, extension.Length - 1);

                    switch (extension)
                    {
                        case SaveFile.FILE_EXTENSION:
                            file = Serializer.Load(path);
                            break;
                    }
                }
            }


            Current = file;
            CurrentName = Current.Entries.TryGetValue(Manager.GetSlVersion(Current).GetNameField().id, out var name) ?
                name.content :
                Path.GetFileNameWithoutExtension(path);

            OnChangeCurrent?.Invoke();
        }

        public void AddPath(string path)
        {
            AvaliableTranslations.Add(path);
            loadedFiles.Add(path);
        }
    }
}