using Assets.Scripts.GUI;
using Project.GUI.Hierarchy;
using Project.GUI.Inspector;
using Project.GUI.Preview;
using qASIC.Options;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Top
{
    public class TopMenuView : TopMenu
    {
        [SerializeField] HierarchyController hierarchy;
        [SerializeField] PreviewManager preview;
        [SerializeField] InspectorDisplay inspector;
        [SerializeField] ComparisonManagerUI comparison;

        [Option("view_show_ids")]
        public static bool Sett_ShowIds { get; private set; } = false;

        protected override string ButtonName => "view";

        protected override void CreateMenu()
        {
            menu.AppendAction("Launch SCPSL", _ => Application.OpenURL("steam://rungameid/700330"));

            menu.AppendSeparator();

            menu.AppendAction("Expand All", (args) =>
            {
                hierarchy.ExpandAll();
            });

            menu.AppendAction("Collapse All", (args) =>
            {
                hierarchy.CollapseAll();
            });

            menu.AppendSeparator();

            menu.AppendAction("Previous Entry", _ => hierarchy.SelectBy(-1));
            menu.AppendAction("Next Entry", _ => hierarchy.SelectBy(1));
            menu.AppendAction("Previous Scene", _ => preview.SelectBy(-1));
            menu.AppendAction("Next Scene", _ => preview.SelectBy(1));

            menu.AppendSeparator();

            menu.AppendAction("Show Ids Instead Of Names", _ =>
            {
                manager.Options.SetOptionAndApply("view_show_ids", !Sett_ShowIds);
                hierarchy.Refresh();
                inspector.ReloadName();

            }, Sett_ShowIds ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            menu.AppendSeparator();

            menu.AppendAction("Comparison Manager", _ =>
            {
                comparison.Open();
            });
        }
    }
}