using UnityEngine;

namespace Project.Translation.UI
{
    public sealed class HierarchyNormalProvider : HierarchyItemProvider
    {
        [SerializeField] HierarchyItem[] items;

        public override HierarchyItem[] GetItems() =>
            items;
    }
}