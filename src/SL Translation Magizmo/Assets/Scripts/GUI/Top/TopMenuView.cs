using Project.GUI.Hierarchy;
using Project.Translation.Data;
using UnityEngine;
using UnityEngine.UIElements;

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

            menu.AppendSeparator();

            foreach (var item in manager.ComparisonManager.translations)
            {
                var status = manager.ComparisonManager.CurrentTranslation == item.Value ?
                    DropdownMenuAction.Status.Checked :
                    DropdownMenuAction.Status.Normal;

                menu.AppendAction($"Translation Comparison/{item.Value.displayName}", 
                    _ => manager.ComparisonManager.ChangeCurrent(item.Key),
                    status);
            }

            menu.AppendSeparator();

            menu.AppendAction("Launch SCPSL", _ => Application.OpenURL("steam://rungameid/700330"));
        }
    }
}