using UnityEngine;
using System;

namespace Project.Translation.Defines.Manifest
{
    [CreateAssetMenu(fileName = "New Manifest Defines", menuName = "Scriptable Objects/Translation/Defines/Manifest/13.1")]
    public class ManifestDefines13_1 : ManifestDefinesBase<ManifestDefines13_1.ManifestData>
    {
        [Serializable]
        public class ManifestData
        {
            [DefineName("manifest_name")] public string Name;
            [DefineName("manifest_authors")] public string[] Authors;
            [DefineName("manifest_interface_locales")] public string[] InterfaceLocales;
            [DefineName("manifest_system_locales")] public string[] SystemLocales;
            [DefineName("manifest_forced_font_order")] public string[] ForcedFontOrder;
        }
    }
}