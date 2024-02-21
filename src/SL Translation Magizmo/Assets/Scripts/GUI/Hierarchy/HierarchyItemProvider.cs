using UnityEngine;

namespace Project.GUI.Hierarchy
{
    public abstract class HierarchyItemProvider : MonoBehaviour
    {
        public abstract HierarchyItem[] GetItems();
    }
}