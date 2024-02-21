using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using UnityEditor.AssetImporters;

using TargetFile = Project.Translation.Mapping.MultiEntryTranslationMapping;

namespace Project.Editor.Translation.Defines
{
    [ScriptedImporter(VERSION, TargetFile.EXTENSION)]
    internal class MultiEntryTranslationDefinesImporter : ScriptedImporter
    {
        private const int VERSION = 0;
        private const string DEFAULT_ASSET_CONTENT = "";

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
            }
            catch (Exception e)
            {
                ctx.LogImportError($"Could not parse prompt library in JSON format from '{ctx.assetPath}' ({e})");
                DestroyImmediate(asset);
                return;
            }

            asset.name = Path.GetFileNameWithoutExtension(assetPath);
            ctx.AddObjectToAsset("<root>", asset);
        }

        [MenuItem("Assets/Create/Scriptable Objects/Translation/Mapping/Multi Entry")]
        public static void CreateInputAsset()
        {
            ProjectWindowUtil.CreateAssetWithContent($"New Multi Entry Translation Defines.{TargetFile.EXTENSION}",
                DEFAULT_ASSET_CONTENT);
        }
    }
}