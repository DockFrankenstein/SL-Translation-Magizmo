using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Mapping;
using qASIC;
using SFB;
using UnityEngine.Events;
using qASIC.Input;
using System.Linq;
using System;
using Project.Translation.Comparison;
using qASIC.Options;
using qASIC.Files;
using System.Collections.Generic;
using Project.Undo;

namespace Project.Translation
{
    public class TranslationManager : MonoBehaviour
    {
        [Label("Mapping")]
        public TranslationVersion[] versions;

        [Label("Saving")]
        [SerializeField] RecentsManager recentFiles = new RecentsManager();

        [Label("Settings")]
        [SerializeField] AdvancedGenericFilePath settingsPath = new AdvancedGenericFilePath(GenericFolder.PersistentDataPath, "settings.txt");

        [Label("Comparisons")]
        [SerializeField] ComparisonManager comparisonManager;

        [Label("Application")]
        [SerializeField] NotificationManager notifications;
        [SerializeField] ErrorWindow errorWindow;
        [SerializeField] UndoManager undo;

        public SaveFile File { get; set; } = null;
        public string FilePath { get; private set; } = null;

        [Label("Shortcuts")]
        public InputMapItemReference i_save;
        public InputMapItemReference i_saveAs;
        public InputMapItemReference i_load;

        [Label("Events")]
        public UnityEvent OnSave;
        public UnityEvent OnCancelSave;
        public UnityEvent OnLoad;

        public event Func<string, bool> OnWantToLoad;

        public OptionsManager Options { get; private set; }

        public SaveFileSerializer Serializer { get; private set; }
        public ComparisonManager ComparisonManager => comparisonManager;
        public RecentsManager RecentFiles =>
            recentFiles;

        TranslationVersion _currentVersion;
        public TranslationVersion CurrentVersion 
        {
            get => _currentVersion;
            private set
            {
                if (_currentVersion == value) return;

                _currentVersion = value;
                OnCurrentVersionChanged?.Invoke(value);
            }
        }

        public event Action<TranslationVersion> OnCurrentVersionChanged;

        public bool IsLoading { get; private set; }

        public TranslationVersion GetVersion(Version version) =>
            versions
            .Where(x => x.version == version)
            .FirstOrDefault();

        public TranslationVersion GetSlVersion(SaveFile file) =>
            file.UseNewestSlVersion ?
            GetNewestVersion() :
            GetVersion(file.SlVersion);

        public TranslationVersion GetNewestVersion() =>
            versions.LastOrDefault();

        public void LoadCurrentVersionFromFile() =>
            CurrentVersion = GetSlVersion(File);

        private void Awake()
        {
            var optionsSerializer = new OptionsSerializer(settingsPath.GetFullPath());
            optionsSerializer.OnSave += data =>
            {
                var txt = string.Empty;
                foreach (var item in data)
                    txt = ConfigController.SetSetting(txt, item.Key, item.Value?.ToString());

                return txt;
            };

            optionsSerializer.OnLoad += txt =>
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                foreach (var item in Options.OptionsList)
                {
                    try
                    {
                        var txtVal = ConfigController.GetSetting(txt, item.Key);
                        data.Add(item.Key, Convert.ChangeType(txtVal, item.Value.Value.GetType()));
                    }
                    catch { }
                }

                return data;
            };

            Options = new OptionsManager(qApplication.QasicInstance, optionsSerializer);
            Options.Initialize();

            foreach (var version in versions)
                version.Initialize();

            Serializer = new SaveFileSerializer(this);
            CurrentVersion = GetNewestVersion();
            File = new SaveFile(CurrentVersion);

            ComparisonManager.Initialize(this);

            recentFiles.Load();
        }

        private void Update()
        {
            if (i_save.GetInputDown())
                Save();

            if (i_saveAs.GetInputDown())
                SaveAs();

            if (i_load.GetInputDown())
                Open();
        }

        public void Save()
        {
            if (File == null)
            {
                errorWindow.CreatePrompt("Save Error", "Cannot save file, file is null. This is an error in the program, please report this issue.");
                OnCancelSave.Invoke();
                return;
            }

            if (!CheckPath())
            {
                OnCancelSave.Invoke();
                return;
            }

            try
            {
                Serializer.Save(FilePath, File);
                recentFiles.Add(FilePath);
                undo.ClearDirty();
                Debug.Log("Saved file");
                notifications.Notify("File saved.");
                OnSave.Invoke();
            }
            catch (Exception e)
            {
                errorWindow.CreatePrompt("Save Error", $"Application ran into a problem while saving file.\n {e}");
                OnCancelSave.Invoke();
                return;
            }
        }

        public void SaveAs()
        {
            if (!ChangePath())
            {
                OnCancelSave.Invoke();
                return;
            }

            Save();
        }

        bool CheckPath()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                return ChangePath();

            return true;
        }

        bool ChangePath()
        {
            var path = StandaloneFileBrowser.SaveFilePanel("Save As", "", "translation", SaveFile.FILE_EXTENSION);

            if (string.IsNullOrWhiteSpace(path))
                return false;

            FilePath = path;
            return true;
        }

        public void Open()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Load", "", SaveFile.FILE_EXTENSION, false);

            if (paths.Length == 0)
                return;

            Open(paths[0]);
        }

        public void Open(string path)
        {
            if (OnWantToLoad?.Invoke(path) == false)
                return;

            IsLoading = true;
            FilePath = path;

            try
            {
                var file = Serializer.Load(path);

                File = file;
                LoadCurrentVersionFromFile();
            }
            catch (SaveFileSerializer.SerializerException e)
            {
                errorWindow.CreatePrompt("Load Error", e.Message);
                return;
            }
            catch (Exception e)
            {
                errorWindow.CreatePrompt("Load Error", $"Application ran into a problem whilte loading file.\n{e}");
                return;
            }

            IsLoading = false;
            OnLoad.Invoke();
            notifications.Notify($"Loaded file {FilePath}");
            undo.ClearAll();
            recentFiles.Add(path);
        }
    }
}