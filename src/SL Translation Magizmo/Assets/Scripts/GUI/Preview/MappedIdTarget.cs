using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.GUI.Preview
{
    [Serializable]
    public struct MappedIdTarget
    {
        public MappedIdTarget(string entryId) : this(entryId, false) { }

        public MappedIdTarget(string entryId, bool isMain) : this()
        {
            this.entryId = entryId;
            this.isMain = isMain;
        }

        public string entryId;
        [FormerlySerializedAs("isDefault")] public bool isMain;
        [TextArea] public string defaultValue;

        public static implicit operator string(MappedIdTarget target) =>
            target.entryId;
    }
}