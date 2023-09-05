using UnityEngine;
using System.Linq;

namespace Project.Translation.UI.Preview
{
    public class PreviewScene : MonoBehaviour
    {
        [EditorButton(nameof(AutoDetectEntries), "Populate")]
        public PreviewEntry[] entries;

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