using UnityEngine;

namespace Project.GUI.Hierarchy
{
    public abstract class HierarchyItemProvider : MonoBehaviour
    {
        public HierarchyController Hierarchy { get; internal set; }

        public abstract HierarchyItem[] GetItems();
    }
}