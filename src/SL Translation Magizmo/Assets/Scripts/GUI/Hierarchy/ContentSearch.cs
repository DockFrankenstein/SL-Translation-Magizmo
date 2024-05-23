using Project.Translation;
using UnityEngine;

namespace Project.GUI.Hierarchy
{
    public class ContentSearch : HierarchySearchProvider
    {
        [SerializeField] TranslationManager manager;

        public override string[] Names => new string[] { "content", "c" };

        public override string GetSearchString(HierarchyItem item) =>
            manager.File.Entries.TryGetValue(item.id, out var val) ? val.content : string.Empty;
    }
}