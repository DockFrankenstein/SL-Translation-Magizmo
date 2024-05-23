using System.Collections.Generic;
using UnityEngine;

namespace Project.GUI.Hierarchy
{
    public abstract class HierarchySearchProvider : MonoBehaviour
    {
        public abstract string[] Names { get; }

        public abstract string GetSearchString(HierarchyItem item);
    }
}