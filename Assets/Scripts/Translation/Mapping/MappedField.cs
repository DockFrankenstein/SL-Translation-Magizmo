using System;
using UnityEngine;

namespace Project.Translation.Mapping
{
    [Serializable]
    public class MappedField
    {
        public enum SetupStatus
        {
            /// <summary>Definition will be used by the application.</summary>
            Used,
            /// <summary>Definition will only be used for exporting translations to SL's format.</summary>
            Ignored,
            /// <summary>Definition won't be used and will leave a blank spot when exporting translations to SL's format.</summary>
            Blank,
        }

        public MappedField()
        {
            guid = Guid.NewGuid().ToString();
        }

        public MappedField(string id) : this()
        {
            this.id = id;
        }

        public MappedField(string id, MappingBase definesBase) : this(id)
        {
            this.mappingContainer = definesBase;
        }

        public string id = string.Empty;
        public bool autoDisplayName = true;
        public string displayName;

        /// <summary>If disabled, application will ignore this definition and will only use it for exporting to SL's format.</summary>
        public bool addToList = true;

        [GUID] public string guid;

        [NonSerialized] public MappingBase mappingContainer;

        /// <summary>How the definition will be used by the program.</summary>
        public SetupStatus Status
        {
            get
            {
                if (IsBlank)
                    return SetupStatus.Blank;

                if (!addToList)
                    return SetupStatus.Ignored;

                return SetupStatus.Used;
            }
        }

        public bool IsBlank =>
            string.IsNullOrWhiteSpace(id);

        public MappedField Duplicate()
        {
            return new MappedField()
            {
                id = id,
                autoDisplayName = autoDisplayName,
                displayName = displayName,
                addToList = addToList,
            };
        }
    }
}