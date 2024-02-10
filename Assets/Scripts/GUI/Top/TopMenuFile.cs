namespace Project.GUI.Top
{
    public class TopMenuFile : TopMenu
    {
        protected override string ButtonName => "file";

        protected override void CreateMenu()
        {
            menu.AppendAction("Save", _ => manager.Save());
            menu.AppendAction("Load", _ => manager.Load());
            menu.AppendSeparator();
            menu.AppendAction("Import", _ => manager.Import());
            menu.AppendAction("Export", _ => manager.Export());
            menu.AppendAction("Test/ASD", _ => { });
        }
    }
}