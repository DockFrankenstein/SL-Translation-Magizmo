using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation.Data;
using qASIC;
using System.Text;

namespace Project.Translation.Defines
{
    [CreateAssetMenu(fileName = "New Translation Define", menuName = "Scriptable Objects/Translation/Defines/Multi Entry")]
    public class MultiEntryTranslationDefines : TranslationDefinesBase
    {
        public enum IdentificationType
        {
            LineId,
            FirstItem,
        }

        public IdentificationType identificationType;
        public bool useSeparationCharacter;
        public char separationCharacter;

        public List<Define> defines = new List<Define>();

        public override string[] GetDefines() =>
            defines
            .SelectMany(x => x.fieldIds)
            .ToArray();

        public override void Import(AppFile file, string txt)
        {
            var txtLines = txt
                    .Replace("\r\n", "\n")
                    .Split('\n')
                    .ToArray();

            for (int i = 0; i < defines.Count; i++)
            {
                var define = defines[i];

                foreach (var id in define.fieldIds)
                    if (!file.Entries.ContainsKey(id))
                        file.Entries.Add(id, new AppFile.EntryData(id));

                string line;

                switch (identificationType)
                {
                    //Search by line index
                    default:
                        if (!txtLines.IndexInRange(i))
                            continue;

                        line = txtLines[i];
                        break;

                    case IdentificationType.FirstItem:
                        var startsWithText = $"{define.lineId}{separationCharacter}";

                        var targetLines = txtLines
                            .Where(x => x.StartsWith(startsWithText));

                        if (targetLines.Count() == 0)
                            continue;

                        line = targetLines.First().Remove(0, startsWithText.Length);
                        break;
                }

                switch (useSeparationCharacter)
                {
                    case true:
                        var splitLine = line.Split(separationCharacter);

                        for (int x = 0; x < Mathf.Min(define.fieldIds.Length, splitLine.Length); x++)
                            file.Entries[define.fieldIds[x]] = new AppFile.EntryData(define.fieldIds[x], splitLine[x]);
                        break;
                    case false:
                        file.Entries[define.fieldIds[0]] = new AppFile.EntryData(define.fieldIds[0], line);
                        break;
                }
            }
        }

        public override string Export(AppFile file)
        {
            StringBuilder txt = new StringBuilder();
            foreach (var define in defines)
            {
                var values = define.fieldIds
                    .Select(x => file.Entries.TryGetValue(x, out var y) ?
                        y.content :
                        string.Empty);

                switch (identificationType)
                {
                    default:
                        txt.Append($"{string.Join(separationCharacter, values)}\n");
                        break;
                    case IdentificationType.FirstItem:
                        var valuesTxt = string.Join(separationCharacter, values);

                        if (valuesTxt.EndsWith(separationCharacter))
                            valuesTxt = valuesTxt.Substring(0, valuesTxt.Length - 1);

                        txt.Append($"{define.lineId}{separationCharacter}{valuesTxt}\n");
                        break;
                }
            }

            return txt.ToString();
        }

        [System.Serializable]
        public struct Define
        {
            public string lineId;
            public string[] fieldIds;
        }
    }
}