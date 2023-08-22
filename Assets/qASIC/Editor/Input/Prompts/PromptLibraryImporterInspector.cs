using UnityEngine;
using UnityEditor;
using qASIC.EditorTools.Internal;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace qASIC.Input.Prompts.Internal
{
    [CustomEditor(typeof(PromptLibraryImporter))]
    internal class PromptLibraryImporterInspector : AssetImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var asset = assetTarget as PromptLibrary;

            using (new EditorGUI.DisabledScope(asset == null))
            {
                if (GUILayout.Button("Open Editor", qGUIInternalUtility.Styles.OpenButton))
                    PromptLibraryWindow.OpenAsset(asset);
            }

            ApplyRevertGUI();
        }
    }
}
