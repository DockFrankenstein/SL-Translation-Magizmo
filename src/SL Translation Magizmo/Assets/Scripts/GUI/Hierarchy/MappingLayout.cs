using Project.Translation.Mapping;
using System.Collections.Generic;
using UnityEngine;

namespace Project.GUI.Hierarchy
{
    public class MappingLayout : ScriptableObject
    {
        public const string EXTENSION = "tml";

        public TranslationVersion version;
        public List<HierarchyItem> items = new List<HierarchyItem>();
    }
}