using UnityEngine;

namespace Project.GUI.Hierarchy
{
    public sealed class HierarchyNormalProvider : HierarchyItemProvider
    {
        [SerializeField] HierarchyItem[] items;

        public override HierarchyItem[] GetItems() =>
            items;
    }
}