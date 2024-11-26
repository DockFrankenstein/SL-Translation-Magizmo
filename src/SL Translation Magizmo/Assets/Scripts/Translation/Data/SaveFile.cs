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
        public SaveFile()
        {

        }

        public SaveFile(TranslationVersion version)
        {
            CleanupToVersion(version);
        }

        /// <summary>Creates a copy of another <see cref="SaveFile"/>,</summary>
        /// <param name="other"><see cref="SaveFile"/> to copy.</param>
        public SaveFile(SaveFile other)
        {
            UseNewestSlVersion = other.UseNewestSlVersion;
            SlVersion = other.SlVersion;
            Entries = other.Entries
                .Select(x => new KeyValuePair<string, EntryData>(x.Key, new EntryData(x.Value)))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public const string FILE_EXTENSION = "sltmf";

        #region Serialization
        [SerializeField] bool useNewestSlVersion = true;
        [SerializeField] string slVersion;
        [SerializeField] List<EntryData> _entries = new List<EntryData>();

        public void OnBeforeSerialize()
        {
            _entries = Entries
                .Select(x => x.Value)
                .ToList();
        }

        public void OnAfterDeserialize()
        {
            Entries = _entries
                .GroupBy(x => x.entryId)
                .ToDictionary(x => x.Key, x => x.First());
        }
        #endregion

        #region Controls
        public Dictionary<string, EntryData> Entries = new Dictionary<string, EntryData>();

        public bool UseNewestSlVersion
        {
            get => useNewestSlVersion;
            set => useNewestSlVersion = value;
        }

        public Version SlVersion
        {
            get => Version.TryParse(slVersion, out var ver) ? 
                ver : 
                new Version();
            set => slVersion = value.ToString();
        }
        #endregion

        public void UpgradeToVersion(TranslationVersion prev, TranslationVersion current)
        {
            if (prev == null || current == null)
                return;

            foreach (var field in current.MappedFields)
            {
                if (!prev.MappedFields.TryGetValue(field.Key, out var prevField))
                    continue;

                if (!Entries.ContainsKey(field.Key))
                    continue;

                var length = Mathf.Min(field.Value.dynamicValues.Count, prevField.dynamicValues.Count);
                for (int i = 0; i < length; i++)
                    Entries[field.Key].content = Entries[field.Key].content.Replace(prevField.dynamicValues[i].tag, field.Value.dynamicValues[i].tag);
            }
        }

        /// <summary>Removes unused empty entries that aren't included in the version and adds missing ones.</summary>
        public void CleanupToVersion(TranslationVersion version)
        {
            var entries = Entries
                .Select(x => x.Value)
                .Where(x => !string.IsNullOrEmpty(x.content));

            var ids = entries.Select(x => x.entryId);

            IEnumerable<EntryData> verEntries = new List<EntryData>();

            if (version != null)
                verEntries = version.MappedFields
                    .Select(x => x.Key)
                    .Except(ids)
                    .Select(x => new EntryData(x));

            var prevCount = Entries.Count;

            Entries = entries
                .Concat(verEntries)
                .ToDictionary(x => x.entryId);

            Debug.Log($"Cleanup completed, removed {prevCount - entries.Count()}, added {verEntries.Count()}");
        }

        [Serializable]
        public class EntryData : IApplicationObject
        {
            public EntryData(string entryId)
            {
                this.entryId = entryId;
            }

            public EntryData(MappedField field) : this(field.id) { }

            public EntryData(string entryId, string content) : this(entryId)
            {
                this.content = content;
            }

            public EntryData(MappedField field, string content) : this(field.id, content) { }

            public EntryData(EntryData other)
            {
                entryId = other.entryId;
                content = other.content;
            }

            string IApplicationObject.Name => entryId;

            public string entryId;
            public string content = string.Empty;

            public override string ToString() =>
                $"{entryId} data: {content}";
        }
    }
}