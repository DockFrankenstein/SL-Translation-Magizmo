using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation.Data;
using qASIC;
using System.Text;
using UnityEditor;
using UnityEngine.Serialization;
using System;

namespace Project.Translation.Mapping
{
    public class MultiEntryTranslationMapping : MappingBase
    {
        public const string EXTENSION = "metd";

        public enum IdentificationType
        {
            LineId,
            FirstItem,
        }

        public IdentificationType identificationType;
        public bool useSeparationCharacter;
        public char separationCharacter;

        [ReorderableList]
        [FormerlySerializedAs("defines")]
        public List<Line> lines = new List<Line>();

        public override MappedField[] GetMappedFields() =>
            lines
            .SelectMany(x => x.fields)
            .Where(x => x.Status == MappedField.SetupStatus.Used)
            .Select(x => { x.mappingContainer = this; return x; })
            .ToArray();

        public override void Import(SaveFile file, string txt)
        {
            var txtLines = txt
                    .Replace("\r\n", "\n")
                    .Replace('\r', '\n')
                    .Split('\n')
                    .ToArray();

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                foreach (var mappedFields in line.fields)
                    if (!file.Entries.ContainsKey(mappedFields.id))
                        file.Entries.Add(mappedFields.id, new SaveFile.EntryData(mappedFields));

                string lineTxt;

                switch (identificationType)
                {
                    //Search by line index
                    default:
                        if (!txtLines.IndexInRange(i))
                            continue;

                        lineTxt = txtLines[i];
                        break;

                    case IdentificationType.FirstItem:
                        var startsWithText = $"{line.lineId}{separationCharacter}";

                        var targetLines = txtLines
                            .Where(x => x.StartsWith(startsWithText));

                        if (targetLines.Count() == 0)
                            continue;

                        lineTxt = targetLines.First().Remove(0, startsWithText.Length);
                        break;
                }

                switch (useSeparationCharacter && line.fields.Count > 1)
                {
                    case true:
                        var splitLine = lineTxt.Split(separationCharacter);

                        for (int x = 0; x < Mathf.Min(line.fields.Count, splitLine.Length); x++)
                        {
                            if (!line.fields[x].addToList) continue;
                            file.Entries[line.fields[x].id].content = splitLine[x];
                        }

                        break;
                    case false:
                        if (line.fields.Count == 0) break;
                        if (!line.fields[0].addToList) break;

                        file.Entries[line.fields[0].id].content = lineTxt;
                        break;
                }
            }
        }

        public override string Export(Func<int, MappedField, string> getTextContent)
        {
            StringBuilder txt = new StringBuilder();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                var values = line.fields
                    .Select(x => getTextContent(i, x));

                switch (identificationType)
                {
                    default:
                        txt.Append($"{string.Join(separationCharacter, values)}\n");
                        break;
                    case IdentificationType.FirstItem:
                        var valuesTxt = string.Join(separationCharacter, values);

                        if (valuesTxt.EndsWith(separationCharacter))
                            valuesTxt = valuesTxt.Substring(0, valuesTxt.Length - 1);

                        txt.Append($"{line.lineId}{separationCharacter}{valuesTxt}\n");
                        break;
                }
            }

            return txt.ToString();
        }

        [Serializable]
        public class Line
        {
            public Line()
            {
                guid = Guid.NewGuid().ToString();
            }

            public string lineId;
            public string guid;
            public List<MappedField> fields = new List<MappedField>();

            public Line Duplicate(bool duplicateDefines = true)
            {
                var lines = new Line()
                {
                    lineId = lineId,
                    fields = fields
                        .Select(x => x.Duplicate())
                        .ToList(),
                };

                lines.fields = duplicateDefines ? 
                    fields.Select(x => x.Duplicate()).ToList() : 
                    new List<MappedField>(fields);

                return lines;
            }
        }
    }
}