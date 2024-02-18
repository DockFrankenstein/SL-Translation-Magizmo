using qASIC.EditorTools.AssetEditor;
using Project.Translation.Mapping;
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools;
using qASIC;
using System.Linq;
using System.Collections.Generic;
using Project.Editor.Translation.Defines.Tools;

namespace Project.Editor.Translation.Defines
{
    public class MultiEntryWindowToolbar : AssetEditorToolbar<MultiEntryWindow, MultiEntryTranslationMapping>
    {
        public MultiEntryWindowToolbar(MultiEntryWindow window) : base(window)
        {
        }

        Rect _prefsRect;
        Rect _toolsRect;

        List<MultiEntryWindowTool> _tools = new List<MultiEntryWindowTool>()
        {
            new MEWT_AutoFill(),
            new MEWT_ReplaceText(),
        };

        protected override void OnLeftGUI()
        {
            if (GUILayout.Button(qGUIEditorUtility.PlusIcon, EditorStyles.toolbarButton))
            {
                window.tree.CreateLine();
            }

            using (new EditorGUI.DisabledGroupScope(!window.tree.CanDuplicateLine))
            {
                if (GUILayout.Button("Duplicate Line", EditorStyles.toolbarButton))
                {
                    window.tree.DuplicateSelectedLine();
                }
            }

            DisplayMenu("Tools", ref _toolsRect, (menu) =>
            {
                foreach (var tool in _tools)
                {
                    menu.AddItem(tool.Name, false, () =>
                    {
                        window.inspector.SelectedItem = tool;
                    });
                }
            });

            DisplayMenu("Preferences", ref _prefsRect, (menu) =>
            {
                menu.AddItem("Start Line Count From 1", window.Prefs_StartLineCountFromOne, () => window.Prefs_StartLineCountFromOne = !window.Prefs_StartLineCountFromOne);
                menu.AddItem("Use Single Line Mode When Avaliable", window.Prefs_UseSingleLine, () => window.Prefs_UseSingleLine = !window.Prefs_UseSingleLine);
            });
        }

        protected override void OnRightGUI()
        {
            GUIAutoSaveButton();
            GUISaveButton();
        }
    }
}