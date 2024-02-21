using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using Project.Translation.Mapping;
using qASIC;

namespace Project.Editor.Translation.Defines
{
    [CustomEditor(typeof(MultiEntryTranslationDefinesImporter))]
    internal class MultiEntryTranslationDefinesImporterInspector : AssetImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var file = assetTarget as MultiEntryTranslationMapping;

            file.fileName = EditorGUILayout.TextField("File Name", file.fileName);
            file.identificationType = (MultiEntryTranslationMapping.IdentificationType)EditorGUILayout.EnumPopup("Identification Type", file.identificationType);

            using (new GUILayout.HorizontalScope(GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                GUILayout.FlexibleSpace();
            
            var separationCharRect = GUILayoutUtility.GetLastRect();

            separationCharRect = EditorGUI.PrefixLabel(separationCharRect, new GUIContent("Separation Character"));

            var useSeparationCharRect = separationCharRect
                .ResizeToLeft(EditorGUIUtility.singleLineHeight);

            separationCharRect = separationCharRect
                .BorderLeft(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            file.useSeparationCharacter = EditorGUI.Toggle(useSeparationCharRect, file.useSeparationCharacter);
            using (new EditorGUI.DisabledScope(!file.useSeparationCharacter))
                file.separationCharacter = EditorGUI.TextField(separationCharRect, file.separationCharacter.ToString())[0];

            GUILayout.Space(18f);

            if (GUILayout.Button("Open Editor", GUILayout.Height(32f)))
                MultiEntryWindow.OpenAsset(file);

            ApplyRevertGUI();
        }

        public override bool HasModified()
        {
            var relativePath = AssetDatabase.GetAssetPath(assetTarget);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            var unmodified = qASIC.Files.FileManager.LoadFileWriter(path);
            var current = JsonUtility.ToJson(assetTarget, true);

            var unmodifiedAsset = CreateInstance<MultiEntryTranslationMapping>();
            JsonUtility.FromJsonOverwrite(unmodified, unmodifiedAsset);
            unmodified = JsonUtility.ToJson(unmodifiedAsset, true);

            return unmodified != current;
        }

        protected override bool OnApplyRevertGUI()
        {
            return base.OnApplyRevertGUI();
        }

        protected override void Apply()
        {
            base.Apply();

            var relativePath = AssetDatabase.GetAssetPath(assetTarget);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            qASIC.Files.FileManager.SaveFileJSON(path, assetTarget, true);
            AssetDatabase.ImportAsset(relativePath);
        }

        protected override void ResetValues()
        {
            var relativePath = AssetDatabase.GetAssetPath(assetTarget);
            AssetDatabase.ImportAsset(relativePath);
            AssetDatabase.Refresh();
        }
    }
}