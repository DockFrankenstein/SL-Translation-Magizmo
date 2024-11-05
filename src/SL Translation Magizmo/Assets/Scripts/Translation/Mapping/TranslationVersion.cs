using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using Project.Translation.Data;
using qASIC.Files;
using qASIC;
using System;
using Project.Text;

namespace Project.Translation.Mapping
{
    [CreateAssetMenu(fileName = "New Project Version", menuName = "Scriptable Objects/Translation/Version", order = 20)]
    public class TranslationVersion : ScriptableObject
    {
        [SerializeField] string displayName;
        public Version version;
        public TextPostProcessing exportPostProcessing;
        public MappingBase[] containers = new MappingBase[0];

        public MappedField[] GetMappedFields() =>
            containers
            .SelectMany(x => x.GetMappedFields()
                .Where(y =>
                {
                    y.mappingContainer = x;
                    return true;
                }))
            .GroupBy(x => x.id)
            .Select(x => x.First())
            .ToArray();

        private Dictionary<string, MappedField> _mappedFields = null;
        public Dictionary<string, MappedField> MappedFields
        {
            get
            {
                if (_mappedFields == null)
                    _mappedFields = GetMappedFields()
                        .ToDictionary(x => x.id);

                return _mappedFields;
            }
        }

        public MappedField GetNameField() =>
            containers
                .Where(x => x.NameField != null)
                .Select(x => x.NameField)
                .FirstOrDefault();

        public string DisplayName =>
            string.IsNullOrWhiteSpace(displayName) ? version.ToString() : displayName;

        public void Initialize()
        {
            foreach (var container in containers)
            {
                var mappedFields = container.GetMappedFields();

                for (int i = 0; i < mappedFields.Length; i++)
                    mappedFields[i].mappingContainer = container;
            }

            _ = MappedFields;
        }

        public void Import(SaveFile file, string path)
        {
            foreach (var definesFile in containers)
            {
                var filePath = $"{path}/{definesFile.fileName}";
                if (!System.IO.File.Exists(filePath)) continue;

                var txt = System.IO.File.ReadAllText(filePath);
                definesFile.Import(file, txt);
            }
        }

        public void Export(SaveFile file, string path, Func<PrepareExportDataArgs, string> prepareData)
        {
            foreach (var container in containers)
            {
                var txt = container.Export((i, x) =>
                {
                    var txt = prepareData(new PrepareExportDataArgs()
                    {
                        container = container,
                        index = i,
                        field = x,
                    });

                    txt ??= exportPostProcessing.ProcessText(txt);
                    return txt;
                });

                FileManager.SaveFileWriter($"{path}/{container.fileName}", txt);
            }
        }

        public void Export(SaveFile file, string path, string emptyEntryContent = "-")
        {
            foreach (var definesFile in containers)
            {
                var txt = definesFile.Export((i, x) =>
                {
                    return file.Entries.TryGetValue(x.id, out var val) && !x.IsBlank ?
                        (exportPostProcessing.ProcessText(val.content) ?? val.content) :
                        emptyEntryContent;
                });

                FileManager.SaveFileWriter($"{path}/{definesFile.fileName}", txt);
            }
        }

        public class PrepareExportDataArgs
        {
            public int index;
            public MappedField field;
            public MappingBase container;
        }
    }
}