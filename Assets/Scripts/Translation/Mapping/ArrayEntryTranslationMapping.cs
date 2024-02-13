using Project.Translation.Data;
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

        public override string Export(SaveFile file)
        {
            var txt = string.Empty;
            
            if (file.Entries.ContainsKey(field.id))
                txt = file.Entries[field.id].content
                    .Replace("\n", "\r\n");

            txt = $"{prefix}\n{txt}";

            return txt;
        }

        public override string ExportDebug()
        {
            return $"{prefix}\n{field.id}";
        }

        public override MappedField[] GetMappedFields() => new MappedField[] { field };

        public override void Import(SaveFile file, string txt)
        {
            var txtLines = txt
                .Replace("\r\n", "\n")
                .Split('\n')
                .Where(x => !x.TrimStart().StartsWith("#"));

            if (!file.Entries.ContainsKey(field.id))
                file.Entries.Add(field.id, new SaveFile.EntryData(field));

            var content = string.Join('\n', txtLines);
            file.Entries[field.id].content = content;
            ProjectDebug.LogValueImport(field, content);
        }
    }
}