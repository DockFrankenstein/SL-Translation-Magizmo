using Project.Translation.Data;
using System.Linq;
using UnityEngine;

namespace Project.Translation.Defines
{
    [CreateAssetMenu(fileName = "New Translation Define", menuName = "Scriptable Objects/Translation/Defines/Array Entry")]
    public class ArrayEntryTranslationDefines : DefinesBase
    {
        public DefineField defineField;
        [TextArea]
        public string prefix;

        public override string Export(SaveFile file)
        {
            if (!file.Entries.ContainsKey(defineField.id))
                return string.Empty;

            var txt = file.Entries[defineField.id].content
                .Replace("\n", "\r\n");

            txt = $"{prefix}\n{txt}";

            return txt;
        }

        public override DefineField[] GetDefines() => new DefineField[] { defineField };

        public override void Import(SaveFile file, string txt)
        {
            var txtLines = txt
                .Replace("\r\n", "\n")
                .Split('\n')
                .Where(x => !x.TrimStart().StartsWith("#"));

            if (!file.Entries.ContainsKey(defineField.id))
                file.Entries.Add(defineField.id, new SaveFile.EntryData(defineField));

            var content = string.Join('\n', txtLines);
            file.Entries[defineField.id].content = content;
            ProjectDebug.LogValueImport(defineField, content);
        }
    }
}