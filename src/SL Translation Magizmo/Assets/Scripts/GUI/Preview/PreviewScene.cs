using UnityEngine;
using System.Linq;
using Project.Translation.Data;
using Project.Utility.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Project.GUI.Preview
{
    public class PreviewScene : MonoBehaviour
    {
        public string path;

#if UNITY_EDITOR
        [EditorButton(nameof(AutoDetectEntries), "Populate")]
#endif
        public PreviewEntry[] entries = new PreviewEntry[0];

        public Dictionary<string, PreviewEntry> EntriesForIds { get; private set; } = new Dictionary<string, PreviewEntry>();

        public void Initialize()
        {
            EntriesForIds = entries
                .Where(x => x != null)
                .SelectMany(x => x.GetListOfTargets().Select(y => new KeyValuePair<string, PreviewEntry>(y.entryId, x)))
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.First().Value);
        }

        /// <summary>Updates entries and other items.</summary>
        public void Reload()
        {
            foreach (var entry in entries)
                entry?.Reload();

            LayoutGroupController.Refresh();
        }

        private void OnEnable()
        {
            Reload();
        }

#if UNITY_EDITOR
        void AutoDetectEntries()
        {
            var detectedEntries = GetComponentsInChildren<PreviewEntry>()
                .Except(entries);

            if (detectedEntries.Count() == 0) return;

            entries = entries
                .Concat(detectedEntries)
                .ToArray();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}