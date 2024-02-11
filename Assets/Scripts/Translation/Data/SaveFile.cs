using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Project.Translation.Mapping;

namespace Project.Translation.Data
{
    [Serializable]
    public class SaveFile : ISerializationCallbackReceiver  
    {
        public const string FILE_EXTENSION = "sltmf";

        [SerializeField] List<EntryData> _entries = new List<EntryData>();
        [SerializeField] object manifest;

        public Dictionary<string, EntryData> Entries = new Dictionary<string, EntryData>();

        public void OnBeforeSerialize()
        {
            _entries = Entries
                .Select(x => x.Value)
                .ToList();
        }

        public void OnAfterDeserialize()
        {
            //TODO: make sure there aren't any duplicates

            Entries = _entries
                .ToDictionary(x => x.entryId);
        }

        public static SaveFile Create(TranslationVersion translation)
        {
            var file = new SaveFile();

            if (translation != null)
            {
                var defines = translation.GetMappedFields();

                foreach (var item in defines)
                    if (!file.Entries.ContainsKey(item.id))
                        file.Entries.Add(item.id, new EntryData(item));
            }

            return file;
        }

        [Serializable]
        public class EntryData
        {
            public EntryData(string entryId)
            {
                this.entryId = entryId;
            }

            public EntryData(MappedField defineField) : this(defineField.id) { }

            public EntryData(string entryId, string content) : this(entryId)
            {
                this.content = content;
            }

            public EntryData(MappedField defineField, string content) : this(defineField.id, content) { }

            public string entryId;
            public string content = string.Empty;
        }
    }
}