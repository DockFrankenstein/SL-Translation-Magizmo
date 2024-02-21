using qASIC.EditorTools.AssetEditor;
using qASIC.Input.Map.Internal;
using UnityEngine;

namespace qASIC.Input.Prompts.Internal
{
    internal class PromptLibraryWindowToolbar : AssetEditorToolbar<PromptLibraryWindow, PromptLibrary>
    {
        public PromptLibraryWindowToolbar(PromptLibraryWindow window) : base(window)
        {

        }

        Rect r_debug;

        protected override void OnLeftGUI()
        {
            if (InputMapWindow.DebugMode)
            {
                DisplayMenu("Debug", ref r_debug, menu =>
                {
                    menu.AddItem("Reload tree", false, window.variantTree.Reload);
                });
            }
        }

        protected override void OnRightGUI()
        {
            GUIAutoSaveButton();
            GUISaveButton();
        }
    }
}
