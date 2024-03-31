using System.IO;
using System;
using UnityEditor;
using UnityEditor.AssetImporters;

using TargetFile = Project.GUI.Hierarchy.MappingLayout;
using UnityEngine;
using Project.Translation.Mapping;

namespace Project.GUI.Editor.Hierarchy
{
    [ScriptedImporter(VERSION, TargetFile.EXTENSION)]
    internal class MappingLayoutImporter : ScriptedImporter
    {
        const int VERSION = 1;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            string text;
            try
            {
                text = File.ReadAllText(ctx.assetPath);
            }
            catch (Exception e)
            {
                ctx.LogImportError($"Could not read file `{ctx.assetPath}` ({e})");
                return;
            }

            var asset = ScriptableObject.CreateInstance<TargetFile>();

            try
            {
                JsonUtility.FromJsonOverwrite(text, asset);

                var versionPath = AssetDatabase.GUIDToAssetPath(asset.versionId);

                asset.version = AssetDatabase.LoadAssetAtPath<TranslationVersion>(versionPath);
            }
            catch (Exception e)
            {
                ctx.LogImportError($"Could not parse mapping layout in JSON format from '{ctx.assetPath}' ({e})");
                DestroyImmediate(asset);
                return;
            }

            asset.name = Path.GetFileNameWithoutExtension(assetPath);
            ctx.AddObjectToAsset("<root>", asset);
        }

        [MenuItem("Assets/Create/Scriptable Objects/Translation/Hierarchy/Mapping Layout")]
        public static void CreateInputAsset()
        {
            var file = ScriptableObject.CreateInstance<TargetFile>();

            ProjectWindowUtil.CreateAssetWithContent($"New Mapping Layout.{TargetFile.EXTENSION}",
                JsonUtility.ToJson(file, true));
        }
    }
}