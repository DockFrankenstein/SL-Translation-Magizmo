using qASIC.EditorTools.AssetEditor;
using Project.Translation.Defines;
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools;

namespace Project.Editor.Translation.Defines
{
    public class MultiEntryWindowToolbar : AssetEditorToolbar<MultiEntryWindow, MultiEntryTranslationDefines>
    {
        public MultiEntryWindowToolbar(MultiEntryWindow window) : base(window)
        {
        }

        protected override void OnLeftGUI()
        {
            if (GUILayout.Button(qGUIEditorUtility.PlusIcon, EditorStyles.toolbarButton))
            {
                window.tree.AddLine();
            }
        }

        protected override void OnRightGUI()
        {
            GUIAutoSaveButton();
            GUISaveButton();
        }
    }
}