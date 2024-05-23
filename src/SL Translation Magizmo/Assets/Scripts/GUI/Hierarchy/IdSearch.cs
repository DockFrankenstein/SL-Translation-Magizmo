namespace Project.GUI.Hierarchy
{
    internal class IdSearch : HierarchySearchProvider
    {
        public override string[] Names => new string[] { "id", "i" };

        public override string GetSearchString(HierarchyItem item) =>
            item.id;
    }
}