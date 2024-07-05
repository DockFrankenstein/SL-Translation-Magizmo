﻿using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Mapping;
using qASIC;
using SFB;
using UnityEngine.Events;
using qASIC.Input;
using System.Linq;
using System;
using Project.Translation.Comparison;

namespace Project.Translation
{
    public class TranslationManager : MonoBehaviour
    {
        [Label("Mapping")]
        public TranslationVersion[] versions;

        [Label("Saving")]
        [SerializeField] RecentsManager recentFiles = new RecentsManager();

        [Label("Comparisons")]
        [SerializeField] ComparisonManager comparisonManager;

        [Label("Application")]
        [SerializeField] ErrorWindow errorWindow;

        public SaveFile File { get; private set; } = null;
        public string FilePath { get; private set; } = null;

        [Label("Shortcuts")]
        [SerializeField] InputMapItemReference i_save;
        [SerializeField] InputMapItemReference i_saveAs;
        [SerializeField] InputMapItemReference i_load;

        [Label("Events")]
        public UnityEvent OnSave;
        public UnityEvent OnCancelSave;
        public UnityEvent OnLoad;

        public event Action<object> OnFileChanged;

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
        public bool IsDirty { get; private set; }

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
                ClearDirty();
                Debug.Log("Saved file");
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
            MarkFileDirty(this);
            ClearDirty();
            recentFiles.Add(path);
        }

        public void MarkFileDirty(object fromContext)
        {
            PUtility.ChangeWindowTitle($"*{Application.productName}");
            IsDirty = true;
            OnFileChanged?.Invoke(fromContext);
        }

        private void ClearDirty()
        {
            IsDirty = false;
            PUtility.ChangeWindowTitle(Application.productName);
        }
    }
}