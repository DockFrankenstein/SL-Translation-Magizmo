using Project.GUI.Settings;

namespace Project.GUI.Top
{
    public class TopMenuEdit : TopMenu
    {
        public PrefsWindowController prefsWindow;

        protected override string ButtonName => "edit";

        protected override void CreateMenu()
        {
            menu.AppendAction("Preferences", _ => prefsWindow.Visible = !prefsWindow.Visible);
        }
    }
}