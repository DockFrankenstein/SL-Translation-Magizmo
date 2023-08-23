using UnityEngine;
using System.Linq;

namespace Project.Translation.Defines
{
    [CreateAssetMenu(fileName = "New Project Version", menuName = "Scriptable Objects/Translation/Version", order = 20)]
    public class TranslationVersion : ScriptableObject
    {
        public string version;
        public TranslationDefinesBase[] defines;

        public string[] GetDefines() =>
            defines
            .SelectMany(x => x.GetDefines())
            .ToArray();
    }
}