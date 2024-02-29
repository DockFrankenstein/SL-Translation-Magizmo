using Project.Translation.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Translation
{
    public class TranslationFileUpdater
    {
        public const int CURRENT_FILE_VERSION = 2;
        public const int LOWEST_SUPPORTED_FILE_VERSION = 0;

        public TranslationFileUpdater(TranslationManager manager)
        {
            Manager = manager;
        }

        public TranslationManager Manager { get; set; }

        public void EnsureFileIsUpToDate(SaveFile file, int fileVersion)
        {
            switch (fileVersion)
            {
                case 0:
                    Update0(file);
                    break;
                case 1:
                    Update1(file);
                    break;
            }
        }

        void LogUpdate(int prevVersion) =>
            Debug.Log($"Updated file from version {prevVersion - 1} to {prevVersion}");

        void Update0(SaveFile file)
        {
            string[] arrayIds = new string[]
            {
                "manifest_authors",
                "manifest_interface_locales",
                "manifest_system_locales",
                "manifest_forced_font_order",
                "hid_sign",
                "intercom_door_sign",
                "loading_hints",
                "nuke_sign",
            };

            foreach (var id in arrayIds)
                if (file.Entries.ContainsKey(id))
                    file.Entries[id].content += "\n";

            LogUpdate(0);
            Update1(file);
        }

        void Update1(SaveFile file)
        {
            HashSet<KeyValuePair<string, string>> ids = new HashSet<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("death_body_330", "death_screen_330"),
                new KeyValuePair<string, string>("death_body_tesla", "death_screen_tesla"),
                new KeyValuePair<string, string>("death_body_018", "death_body_blunt_trauma"),
                new KeyValuePair<string, string>("death_screen_ff", "death_body_ff")
            };

            foreach (var item in ids)
            {
                if (!file.Entries.ContainsKey(item.Key)) continue;
                if (file.Entries.ContainsKey(item.Value)) continue;

                var data = file.Entries[item.Key];
                data.entryId = item.Value;
                file.Entries.Remove(item.Key);
                file.Entries.Add(item.Value, data);
            }

            LogUpdate(1);
        }
    }
}