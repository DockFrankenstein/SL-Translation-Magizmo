using Project.Translation.Data;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Project.Translation.Defines.Manifest
{
    /// <typeparam name="T">Data to serialize</typeparam>
    public abstract class ManifestDefinesBase<T> : DefinesBase
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

            foreach (var item in fields)
            {
                var attr = ((DefineNameAttribute[])item.GetCustomAttributes(typeof(DefineNameAttribute), false))
                    .FirstOrDefault();

                if (attr == null)
                    continue;

                if (!file.Entries.ContainsKey(attr.Name))
                    file.Entries.Add(attr.Name, new AppFile.EntryData(attr.Name));

                object value = item.GetValue(manifestFile);

                //convert arrays
                if (!(value is string) &&
                    value is IEnumerable enumerable)
                {
                    List<object> enumItems = new List<object>();

                    foreach (object enumItem in enumerable)
                        enumItems.Add(enumItem);

                    value = string.Join("\n", enumItems);
                }

                value = value?.ToString() ?? string.Empty;
                file.Entries[attr.Name].content = value.ToString();
                ProjectDebug.LogValueImport(attr.Name, value);
            }
        }

        public override string Export(AppFile file)
        {
            throw new System.NotImplementedException();
        }
    }
}
