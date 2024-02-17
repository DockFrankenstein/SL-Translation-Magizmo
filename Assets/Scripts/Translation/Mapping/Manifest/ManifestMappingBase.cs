using Project.Translation.Data;
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

        //Keep this static or else it won't be null
        private static MappedField[] _fieldsCache = null;
        public override MappedField[] GetMappedFields()
        {
            if (_fieldsCache == null)
            {
                _fieldsCache = GetFieldAttributes()
                    .Select(x => x.GetDefineField())
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
                var attr = ((MappedFieldNameAttribute[])field.GetCustomAttributes(typeof(MappedFieldNameAttribute), false))
                    .FirstOrDefault();

                if (attr == null)
                    continue;

                if (!file.Entries.ContainsKey(attr.Name))
                    file.Entries.Add(attr.Name, new SaveFile.EntryData(attr.Name));

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
                file.Entries[attr.Name].content = value.ToString();
                ProjectDebug.LogValueImport(attr.Name, value);
            }
        }

        public override string Export(Func<int, MappedField, string> getTextContent)
        {
            var holder = Activator.CreateInstance(DataType);

            var fields = DataType.GetFields();

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];

                var attr = ((MappedFieldNameAttribute[])field.GetCustomAttributes(typeof(MappedFieldNameAttribute), false))
                    .FirstOrDefault();

                if (attr == null)
                    continue;

                var mappedField = new MappedField(attr.Name, this);
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
    }

    /// <typeparam name="T">Data to serialize</typeparam>
    public abstract class ManifestMappingBase<T> : ManifestMappingBase where T : new()
    {
        public override Type DataType => typeof(T);
    }
}
