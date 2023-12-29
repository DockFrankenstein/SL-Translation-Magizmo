using UnityEngine;
using System.Linq;
using Project.Translation.Data;

namespace Project.Translation.UI.Preview
{
    public class PreviewScene : MonoBehaviour
    {
#if UNITY_EDITOR
        [EditorButton(nameof(AutoDetectEntries), "Populate")]
#endif
        public PreviewEntry[] entries;

        /// <summary>Updates entries and other items.</summary>
        public void ReloadScene(AppFile appFile)
        {
            foreach (var entry in entries)
                if (appFile.Entries.TryGetValue(entry.entryID, out var content))
                    entry.text.text = content.content;
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