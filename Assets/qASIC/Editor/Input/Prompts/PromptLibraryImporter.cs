using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace qASIC.Input.Prompts.Internal
{
    [ScriptedImporter(VERSION, PromptLibrary.EXTENSION)]
    internal class PromptLibraryImporter : ScriptedImporter
    {
        private const int VERSION = 0;
        private const string DEFAULT_ASSET_CONTENT = "{}";

        [SerializeField] Texture2D icon;

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

            var asset = ScriptableObject.CreateInstance<PromptLibrary>();

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
            ctx.AddObjectToAsset("<root>", asset, icon);
        }

        [MenuItem("Assets/Create/qASIC/Input/Prompt Library")]
        public static void CreateInputAsset()
        {
            ProjectWindowUtil.CreateAssetWithContent($"New Prompt Library.{PromptLibrary.EXTENSION}",
                DEFAULT_ASSET_CONTENT);
        }
    }
}
