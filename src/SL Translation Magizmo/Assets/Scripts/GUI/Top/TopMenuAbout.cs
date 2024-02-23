using Project.GUI.Other;
using UnityEngine;

namespace Project.GUI.Top
{
    public class TopMenuAbout : TopMenu
    {
        [SerializeField] AboutWindow aboutWindow;

        protected override string ButtonName => "about";

        protected override void CreateMenu()
        {
            menu.AppendAction("About", _ =>
            {
                aboutWindow.Open();
            });

            menu.AppendSeparator("");

            menu.AppendAction("SCPSL on steam", _ =>
            {
                Application.OpenURL("https://store.steampowered.com/app/700330/SCP_Secret_Laboratory/");
            });

            menu.AppendSeparator("");

            menu.AppendAction("Report a bug", _ =>
            {
                Application.OpenURL("https://github.com/DockFrankenstein/SL-Translation-Magizmo/issues");
            });

            menu.AppendAction("Github", _ =>
            {
                Application.OpenURL("https://github.com/DockFrankenstein/SL-Translation-Magizmo");
            });
        }
    }
}