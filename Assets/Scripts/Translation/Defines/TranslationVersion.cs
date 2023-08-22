using UnityEngine;

namespace Project.Translation.Defines
{
    [CreateAssetMenu(fileName = "New Project Version", menuName = "Scriptable Objects/Translation/Version")]
    public class TranslationVersion : ScriptableObject
    {
        public string version;
        public TranslationDefines[] defines;
    }
}