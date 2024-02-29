using Project.Translation.Data;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Translation.Mapping
{
    [CreateAssetMenu(fileName = "New Translation Define", menuName = "Scriptable Objects/Translation/Mapping/Array Entry")]
    public class ArrayEntryTranslationMapping : MappingBase
    {
        [FormerlySerializedAs("defineField")] public MappedField field;

        [Space]
        [TextArea]
        public string prefix;

        public override string Export(Func<int, MappedField, string> getTextContent)
        {
            var txt = getTextContent(0, field);
            txt = $"{prefix}{txt.TrimEnd('\n')}";
            return txt;
        }

        public override MappedField[] GetMappedFields() => new MappedField[] { field };

        public override void Import(SaveFile file, string txt)
        {
            var txtLines = txt
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split('\n')
                .Where(x => !x.TrimStart().StartsWith("#"));

            if (!file.Entries.ContainsKey(field.id))
                file.Entries.Add(field.id, new SaveFile.EntryData(field));

            var content = txtLines.ToEntryContent();
            file.Entries[field.id].content = content;
        }
    }
}