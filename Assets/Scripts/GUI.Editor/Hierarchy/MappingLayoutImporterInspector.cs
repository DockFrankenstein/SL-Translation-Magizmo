using Project.GUI.Hierarchy;
using Project.Translation.Mapping;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Project.GUI.Editor.Hierarchy
{
    [CustomEditor(typeof(MappingLayoutImporter))]
    internal class MappingLayoutImporterInspector : AssetImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var file = assetTarget as MappingLayout;
            file.version = (TranslationVersion)EditorGUILayout.ObjectField("Version", file.version, typeof(TranslationVersion), false);

            GUILayout.Space(18f);

            if (GUILayout.Button("Open Editor", GUILayout.Height(32f)))
                MappingLayoutWindow.OpenAsset(file);

            ApplyRevertGUI();
        }

        public override bool HasModified()
        {
            var relativePath = AssetDatabase.GetAssetPath(assetTarget);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            var unmodified = qASIC.Files.FileManager.LoadFileWriter(path);
            var current = JsonUtility.ToJson(assetTarget, true);

            var unmodifiedAsset = CreateInstance<MappingLayout>();
            JsonUtility.FromJsonOverwrite(unmodified, unmodifiedAsset);
            unmodified = JsonUtility.ToJson(unmodifiedAsset, true);

            return unmodified != current;
        }

        protected override void Apply()
        {
            base.Apply();

            var relativePath = AssetDatabase.GetAssetPath(assetTarget);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            qASIC.Files.FileManager.SaveFileJSON(path, assetTarget, true);
            AssetDatabase.ImportAsset(relativePath);
        }
    }
}