using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using SFB;

namespace Project.Translation.Mapping
{
    [CreateAssetMenu(fileName = "New Project Version", menuName = "Scriptable Objects/Translation/Version", order = 20)]
    public class TranslationVersion : ScriptableObject
    {
        public string version;
        [EditorButton(nameof(ExportIdTestTranslation))]
        public MappingBase[] containers = new MappingBase[0];

        public MappedField[] GetMappedFields() =>
            containers
            .SelectMany(x => x.GetMappedFields())
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
                    var txt = container.ExportDebug();
                    System.IO.File.WriteAllText($"{path.Replace('\\', '/')}/{container.fileName}", txt);
                }
            }
        }
    }
}