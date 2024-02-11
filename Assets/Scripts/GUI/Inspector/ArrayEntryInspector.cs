using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation.Mapping;
using Project.Utility.UI;

namespace Project.GUI.Inspector
{
    public class ArrayEntryInspector : InspectorDisplayPanel
    {
        [SerializeField] ReorderableListUI contentList;

        private void Awake()
        {
            contentList.OnChange.AddListener(ContentList_OnChange);
        }

        public override void Initialize()
        {
            if (manager.file.Entries.ContainsKey(id))
            {
                var entry = manager.file.Entries[id];
                contentList.ChangeValuesWithoutNotify(entry.content.EntryContentToArray().ToList());
            }
        }

        public override void Uninitialize()
        {
            contentList.ChangeValuesWithoutNotify(new List<string>());
        }

        public override bool ShouldOpen(string id)
        {
            var a = manager.CurrentVersion.MappedFields.TryGetValue(id, out var item) &&
            item.mappingContainer is ArrayEntryTranslationMapping;

            return a;
        }

        private void ContentList_OnChange()
        {
            if (manager.file.Entries.ContainsKey(id))
                manager.file.Entries[id].content = contentList.Values.ToEntryContent();

            inspector.RepaintPreview();
        }
    }
}