using UnityEditor;
using UnityEngine;

namespace Project.Editor.Translation.Defines.Tools
{
    public class MEWT_AutoFill : MultiEntryWindowTool
    {
        public override string Name => "Auto Fill Names";

        bool _ignoreWithAuto;
        bool _ignoreFilled;

        GUIContent c_ignoreWithAuto = new GUIContent("Ignore With Auto Name", "Ignore entries that have autoName set to true.");
        GUIContent c_ignoreFilled = new GUIContent("Ignore With Filled Names", "Ignore entries that already have their names filled.");

        public override void Initialize()
        {
            _ignoreWithAuto = true;
            _ignoreFilled = true;
        }

        public override void OnGUI()
        {
            _ignoreWithAuto = EditorGUILayout.Toggle(c_ignoreWithAuto, _ignoreWithAuto);
            _ignoreFilled = EditorGUILayout.Toggle(c_ignoreFilled, _ignoreFilled);

            EditorGUILayout.Space();

            if (GUILayout.Button("Auto Fill Names", GUILayout.Height(36f)))
            {
                foreach (var line in Window.asset.lines)
                {
                    foreach (var define in line.fields)
                    {
                        if (define.Status == Project.Translation.Mapping.MappedField.SetupStatus.Blank) continue;

                        if (_ignoreWithAuto && define.autoDisplayName) continue;
                        if (_ignoreFilled && !string.IsNullOrWhiteSpace(define.displayName)) continue;

                        define.autoDisplayName = false;
                        define.displayName = PUtility.GenerateDisplayName(define.id);
                    }
                }

                Window.tree.Reload();
                Window.SetAssetDirty();
            }
        }
    }
}