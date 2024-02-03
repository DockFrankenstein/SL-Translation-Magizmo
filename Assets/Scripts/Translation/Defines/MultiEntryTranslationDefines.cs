using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation.Data;
using qASIC;
using System.Text;
using UnityEditor;
using qASIC.Files;
using UnityEngine.Serialization;

namespace Project.Translation.Defines
{
    //[CreateAssetMenu(fileName = "New Translation Define", menuName = "Scriptable Objects/Translation/Defines/Multi Entry")]
    public class MultiEntryTranslationDefines : DefinesBase
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

        public override DefineField[] GetDefines() =>
            lines
            .SelectMany(x => x.defines)
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
                var define = lines[i];

                foreach (var defineField in define.defines)
                    if (!file.Entries.ContainsKey(defineField.id))
                        file.Entries.Add(defineField.id, new SaveFile.EntryData(defineField));

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

                        for (int x = 0; x < Mathf.Min(define.defines.Count, splitLine.Length); x++)
                        {
                            file.Entries[define.defines[x].id] = new SaveFile.EntryData(define.defines[x], splitLine[x]);
                            ProjectDebug.LogValueImport(define.defines[x], splitLine[x]);
                        }
                        break;
                    case false:
                        file.Entries[define.defines[0].id] = new SaveFile.EntryData(define.defines[0], line);
                        ProjectDebug.LogValueImport(define.defines[0], line);
                        break;
                }
            }
        }

        public override string Export(SaveFile file)
        {
            StringBuilder txt = new StringBuilder();
            foreach (var define in lines)
            {
                var values = define.defines
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

        [System.Serializable]
        public class Line
        {
            public Line()
            {
                guid = System.Guid.NewGuid().ToString();
            }

            public string lineId;
            public string guid;
            [FormerlySerializedAs("fieldIds")]
            public List<DefineField> defines = new List<DefineField>();
        }
    }
}