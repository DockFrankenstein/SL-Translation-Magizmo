using Project.Translation.Data;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Project.Translation.Defines.Manifest
{
    /// <typeparam name="T">Data to serialize</typeparam>
    public abstract class ManifestDefinesBase<T> : DefinesBase where T : new()
    {
        public override bool Hide => true;

        //Keep this static or else it won't be null
        private static string[] _definesCache = null;
        public override string[] GetDefines()
        {
            if (_definesCache == null)
            {
                _definesCache = typeof(T).GetFields()
                    .SelectMany(x => (DefineNameAttribute[])x.GetCustomAttributes(typeof(DefineNameAttribute), false))
                    .Select(x => x.Name)
                    .ToArray();
            }

            return _definesCache;
        }

        public override void Import(AppFile file, string txt)
        {
            var manifestFile = JsonUtility.FromJson<T>(txt);
            var fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                var attr = ((DefineNameAttribute[])field.GetCustomAttributes(typeof(DefineNameAttribute), false))
                    .FirstOrDefault();

                if (attr == null)
                    continue;

                if (!file.Entries.ContainsKey(attr.Name))
                    file.Entries.Add(attr.Name, new AppFile.EntryData(attr.Name));

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

        public override string Export(AppFile file)
        {
            T holder = new T();
            var fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                var attr = ((DefineNameAttribute[])field.GetCustomAttributes(typeof(DefineNameAttribute), false))
                    .FirstOrDefault();

                if (attr == null)
                    continue;

                //Continue if value does not exist
                if (!file.Entries.ContainsKey(attr.Name))
                    continue;

                string entry = file.Entries[attr.Name].content;
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
}
