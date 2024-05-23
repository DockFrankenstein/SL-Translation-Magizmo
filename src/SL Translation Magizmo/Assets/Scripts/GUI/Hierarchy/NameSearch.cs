namespace Project.GUI.Hierarchy
{
    public class NameSearch : HierarchySearchProvider
    {
        public override string[] Names => new string[] { "name" };

        public override string GetSearchString(HierarchyItem item) =>
            item.displayText;
    }
}