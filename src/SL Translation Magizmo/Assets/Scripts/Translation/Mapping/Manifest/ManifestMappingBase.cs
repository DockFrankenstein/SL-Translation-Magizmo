﻿using Project.Translation.Data;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Project.Translation.Mapping.Manifest
{
    public abstract class ManifestMappingBase : MappingBase
    {
        public override bool Hide => true;

        public virtual Type DataType { get; }

        [NonSerialized] private MappedField[] _fieldsCache = null;
        public override MappedField[] GetMappedFields()
        {
            if (_fieldsCache == null)
            {
                _fieldsCache = GetFieldAttributes()
                    .Select(x => x.GetMappedField())
                    .ToArray();
            }

            return _fieldsCache;
        }

        public IEnumerable<MappedFieldNameAttribute> GetFieldAttributes() =>
            DataType.GetFields()
                .SelectMany(x => (MappedFieldNameAttribute[])x.GetCustomAttributes(typeof(MappedFieldNameAttribute), false));

        public override void Import(SaveFile file, string txt)
        {
            var manifestFile = JsonUtility.FromJson(txt, DataType);
            var fields = DataType.GetFields();

            foreach (var field in fields)
            {
                if (!GetAttribute(field, out var attr))
                    continue;

                if (!file.Entries.ContainsKey(attr.Id))
                    file.Entries.Add(attr.Id, new SaveFile.EntryData(attr.Id));

                object value = field.GetValue(manifestFile);

                //convert arrays
                if (!(value is string) &&
                    value is IEnumerable enumerable)
                {
                    List<string> enumItems = new List<string>();

                    foreach (object enumItem in enumerable)
                        enumItems.Add(enumItem?.ToString() ?? string.Empty);

                    value = enumItems.ToEntryContent();
                }

                value = value?.ToString() ?? string.Empty;
                file.Entries[attr.Id].content = value.ToString();
                ProjectDebug.LogValueImport(attr.Id, value);
            }
        }

        public override string Export(Func<int, MappedField, string> getTextContent)
        {
            var holder = Activator.CreateInstance(DataType);

            var fields = DataType.GetFields();

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                if (!GetAttribute(field, out var attr))
                    continue;

                var mappedField = new MappedField(attr.Id, this);
                string entry = getTextContent(i, mappedField);
                object value = entry;

                //convert arrays
                if (field.FieldType.IsArray)
                    value = entry.EntryContentToArray();

                value = Convert.ChangeType(value, field.FieldType);
                field.SetValue(holder, value);
            }

            return JsonUtility.ToJson(holder, true);
        }

        bool GetAttribute(FieldInfo field, out MappedFieldNameAttribute attr)
        {
            attr = ((MappedFieldNameAttribute[])field.GetCustomAttributes(typeof(MappedFieldNameAttribute), false))
                .FirstOrDefault();

            return attr != null;
        }

        public override void UpdateFileToNextVersion(SaveFile file, int fileVersion)
        {
            switch (fileVersion)
            {
                case 0:
                    foreach (var field in DataType.GetFields())
                    {
                        if (!GetAttribute(field, out var attr)) continue;
                        if (!field.FieldType.IsArray) continue;
                        if (!file.Entries.ContainsKey(attr.Id)) continue;
                        file.Entries[attr.Id].content += "\n";
                    }

                    break;
            }
        }
    }

    /// <typeparam name="T">Data to serialize</typeparam>
    public abstract class ManifestMappingBase<T> : ManifestMappingBase where T : new()
    {
        public override Type DataType => typeof(T);
    }
}
