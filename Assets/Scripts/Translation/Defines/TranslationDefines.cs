using System.Collections.Generic;
using UnityEngine;

namespace Project.Translation.Defines
{
    [CreateAssetMenu(fileName = "New Translation Define", menuName = "Scriptable Objects/Translation/Defines")]
    public class TranslationDefines : ScriptableObject
    {
        public enum IdentificationType
        {
            LineId,
            FirstItem,
        }

        public string fileName;

        public IdentificationType identificationType;
        public bool useSeparationCharacter;
        public char separationCharacter;

        public List<Define> defines = new List<Define>();

        [System.Serializable]
        public struct Define
        {
            public string lineId;
            public string[] fieldIds;
        }
    }
}