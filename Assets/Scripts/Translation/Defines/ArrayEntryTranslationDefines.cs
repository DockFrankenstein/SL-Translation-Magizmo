using Project.Translation.Data;
using System.Linq;
using UnityEngine;

namespace Project.Translation.Defines
{
    [CreateAssetMenu(fileName = "New Translation Define", menuName = "Scriptable Objects/Translation/Defines/Array Entry")]
    public class ArrayEntryTranslationDefines : DefinesBase
    {
        public string fieldId;
        [TextArea]
        public string prefix;

        public override string Export(AppFile file)
        {
            if (!file.Entries.ContainsKey(fieldId))
                return string.Empty;

            var txt = file.Entries[fieldId].content
                .Replace("\n", "\r\n");

            txt = $"{prefix}\n{txt}";

            return txt;
        }

        public override string[] GetDefines() => new string[] { fieldId };

        public override void Import(AppFile file, string txt)
        {
            var txtLines = txt
                .Replace("\r\n", "\n")
                .Split('\n')
                .Where(x => !x.TrimStart().StartsWith("#"));

            if (!file.Entries.ContainsKey(fieldId))
                file.Entries.Add(fieldId, new AppFile.EntryData(fieldId));

            var content = string.Join('\n', txtLines);
            file.Entries[fieldId].content = content;
            ProjectDebug.LogValueImport(fieldId, content);
        }
    }
}