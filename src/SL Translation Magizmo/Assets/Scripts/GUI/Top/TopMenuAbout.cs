using Project.GUI.Other;
using UnityEngine;

namespace Project.GUI.Top
{
    public class TopMenuAbout : TopMenu
    {
        [SerializeField] AboutWindow aboutWindow;
        [SerializeField] AutoUpdateWindow update;

        protected override string ButtonName => "about";

        protected override void CreateMenu()
        {
            menu.AppendAction("About App", _ =>
            {
                aboutWindow.Open();
            });

            menu.AppendAction("About SCPSL", _ =>
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

            menu.AppendSeparator("");

            menu.AppendAction("Check For Updates", _ => update.Open());
        }
    }
}