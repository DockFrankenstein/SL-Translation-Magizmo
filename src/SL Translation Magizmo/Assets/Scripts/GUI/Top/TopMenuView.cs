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
            menu.AppendAction("Expand All In Hierarchy", (args) =>
            {
                hierarchy.ExpandAll();
            });

            menu.AppendAction("Collapse All In Hierarchy", (args) =>
            {
                hierarchy.CollapseAll();
            });
        }
    }
}