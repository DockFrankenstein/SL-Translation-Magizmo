using UnityEngine;
using Project.Translation.Data;
using Project.Translation.Mapping;
using qASIC;
using SFB;
using qASIC.Files;
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

            File = SaveFile.Create(CurrentVersion);
        }

        private void Update()
        {
            if (i_save.GetInputDown())
                Save();

            if (i_saveAs.GetInputDown())
                SaveAs();

            if (i_load.GetInputDown())
                Load();

            if (i_importing.GetInputDown())
                Import();

            if (i_exporting.GetInputDown())
                Export();
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

        public void Load()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Load", "", SaveFile.FILE_EXTENSION, false);

            if (paths.Length == 0)
                return;

            FilePath = paths[0];

            try
            {
                var txt = System.IO.File.ReadAllText(FilePath);

                var lines = txt.SplitByLines();
                var versionString = lines.First();
                var version = CURRENT_FILE_VERSION;

                if (int.TryParse(versionString, out int newVersion))
                {
                    if (newVersion < LOWEST_SUPPORTED_FILE_VERSION)
                    {
                        errorWindow.CreatePrompt("Load Error", $"This file has been saved in an older version ({newVersion}) that is no longer supported (lowest supported version: {LOWEST_SUPPORTED_FILE_VERSION}).");
                        return;
                    }

                    if (newVersion > version)
                    {
                        errorWindow.CreatePrompt("Load Error", $"This file has been saved in a newer version ({newVersion}, current version: {CURRENT_FILE_VERSION}). You have to update the application in order to load it.");
                        return;
                    }

                    version = newVersion;
                    txt = string.Join("\n", lines.Skip(1));
                }

                File = JsonUtility.FromJson<SaveFile>(txt);
                FileVersion = version;
            }
            catch (Exception e)
            {
                errorWindow.CreatePrompt("Load Error", $"Application ran into a problem whilte loading file.\n{e}");
                return;
            }

            OnLoad.Invoke();
        }

        public void Export()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", Settings.GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            CurrentVersion.Export(File, paths[0]);
            OnExport.Invoke();
        }

        public void Import()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", Settings.GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            CurrentVersion.Import(File, paths[0]);
            OnImport.Invoke();
        }
    }
}