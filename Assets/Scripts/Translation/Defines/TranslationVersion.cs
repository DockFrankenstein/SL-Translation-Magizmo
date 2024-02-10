using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Project.Translation.Data;
using SFB;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;

namespace Project.Translation.Defines
{
    [CreateAssetMenu(fileName = "New Project Version", menuName = "Scriptable Objects/Translation/Version", order = 20)]
    public class TranslationVersion : ScriptableObject
    {
        public string version;
        [EditorButton(nameof(ExportIdTestTranslation))]
        public DefinesBase[] defines;

        public DefineField[] GetDefines() =>
            defines
            .SelectMany(x => x.GetDefines())
            .ToArray();

        private Dictionary<string, DefineField> _definesDictionary = null;
        public Dictionary<string, DefineField> DefinesDictionary
        {
            get
            {
                if (_definesDictionary == null)
                    _definesDictionary = defines
                        .SelectMany(x => x.GetDefines())
                        .GroupBy(x => x.id)
                        .Select(x => x.First())
                        .ToDictionary(x => x.id);

                return _definesDictionary;
            }
        }

        public void Initialize()
        {
            foreach (var defineFile in defines)
            {
                var defines = defineFile.GetDefines();

                for (int i = 0; i < defines.Length; i++)
                    defines[i].definesBase = defineFile;
            }

            _ = DefinesDictionary;
        }

        public void ExportIdTestTranslation()
        {
            //var file = new SaveFile();
            //var entries = defines
            //    .SelectMany(x =>
            //    {
            //        var a = x.GetDefines();
            //        object[][] b = new object[a.Length][];
            //        for (int i = 0; i < a.Length; i++)
            //        {
            //            b[i] = new object[] { i, a[i] };
            //        }

            //        return b;
            //    })
            //    .GroupBy(x => (x[1] as DefineField).id)
            //    .Select(x => x.First())
            //    .ToDictionary(x => (x[1] as DefineField).id, x => new SaveFile.EntryData((x[1] as DefineField).id, $"{(int)x[0]}:{(x[1] as DefineField).id}"));

            //file.Entries = entries;

#if UNITY_EDITOR
            var path = EditorUtility.OpenFolderPanel("Export Translation", string.Empty, string.Empty);
#else
            var paths = StandaloneFileBrowser.OpenFolderPanel("Export Translation", string.Empty, false);
            var path = paths.Length == 1 ? paths[0] : string.Empty;
#endif

            if (!string.IsNullOrWhiteSpace(path))
            {
                foreach (var defineFile in defines)
                {
                    var txt = defineFile.ExportDebug();
                    System.IO.File.WriteAllText($"{path.Replace('\\', '/')}/{defineFile.fileName}", txt);
                }
            }
        }
    }
}