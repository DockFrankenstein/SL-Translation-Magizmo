using UnityEngine;

namespace Project.Translation.UI
{
    public abstract class HierarchyItemProvider : MonoBehaviour
    {
        public abstract HierarchyItem[] GetItems();
    }
}