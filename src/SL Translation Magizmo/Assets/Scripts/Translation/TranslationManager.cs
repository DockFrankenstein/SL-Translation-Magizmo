using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Mapping;
using qASIC;
using SFB;
using UnityEngine.Events;
using qASIC.Input;
using System.Linq;
using System;
using qASIC.Files;
using System.Collections.Generic;
using System.IO;
using Project.Translation.Comparison;

namespace Project.Translation
{
    public class TranslationManager : MonoBehaviour
    {
        [Label("Mapping")]
        public TranslationVersion[] versions;

        [Label("Saving")]
        [SerializeField] GenericFilePath recentFilesCachePath;

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
        public UnityEvent OnLoad;

        public event Action<object> OnFileChanged;

        public List<string> RecentPaths { get; private set; }
        public event Action<string> OnRecentPathAdded;

        public SaveFileSerializer Serializer { get; private set; }
        public ComparisonTranslationManager ComparisonManager { get; private set; }

        public TranslationVersion CurrentVersion { get; private set; }

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

            ComparisonManager = new ComparisonTranslationManager(this);

            LoadRecentsCache();
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

        public void LoadRecentsCache()
        {
            var cachePath = recentFilesCachePath.GetFullPath();

            if (!System.IO.File.Exists(cachePath))
                return;

            using (var stream = new StreamReader(cachePath))
            {
                var txt = stream.ReadToEnd();
                RecentPaths = txt
                    .SplitByLines()
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }
        }

        public void SaveRecentsCache()
        {
            var cachePath = recentFilesCachePath.GetFullPath();

            using (var stream = new StreamWriter(cachePath))
            {
                var txt = string.Join("\n", RecentPaths);
                stream.Write(txt);
            }
        }

        public void AddPathToRecents(string path)
        {
            if (RecentPaths == null)
            {
                RecentPaths = new List<string>(new string[] { path });
                SaveRecentsCache();
                return;
            }

            if (RecentPaths.FirstOrDefault() == path)
                return;

            if (RecentPaths.Contains(path))
                RecentPaths.Remove(path);

            RecentPaths.Insert(0, path);
            OnRecentPathAdded?.Invoke(path);
            SaveRecentsCache();
        }

        public void Save()
        {
            if (File == null)
            {
                errorWindow.CreatePrompt("Save Error", "Cannot save file, file is null. This is an error in the program, please report this issue.");
                return;
            }

            if (!CheckPath()) return;

            try
            {
                Serializer.Save(FilePath, File);
            }
            catch (Exception e)
            {
                errorWindow.CreatePrompt("Save Error", $"Application ran into a problem while saving file.\n {e}");
                return;
            }

            AddPathToRecents(FilePath);
            Debug.Log("Saved file");
            OnSave.Invoke();
        }

        public void SaveAs()
        {
            if (!ChangePath()) return;
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
            FilePath = path;

            try
            {


                //var txt = System.IO.File.ReadAllText(FilePath);

                //var lines = txt.SplitByLines();
                //var fileVersionString = lines.First();
                //var fileVersion = TranslationFileUpdater.CURRENT_FILE_VERSION;

                //if (int.TryParse(fileVersionString, out int newFileVersion))
                //{
                //    if (newFileVersion < TranslationFileUpdater.LOWEST_SUPPORTED_FILE_VERSION)
                //    {
                //        errorWindow.CreatePrompt("Load Error", $"This file has been saved in an older version ({newFileVersion}) that is no longer supported (lowest supported version: {TranslationFileUpdater.LOWEST_SUPPORTED_FILE_VERSION}).");
                //        return;
                //    }

                //    if (newFileVersion > fileVersion)
                //    {
                //        errorWindow.CreatePrompt("Load Error", $"This file has been saved in a newer version ({newFileVersion}, current version: {TranslationFileUpdater.CURRENT_FILE_VERSION}). You have to update the application in order to load it.");
                //        return;
                //    }

                //    fileVersion = newFileVersion;
                //    txt = string.Join("\n", lines.Skip(1));
                //}

                //var file = JsonUtility.FromJson<SaveFile>(txt);
                //_fileUpdater.EnsureFileIsUpToDate(file, fileVersion);

                //var slVer = GetSlVersion(file);

                //if (slVer == null)
                //{
                //    slVer = GetNewestVersion();
                //    errorWindow.CreatePrompt("Unsupported SL Version", $"This file is targetting an unsupported version of SCP: Secret Laboratory ({file.SlVersion}). Changed version to {slVer.version}");
                //}

                //CurrentVersion = slVer;

                //File = file;
                //FileVersion = fileVersion;

                var file = Serializer.Load(path);
                File = file;
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

            OnLoad.Invoke();
            MarkFileDirty(this);
            AddPathToRecents(FilePath);
        }

        public void MarkFileDirty(object fromContext)
        {
            OnFileChanged?.Invoke(fromContext);
        }
    }
}