using UnityEngine;
using Project.AutoUpdate;
using UnityEngine.UIElements;
using qASIC.SettingsSystem;
using System.Collections;
using qASIC.Files;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Project.GUI.Other
{
    public class AutoUpdateWindow : MonoBehaviour
    {
        [SerializeField] UIDocument document;
        [SerializeField] ErrorWindow errorWindow;

        [Label("Text")]
        [SerializeField] string checkingHeader;
        [SerializeField][TextArea] string checkingDescription;

        [Space]
        [SerializeField] string upToDateHeader;
        [SerializeField][TextArea] string upToDateDescription;

        [Space]
        [SerializeField] string newUpdateHeader;
        [SerializeField][TextArea] string newUpdateDescription;

        [Space]
        [SerializeField] string updatingHeader;
        [SerializeField][TextArea] string updatingDescription;

        [Space]
        [SerializeField] string finishedHeader;
        [SerializeField][TextArea] string finishedDescription;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        VisualElement fade;
        Label header;
        Label description;
        VisualElement buttons;
        Button cancelButton;
        Button dontShowButton;
        Button updateButton;
        ProgressBar progress;

        AutoUpdater _updater;

        public bool IsOpened { get; private set; }

        //TODO: replace the settings system.
        //Put it on a hard drive and burn it.
        //Use explosives if you must.
        //I hate this thing with every fiber of my body
        //and I am the one who coded it.
        //This was not my doing.
        //The devil possessed me to unleash this
        //horriblness upon land.
        //
        //It must be destroyed.
        #region Settings
        const string SHOW_AUTO_UPDATE_KEY = "show_auto_update";

        public static bool Sett_AutoUpdate { get; set; } = true;

        [OptionsSetting("show_auto_update", true)]
        static void SettM_AutoUpdate(bool value)
        {
            Sett_AutoUpdate = value;
        }
        #endregion

        private void Awake()
        {
            _updater = new AutoUpdater()
            {
                TargetFileName = "Installer.exe",
                ResultPath = $"{FileManager.GetGenericFolderPath(GenericFolder.UserProfile)}\\Downloads\\SL Translation Magizmo Installer v{{0}}.exe",
                CurrentVersion = Application.version,
            };

            var root = document.rootVisualElement;
            root.ChangeDispaly(false);

            fade = root.Q("fade");
            header = root.Q<Label>("header");
            description = root.Q<Label>("description");
            buttons = root.Q("buttons");
            cancelButton = root.Q<Button>("cancel");
            dontShowButton = root.Q<Button>("dont-show");
            updateButton = root.Q<Button>("update");
            progress = root.Q<ProgressBar>("progress");

            fade.RegisterCallback<ClickEvent>(x =>
            {
                if (x.target == fade)
                    Close();
            });

            cancelButton.clicked += Close;
            dontShowButton.clicked += () =>
            {
                OptionsController.ChangeOption(SHOW_AUTO_UPDATE_KEY, false);
                Close();
            };

            updateButton.clicked += UpdateApp;

#if !UNITY_EDITOR
            StartCoroutine(CheckForUpdatesStartup());
#endif

            Application.quitting += () =>
            {
                if (_updater.UpdaterStatus == AutoUpdater.Status.ReadyToFinalizeUpdate)
                {
                    //We have to put this on a different thread or else
                    //we will create an infinite loop of admin prompts
                    //that can only be stopped by force closing sltm
                    //via the task manager
                    //John Riccitiello my beloved
                    new Thread(() =>
                    {
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = _updater.GetFinalOutputPath(),
                            Arguments = $"--update \"{FileManager.TrimPathEnd(Application.dataPath, 1)}\" --delete-after",
                        });
                    }).Start();
                }
            };
        }

        IEnumerator CheckForUpdatesStartup()
        {
            yield return _updater.CheckForUpdates();

            if (!Sett_AutoUpdate) yield break;
            if (_updater.UpdaterStatus == AutoUpdater.Status.UpdateAvaliable)
                Open(true);
        }

        IEnumerator CheckForUpdates()
        {
            yield return _updater.CheckForUpdates();
            UpdateContent();
        }

        void UpdateApp()
        {
            StartCoroutine(DownloadUpdate());
            UpdateContent();
        }

        IEnumerator DownloadUpdate()
        {
            yield return _updater.DownloadUpdate();

            UpdateContent();
        }

        private void Update()
        {
            if (!string.IsNullOrEmpty(_updater.error))
            {
                if (IsOpened)
                {
                    errorWindow.CreatePrompt($"There was an error while updating", _updater.error);
                    _updater.ClearError();
                    Close();
                }
            }
        }

        public void Open(bool auto = false)
        {
            if (_updater.UpdaterStatus == AutoUpdater.Status.NotPrepared ||
                _updater.UpdaterStatus == AutoUpdater.Status.CheckingForUpdates ||
                _updater.UpdaterStatus == AutoUpdater.Status.CheckingForUpdatesError)
                StartCoroutine(CheckForUpdates());

            UpdateContent();

            dontShowButton.ChangeDispaly(auto && Sett_AutoUpdate);

            IsOpened = true;
            document.rootVisualElement.ChangeDispaly(true);
        }

        void UpdateContent()
        {
            header.text = _updater.UpdaterStatus switch
            {
                AutoUpdater.Status.NotPrepared => checkingHeader,
                AutoUpdater.Status.CheckingForUpdates => checkingHeader,
                AutoUpdater.Status.CheckingForUpdatesError => checkingHeader,
                AutoUpdater.Status.UpdateAvaliable => newUpdateHeader,
                AutoUpdater.Status.DownloadingUpdate => updatingHeader,
                AutoUpdater.Status.DownloadingUpdateError => updatingHeader,
                AutoUpdater.Status.ReadyToFinalizeUpdate => finishedHeader,
                _ => upToDateHeader,
            };

            description.text = _updater.UpdaterStatus switch
            {
                AutoUpdater.Status.NotPrepared => checkingDescription,
                AutoUpdater.Status.CheckingForUpdates => checkingDescription,
                AutoUpdater.Status.CheckingForUpdatesError => checkingDescription,
                AutoUpdater.Status.UpdateAvaliable => newUpdateDescription,
                AutoUpdater.Status.DownloadingUpdate => updatingDescription,
                AutoUpdater.Status.DownloadingUpdateError => updatingDescription,
                AutoUpdater.Status.ReadyToFinalizeUpdate => finishedDescription,
                _ => upToDateDescription,
            };

            cancelButton.text = _updater.UpdaterStatus switch
            {
                AutoUpdater.Status.UpToDate or
                AutoUpdater.Status.ReadyToFinalizeUpdate or
                AutoUpdater.Status.CheckingForUpdatesError or
                AutoUpdater.Status.UpdateAvaliable or
                AutoUpdater.Status.DownloadingUpdateError => "Close",
                AutoUpdater.Status.DownloadingUpdate or
                AutoUpdater.Status.NotPrepared or
                AutoUpdater.Status.CheckingForUpdates or
                _ => "Cancel",
            };

            bool isUpdating = _updater.UpdaterStatus == AutoUpdater.Status.DownloadingUpdate;

            buttons.ChangeDispaly(!isUpdating);
            updateButton.ChangeDispaly(_updater.UpdaterStatus == AutoUpdater.Status.UpdateAvaliable);
            dontShowButton.ChangeDispaly(false);
        }

        void Close()
        {
            if (_updater.UpdaterStatus == AutoUpdater.Status.DownloadingUpdate)
                return;

            IsOpened = false;
            document.rootVisualElement.ChangeDispaly(false);
        }

        [ContextMenu("Set as ready to finalize")]
        void SetToFinalize()
        {
            _updater.UpdaterStatus = AutoUpdater.Status.ReadyToFinalizeUpdate;
        }
    }
}