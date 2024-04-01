using UnityEngine;
using System.Linq;
using Project.Translation.Data;
using Project.Utility.UI;
using System.Collections.Generic;
using Project.GUI.Preview.Interfaces;

namespace Project.GUI.Preview
{
    public class PreviewScene : MonoBehaviour
    {
        public string path;

#if UNITY_EDITOR
        [EditorButton(nameof(AutoDetectEntries), "Populate")]
#endif
        public PreviewElement[] entries = new PreviewElement[0];

        public Dictionary<string, IHasMappedTargets> EntriesForIds { get; private set; } = new Dictionary<string, IHasMappedTargets>();

        public void Initialize()
        {
            EntriesForIds = entries
                .Where(x => x is IHasMappedTargets)
                .Select(x => x as IHasMappedTargets)
                .SelectMany(x => x.GetListOfTargets().Select(y => new KeyValuePair<string, IHasMappedTargets>(y.entryId, x)))
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
            var detectedEntries = GetComponentsInChildren<PreviewElement>()
                .Except(entries);

            if (detectedEntries.Count() == 0) return;

            entries = entries
                .Where(x => x != null)
                .Concat(detectedEntries)
                .ToArray();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}