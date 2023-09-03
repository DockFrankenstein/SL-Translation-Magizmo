using System;
using UnityEngine;

namespace Project.Translation.Defines
{
    [Serializable]
    public class DefineField
    {
        public DefineField() { }

        public DefineField(string id) : this()
        {
            this.id = id;
        }

        public DefineField(string id, DefinesBase definesBase) : this(id)
        {
            this.definesBase = definesBase;
        }

        public string id = string.Empty;
        public bool autoDisplayName = true;
        public string displayName;

        [NonSerialized] public DefinesBase definesBase;
    }
}