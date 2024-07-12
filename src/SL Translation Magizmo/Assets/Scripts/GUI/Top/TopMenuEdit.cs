using Project.GUI.Settings;
using Project.Undo;
using qASIC.Input.Prompts;
using UnityEngine.UIElements;

namespace Project.GUI.Top
{
    public class TopMenuEdit : TopMenu
    {
        public PrefsWindowController prefsWindow;
        public UndoManager undo;
        public PromptLibrary prompts;

        protected override string ButtonName => "edit";

        protected override void CreateMenu()
        {
            menu.AppendAction("Undo", 
                _ => undo.Undo(), 
                undo.CanUndo() ? DropdownMenuAction.AlwaysEnabled : DropdownMenuAction.AlwaysDisabled,
                CreateDataForInput(prompts, undo.i_undo));
            menu.AppendAction("Redo",
                _ => undo.Redo(),
                undo.CanRedo() ? DropdownMenuAction.AlwaysEnabled : DropdownMenuAction.AlwaysDisabled,
                CreateDataForInput(prompts, undo.i_redo));

            menu.AppendSeparator();
            menu.AppendAction("Preferences", _ => prefsWindow.Visible = !prefsWindow.Visible);
        }
    }
}