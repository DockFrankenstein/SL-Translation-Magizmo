using Project.Translation.Mapping;
using System.Linq;
using UnityEngine;
using qASIC;

namespace Project.GUI.Preview
{
    public class FillPreviewEntryWithMappedContainer : MonoBehaviour
    {
        enum SortType
        {
            None,
            Name,
        }

        [SerializeField] PreviewEntry entry;

        [Label("Settings")]
        [SerializeField] [Tooltip("Skips entries by specified amount. E.g. 1 would result in array of 0, 2, 4, 6, ...")] int skipEvery = 0;
        [SerializeField] [Tooltip("Index at which it will start. E.g. 3 would result in array of 3, 4, 5")] int startAtIndex = 0;
        [Space]
        [SerializeField] SortType sortType = SortType.None;
        [SerializeField] bool removeDuplicates = true;
        [SerializeField] bool removeNotAddedToList = true;
        [SerializeField] bool removeBlank = true;

        [Label("Containers")]
        [EditorButton(nameof(Fill))]
        [SerializeField] MappingBase[] containers;

        private void Reset()
        {
            entry = GetComponent<PreviewEntry>();
        }

        public void Fill()
        {
            if (entry == null) return;

            var entries = containers
                .SelectMany(x => x.GetMappedFields())
                .Skip(startAtIndex)
                .Where((x, i) => i % (skipEvery + 1) == 0)
                .Where(x => !removeNotAddedToList || x.addToList)
                .Where(x => !removeBlank || !x.IsBlank)
                .Select(x => x.id);

            switch (sortType)
            {
                case SortType.Name:
                    entries = entries.OrderBy(x => x);
                    break;
            }

            if (removeDuplicates)
                entries = entries.Distinct();

            entry.mainId = new MappedIdTarget();
            entry.otherIds = new MappedIdTarget[0];

            if (entries.Count() > 0)
            {
                entry.mainId = new MappedIdTarget(entries.First(), true);
                entry.otherIds = entries
                    .Skip(1)
                    .Select(x => new MappedIdTarget(x, true))
                    .ToArray();
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(entry);
            }
#endif
        }
    }
}