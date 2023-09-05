using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Project.Translation.Defines
{
    [CreateAssetMenu(fileName = "New Project Version", menuName = "Scriptable Objects/Translation/Version", order = 20)]
    public class TranslationVersion : ScriptableObject
    {
        public string version;
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
    }
}