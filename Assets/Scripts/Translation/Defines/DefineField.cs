﻿using System;
using UnityEngine;

namespace Project.Translation.Defines
{
    [Serializable]
    public class DefineField
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

        /// <summary>If disabled, application will ignore this definition and will only use it for exporting to SL's format.</summary>
        public bool addToList = true;

        [GUID] public string guid;

        [NonSerialized] public DefinesBase definesBase;

        /// <summary>How the definition will be used by the program.</summary>
        public SetupStatus Status
        {
            get
            {
                if (string.IsNullOrWhiteSpace(id))
                    return SetupStatus.Blank;

                if (!addToList)
                    return SetupStatus.Ignored;

                return SetupStatus.Used;
            }
        }

        public DefineField Duplicate()
        {
            return new DefineField()
            {
                id = id,
                autoDisplayName = autoDisplayName,
                displayName = displayName,
                addToList = addToList,
            };
        }
    }
}