using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation.Data;
using qASIC;
using System.Text;
using UnityEditor;
using qASIC.Files;
using UnityEngine.Serialization;

namespace Project.Translation.Mapping
{
    //[CreateAssetMenu(fileName = "New Translation Define", menuName = "Scriptable Objects/Translation/Defines/Multi Entry")]
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
        [EditorButton(nameof(Temp))]
        [FormerlySerializedAs("defines")]
        public List<Line> lines = new List<Line>();

        public override MappedField[] GetMappedFields() =>
            lines
            .SelectMany(x => x.fields)
            .Where(x => x.Status == MappedField.SetupStatus.Used)
            .Select(x => { x.mappingContainer = this; return x; })
            .ToArray();

        
        public void Temp()
        {
#if UNITY_EDITOR
            var text = JsonUtility.ToJson(this);

            var path = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + AssetDatabase.GetAssetPath(this);
            path = System.IO.Path.ChangeExtension(path, EXTENSION);

            FileManager.SaveFileWriter(path, text.ToString());

            Debug.Log(path);
#endif
        }

        public override void Import(SaveFile file, string txt)
        {
            var txtLines = txt
                    .Replace("\r\n", "\n")
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
                            file.Entries[line.fields[x].id] = new SaveFile.EntryData(line.fields[x], splitLine[x]);
                            ProjectDebug.LogValueImport(line.fields[x], splitLine[x]);
                        }

                        break;
                    case false:
                        if (!line.fields[0].addToList) break;
                        file.Entries[line.fields[0].id] = new SaveFile.EntryData(line.fields[0], lineTxt);
                        ProjectDebug.LogValueImport(line.fields[0], line);
                        break;
                }
            }
        }

        public override string Export(SaveFile file)
        {
            StringBuilder txt = new StringBuilder();
            foreach (var define in lines)
            {
                var values = define.fields
                    .Select(x => file.Entries.TryGetValue(x.id, out var y) ?
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

        public override string ExportDebug()
        {
            StringBuilder txt = new StringBuilder();

            for (int i = 0; i < lines.Count; i++)
            {
                var fieldsCount = useSeparationCharacter ? lines[i].fields.Count : 1;

                if (identificationType == IdentificationType.FirstItem)
                {
                    txt.Append($"{lines[i].lineId}{separationCharacter}");
                }

                for (int x = 0; x < fieldsCount; x++)
                {
                    if (fieldsCount == 1)
                    {
                        txt.Append($"{i+1}]{lines[i].fields[x].id}");
                        continue;
                    }

                    txt.Append($"{i+1}.{x}]{lines[i].fields[x].id}");

                    if (x < fieldsCount)
                        txt.Append(separationCharacter);
                }

                txt.Append("\n");
            }

            return txt.ToString();
        }

        [System.Serializable]
        public class Line
        {
            public Line()
            {
                guid = System.Guid.NewGuid().ToString();
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