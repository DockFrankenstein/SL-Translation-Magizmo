using System;
using System.Linq;

namespace Project.Translation.UI
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
            this.displayText = displayName;
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

            displayText = string.Join(" ", id
                .Split('_')
                .Where(x => x.Length > 0)
                .Select(x => $"{x[0].ToString().ToUpper()}{x.Substring(1, x.Length - 1)}"));
        }

        public ItemType type = ItemType.Normal;
        public string id = "";
        public string displayText = "";
    }
}