using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using SFB;
using Project.Translation.Data;
using qASIC.Files;
using qASIC;

namespace Project.Translation.Mapping
{
    [CreateAssetMenu(fileName = "New Project Version", menuName = "Scriptable Objects/Translation/Version", order = 20)]
    public class TranslationVersion : ScriptableObject
    {
        public Version version;
        public MappingBase[] containers = new MappingBase[0];

        [Label("Exporting")]
        [EditorButton(nameof(ExportIdTestTranslation))]
        public string emptyEntryContent = "-";

        public MappedField[] GetMappedFields() =>
            containers
            .SelectMany(x => x.GetMappedFields().ForEach(y => y.mappingContainer = x))
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

        public void Export(SaveFile file, string path, string emptyEntryContent = "-")
        {
            foreach (var definesFile in containers)
            {
                var txt = definesFile.Export((i, x) =>
                {
                    return file.Entries.TryGetValue(x.id, out var val) && !x.IsBlank ?
                        val.content :
                        emptyEntryContent;
                });

                FileManager.SaveFileWriter($"{path}/{definesFile.fileName}", txt);
            }
        }

        public void ExportIdTestTranslation()
        {
#if UNITY_EDITOR
            var path = EditorUtility.OpenFolderPanel("Export Translation", string.Empty, string.Empty);
#else
            var paths = StandaloneFileBrowser.OpenFolderPanel("Export Translation", string.Empty, false);
            var path = paths.Length == 1 ? paths[0] : string.Empty;
#endif

            if (!string.IsNullOrWhiteSpace(path))
            {
                foreach (var container in containers)
                {
                    var txt = container.Export((i, x) =>
                    {
                        return $"{container.fileName}:{i}";
                    });

                    System.IO.File.WriteAllText($"{path.Replace('\\', '/')}/{container.fileName}", txt);
                }
            }
        }
    }
}