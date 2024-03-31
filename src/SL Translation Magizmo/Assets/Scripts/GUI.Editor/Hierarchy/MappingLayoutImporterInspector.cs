using Project.GUI.Hierarchy;
using Project.Translation.Mapping;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Project.GUI.Editor.Hierarchy
{
    [CustomEditor(typeof(MappingLayoutImporter))]
    internal class MappingLayoutImporterInspector : AssetImporterEditor
    {
        MappingLayout file;

        public override void OnInspectorGUI()
        {
            if (file == null)
                file = assetTarget as MappingLayout;

            file.version = (TranslationVersion)EditorGUILayout.ObjectField("Version", file.version, typeof(TranslationVersion), false);
            file.versionId = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(file.version));

            GUILayout.Space(18f);

            if (GUILayout.Button("Open Editor", GUILayout.Height(32f)))
                MappingLayoutWindow.OpenAsset(file);

            ApplyRevertGUI();
        }

        public override bool HasModified()
        {
            var currentAsset = assetTarget as MappingLayout;

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