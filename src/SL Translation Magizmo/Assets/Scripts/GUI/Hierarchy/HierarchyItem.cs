using System;

namespace Project.GUI.Hierarchy
{
    [Serializable]
    public class HierarchyItem : IApplicationObject
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

        public HierarchyItem(HierarchyItem item)
        {
            type = item.type;
            id = item.id;
            displayText = item.displayText;
            Item = item.Item;
            IsExpanded = item.IsExpanded;
        }

        public ItemType type = ItemType.Normal;
        public string id = "";
        public string displayText = "";

        [GUID] public string guid = Guid.NewGuid().ToString();

        public object Item { get; set; }
        public bool IsExpanded { get; set; } = true;

        string IApplicationObject.Name => displayText;

        public override string ToString() =>
            $"[{type}] {id}: \"{displayText}\", guid:{guid}";
    }
}