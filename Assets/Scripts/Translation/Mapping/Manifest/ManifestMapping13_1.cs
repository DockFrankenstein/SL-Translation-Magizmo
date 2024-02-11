using UnityEngine;
using System;

namespace Project.Translation.Mapping.Manifest
{
    [CreateAssetMenu(fileName = "New Manifest Defines", menuName = "Scriptable Objects/Translation/Mapping/Manifest/13.1")]
    public class ManifestMapping13_1 : ManifestMappingBase<ManifestMapping13_1.ManifestData>
    {
        [Serializable]
        public class ManifestData
        {
            [MappedFieldName("manifest_name")] public string Name;
            [MappedFieldName("manifest_authors")] public string[] Authors;
            [MappedFieldName("manifest_interface_locales")] public string[] InterfaceLocales;
            [MappedFieldName("manifest_system_locales")] public string[] SystemLocales;
            [MappedFieldName("manifest_forced_font_order")] public string[] ForcedFontOrder;
        }
    }
}