using qASIC.EditorTools.AssetEditor;
using Project.Translation.Defines;
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools;
using qASIC;
using System.Linq;

namespace Project.Editor.Translation.Defines
{
    public class MultiEntryWindowToolbar : AssetEditorToolbar<MultiEntryWindow, MultiEntryTranslationDefines>
    {
        public MultiEntryWindowToolbar(MultiEntryWindow window) : base(window)
        {
        }

        Rect _prefsRect;
        Rect _toolsRect;

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
                menu.AddItem("Auto Fill names", false, () => 
                {
                    foreach (var line in window.asset.lines)
                    {
                        foreach (var define in line.defines)
                        {
                            if (!string.IsNullOrWhiteSpace(define.displayName)) continue;
                            define.autoDisplayName = false;
                            define.displayName = PUtility.GenerateDisplayName(define.id);
                        }
                    }

                    window.tree.Reload();
                    window.SetAssetDirty();
                });
            });

            DisplayMenu("Preferences", ref _prefsRect, (menu) =>
            {
                menu.AddItem("Start Line Count From 1", window.Prefs_StartLineCountFromOne, () => window.Prefs_StartLineCountFromOne = !window.Prefs_StartLineCountFromOne);
            });
        }

        protected override void OnRightGUI()
        {
            GUIAutoSaveButton();
            GUISaveButton();
        }
    }
}