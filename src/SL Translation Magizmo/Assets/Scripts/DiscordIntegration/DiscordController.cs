using Project.Translation;
using UnityEngine;
using Discord;
using qASIC.SettingsSystem;

using DiscordClient = Discord.Discord;
using Project.GUI.Hierarchy;
using System;

namespace Project.DiscordIntegration
{
    public class DiscordController : MonoBehaviour
    {
        [Label("Assign")]
        [SerializeField] TranslationManager manager;
        [SerializeField] HierarchyController hierarchy;

        [Label("Client")]
        [EditorButton(nameof(UpdateActivity))]
        [SerializeField] long clientId;

        [Label("Details")]
        [SerializeField] string smallImage = "logo";
        [SerializeField] string largeImage = "logo";


        public DiscordClient Client { get; private set; } = null;
        public ActivityManager ActivityManager { get; private set; }

        #region Settings
        private static event Action SettA_UseActivity;

        [OptionsSetting("discord_use_activity", true)]
        private static void SettM_UseActivity(bool value)
        {
            Sett_UseActivity = value;
            SettA_UseActivity?.Invoke();
        }

        private static bool Sett_UseActivity { get; set; } = true;
        #endregion

        private void Reset()
        {
            manager = FindObjectOfType<TranslationManager>();
            hierarchy = FindObjectOfType<HierarchyController>();
        }

        private void Awake()
        {
            UpdateIntegrationStatus();
        }

        private void OnEnable()
        {
            SettA_UseActivity += UpdateIntegrationStatus;
            hierarchy.OnSelect += _ => UpdateActivity();
            manager.OnFileChanged += _ => UpdateActivity();
        }

        private void OnDisable()
        {
            SettA_UseActivity -= UpdateIntegrationStatus;
            hierarchy.OnSelect -= _ => UpdateActivity();
            manager.OnFileChanged -= _ => UpdateActivity();
        }

        private void OnDestroy()
        {
            StopIntegration();
        }

        void UpdateIntegrationStatus()
        {
            switch (Sett_UseActivity)
            {
                case true:
                    StartIntegration();
                    break;
                case false:
                    StopIntegration();
                    break;
            }
        }

        void StartIntegration()
        {
            if (Client != null)
                return;

            Client = new DiscordClient(clientId, (ulong)CreateFlags.NoRequireDiscord);
            ActivityManager = Client.GetActivityManager();
            UpdateActivity();
            
            Debug.Log("Discord integration started");
        }

        void StopIntegration()
        {
            if (Client == null)
                return;

            Client.Dispose();
            Client = null;
            ActivityManager = null;

            Debug.Log("Discord integration stopped");
        }

        void UpdateActivity()
        {
            if (ActivityManager == null)
                return;

            string fileName = null;
            var nameField = manager.CurrentVersion.GetNameField();

            if (nameField != null && manager.File.Entries.TryGetValue(nameField.id, out var content))
            {
                fileName = content.content;
            }

            var selectedId = string.IsNullOrWhiteSpace(hierarchy.SelectedId) ?
                    "None" :
                    hierarchy.SelectedId;

            var activity = new Activity()
            {
                Name = "SL Translation Magizmo",
                Instance = true,

                State = string.IsNullOrWhiteSpace(fileName) ?
                    "Editing a file" :
                    $"Editing {fileName}",
                
                Details = $"Id: {selectedId}",

                Assets = new ActivityAssets()
                {
                    SmallImage = smallImage,
                    LargeImage = largeImage,
                },
            };

            ActivityManager.UpdateActivity(activity, _ => { });
        }

        private void Update()
        {
            Client?.RunCallbacks();
        }
    }
}