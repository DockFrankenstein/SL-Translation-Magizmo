using System.Linq;
using UnityEngine;
using qASIC;
using Project.Translation;

namespace Project.GUI.Hierarchy
{
    public sealed class HierarchyEntryProvider : HierarchyItemProvider
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] bool addSeparators;

        public override HierarchyItem[] GetItems() =>
            manager.CurrentVersion.containers
                .Where(x => !x.Hide)
                .Select(x =>
                {
                    var a = x.GetMappedFields().Select(x => new HierarchyItem(x.id, x.autoDisplayName ? PUtility.GenerateDisplayName(x.id) : x.displayName));

                    if (addSeparators)
                        a = a.Append(new HierarchyItem("---"));

                    return a;
                })
                .SelectMany(x => x)
                .ToArray();

        private void Reset()
        {
            manager = FindObjectOfType<TranslationManager>();
        }
    }
}
