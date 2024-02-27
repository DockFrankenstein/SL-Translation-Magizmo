using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Mapping;
using qASIC;
using SFB;
using UnityEngine.Events;
using qASIC.Input;
using System.Linq;
using System;

namespace Project.Translation
{
    public class TranslationManager : MonoBehaviour
    {
        public const int CURRENT_FILE_VERSION = 0;
        public const int LOWEST_SUPPORTED_FILE_VERSION = 0;

        [Label("Mapping")]
        public TranslationVersion[] versions;

        [Label("Application")]
        [SerializeField] ErrorWindow errorWindow;

        public SaveFile File { get; private set; } = null;
        public int FileVersion { get; private set; }
        public string FilePath { get; private set; } = null;

        [Label("Shortcuts")]
        [SerializeField] InputMapItemReference i_save;
        [SerializeField] InputMapItemReference i_saveAs;
        [SerializeField] InputMapItemReference i_load;

        [Label("Events")]
        public UnityEvent OnSave;
        public UnityEvent OnLoad;

        public event Action<object> OnFileChanged;

        public TranslationVersion CurrentVersion { get; private set; }

        public TranslationVersion GetVersion(Version version) =>
            versions
            .Where(x => x.version == version)
            .FirstOrDefault();

        public TranslationVersion GetVersion(SaveFile file) =>
            file.UseNewestSlVersion ?
            GetNewestVersion() :
            GetVersion(file.SlVersion);

        public TranslationVersion GetNewestVersion() =>
            versions.LastOrDefault();

        public void LoadCurrentVersionFromFile() =>
            CurrentVersion = GetVersion(File);

        private void Awake()
        {
            foreach (var version in versions)
                version.Initialize();

            CurrentVersion = GetNewestVersion();

            File = new SaveFile(CurrentVersion);
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
                return;
            }

            if (!CheckPath()) return;

            try
            {
                var json = JsonUtility.ToJson(File, true);
                var txt = $"{CURRENT_FILE_VERSION}\n{json}";
                System.IO.File.WriteAllText(FilePath, txt);
            }
            catch (Exception e)
            {
                errorWindow.CreatePrompt("Save Error", $"Application ran into a problem while saving file.\n {e}");
                return;
            }

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

            FilePath = paths[0];

            try
            {
                var txt = System.IO.File.ReadAllText(FilePath);

                var lines = txt.SplitByLines();
                var fileVersionString = lines.First();
                var fileVersion = CURRENT_FILE_VERSION;

                if (int.TryParse(fileVersionString, out int newFileVersion))
                {
                    if (newFileVersion < LOWEST_SUPPORTED_FILE_VERSION)
                    {
                        errorWindow.CreatePrompt("Load Error", $"This file has been saved in an older version ({newFileVersion}) that is no longer supported (lowest supported version: {LOWEST_SUPPORTED_FILE_VERSION}).");
                        return;
                    }

                    if (newFileVersion > fileVersion)
                    {
                        errorWindow.CreatePrompt("Load Error", $"This file has been saved in a newer version ({newFileVersion}, current version: {CURRENT_FILE_VERSION}). You have to update the application in order to load it.");
                        return;
                    }

                    fileVersion = newFileVersion;
                    txt = string.Join("\n", lines.Skip(1));
                }

                var file = JsonUtility.FromJson<SaveFile>(txt);
                var ver = GetVersion(file);

                if (ver == null)
                {
                    ver = GetNewestVersion();
                    errorWindow.CreatePrompt("Unsupported SL Version", $"This file is targetting an unsupported version of SCP: Secret Laboratory ({file.SlVersion}). Changed version to {ver.version}");
                }

                CurrentVersion = ver;

                File = file;
                FileVersion = fileVersion;
            }
            catch (Exception e)
            {
                errorWindow.CreatePrompt("Load Error", $"Application ran into a problem whilte loading file.\n{e}");
                return;
            }

            OnLoad.Invoke();
            MarkFileDirty(this);
        }

        public void MarkFileDirty(object fromContext)
        {
            OnFileChanged?.Invoke(fromContext);
        }
    }
}