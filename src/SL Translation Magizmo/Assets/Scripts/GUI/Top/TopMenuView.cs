using Project.GUI.Hierarchy;
using UnityEngine;

namespace Project.GUI.Top
{
    public class TopMenuView : TopMenu
    {
        [SerializeField] HierarchyController hierarchy;

        protected override string ButtonName => "view";

        protected override void CreateMenu()
        {
            menu.AppendAction("Expand All", (args) =>
            {
                hierarchy.ExpandAll();
            });

            menu.AppendAction("Collapse All", (args) =>
            {
                hierarchy.CollapseAll();
            });
        }
    }
}