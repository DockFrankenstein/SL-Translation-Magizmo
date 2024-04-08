using Project.Translation;
using UnityEngine;
using Discord;

using DiscordClient = Discord.Discord;
using Project.GUI.Hierarchy;

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

        public DiscordClient Client { get; private set; }
        public ActivityManager ActivityManager { get; private set; }

        private void Reset()
        {
            manager = FindObjectOfType<TranslationManager>();
            hierarchy = FindObjectOfType<HierarchyController>();
        }

        private void Awake()
        {
            StartIntegration();

            hierarchy.OnSelect += _ => UpdateActivity();
            manager.OnFileChanged += _ => UpdateActivity();
        }

        private void OnDestroy()
        {
            StopIntegration();
        }

        void StartIntegration()
        {
            Client = new DiscordClient(clientId, (ulong)CreateFlags.NoRequireDiscord);
            ActivityManager = Client.GetActivityManager();
            UpdateActivity();
        }

        void StopIntegration()
        {
            Client.Dispose();
            Client = null;
            ActivityManager = null;
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
            Client.RunCallbacks();
        }
    }
}