using System;
using System.Linq;

namespace Project.GUI.Hierarchy
{
    [Serializable]
    public class HierarchyItem
    {
        public enum ItemType
        {
            Normal,
            Separator,
            Header,
        }

        public HierarchyItem(string id, string displayName)
        {
            this.id = id;
            displayText = displayName;
        }

        public HierarchyItem(string id)
        {
            if (id == "---")
            {
                type = ItemType.Separator;
                return;
            }

            if (id.StartsWith('#'))
            {
                type = ItemType.Header;
                displayText = id.TrimStart('#');
                return;
            }

            this.id = id;

            displayText = PUtility.GenerateDisplayName(id);
        }

        public ItemType type = ItemType.Normal;
        public string id = "";
        public string displayText = "";
    }
}