using Project.Settings;
using Project.Translation.Data;
using System;
using System.Collections.Generic;
using System.IO;
using qASIC.SettingsSystem;
using UnityEngine;

namespace Project.Translation.Comparison
{
    public class ComparisonTranslationManager
    {
        public ComparisonTranslationManager(TranslationManager manager)
        {
            Manager = manager;
            Serializer = new SaveFileSerializer(manager);

            OnSettingsChanged += CheckAndImportTranslations;

            CheckAndImportTranslations();
            ChangeCurrent(Sett_CurrentName);
        }

        private ComparisonTranslation _currentTranslation;
        public ComparisonTranslation CurrentTranslation
        {
            get => _currentTranslation;
            private set
            {
                _currentTranslation = value;
                OnChangeCurrent?.Invoke(value);
            }
        }


        public Dictionary<string, ComparisonTranslation> translations = new Dictionary<string, ComparisonTranslation>();

        public event Action<ComparisonTranslation> OnChangeCurrent;

        public TranslationManager Manager { get; set; }
        public SaveFileSerializer Serializer { get; set; }

        #region Settings
        static Action OnSettingsChanged;

        static bool Sett_UseCache { get; set; } = true;

        [OptionsSetting("comparison_use_cache", true)]
        static void SettM_UseCache(bool val)
        {
            Sett_UseCache = val;
            OnSettingsChanged?.Invoke();
        }

        static string Sett_CurrentName { get; set; } = string.Empty;

        [OptionsSetting("comparison_name", true)]
        static void SettM_CurrentName(string val)
        {
            Sett_CurrentName = val;
        }

        static string Sett_CacheLocation { get; set; } = SettV_CacheLocation();

        [OptionsSetting("comparison_cache_location", defaultValueMethodName = nameof(SettV_CacheLocation))]
        static void SettM_CacheLocation(string val)
        {
            Sett_CacheLocation = val;
            OnSettingsChanged?.Invoke();
        }

        static string SettV_CacheLocation() =>
            $"{Application.persistentDataPath.Replace('/', '\\')}\\comparison-cache";
        #endregion

        public bool TryGetEntryData(string id, out string content)
        {
            if (CurrentTranslation?.file != null && CurrentTranslation.file.Entries.TryGetValue(id, out var data))
            {
                content = data.content;
                return true;
            }

            content = string.Empty;
            return false;
        }

        public void CheckAndImportTranslations()
        {
            translations.Clear();

            var translationsPath = GeneralSettings.TranslationPath;
            if (!Directory.Exists(translationsPath)) return;

            var paths = Directory.GetDirectories(translationsPath);

            foreach (var item in paths)
            {
                try
                {
                    var name = Path.GetFileName(item);
                    var file = GetFile(item);

                    translations.Add(name, new ComparisonTranslation()
                    {
                        file = file,
                        displayName = file.Entries.TryGetValue("manifest_name", out var value) ?
                            value.content.Replace("/", "\\\\") :
                            name,
                    });
                }
                catch { }
            }
        }

        public void ChangeCurrent(string name)
        {
            OptionsController.ChangeOption("comparison_name", name);

            if (!translations.ContainsKey(name))
            {
                CurrentTranslation = null;
                return;
            }

            CurrentTranslation = translations[name];
        }

        SaveFile GetFile(string path)
        {
            var name = Path.GetFileName(path);

            try
            {
                if (!Directory.Exists(Sett_CacheLocation))
                    Directory.CreateDirectory(Sett_CacheLocation);
            }
            catch { }

            var cachePath = Directory.Exists(Sett_CacheLocation) ?
                $"{Sett_CacheLocation.TrimEnd('/')}/{name}.{SaveFile.FILE_EXTENSION}" :
                null;

            //File exists
            if (File.Exists(cachePath))
                return Serializer.Load(cachePath);

            //File doesn't exist
            var file = new SaveFile();
            Manager.CurrentVersion.Import(file, path);

            if (cachePath != null)
                Serializer.Save(cachePath, file);

            return file;
        }

        public class ComparisonTranslation
        {
            public SaveFile file;
            public string displayName;
        }
    }
}