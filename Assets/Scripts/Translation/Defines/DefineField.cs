using System;
using UnityEngine;

namespace Project.Translation.Defines
{
    [Serializable]
    public class DefineField
    {
        public DefineField()
        {
            guid = Guid.NewGuid().ToString();
        }

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
        public string guid;

        [NonSerialized] public DefinesBase definesBase;
    }
}