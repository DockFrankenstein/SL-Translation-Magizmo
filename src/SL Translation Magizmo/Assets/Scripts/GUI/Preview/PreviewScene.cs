using UnityEngine;
using System.Linq;
using Project.Translation.Data;
using Project.Utility.UI;

namespace Project.GUI.Preview
{
    public class PreviewScene : MonoBehaviour
    {
        public string path;

#if UNITY_EDITOR
        [EditorButton(nameof(AutoDetectEntries), "Populate")]
#endif
        public PreviewEntry[] entries = new PreviewEntry[0];

        /// <summary>Updates entries and other items.</summary>
        public void UpdateScene(SaveFile appFile)
        {
            foreach (var entry in entries)
                entry.UpdateContent(appFile);

            LayoutGroupController.Refresh();
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