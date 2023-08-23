using Project.UI;
using TMPro;
using UnityEngine;
using System.Linq;
using Project.Translation.Defines;

namespace Project.Translation.UI
{
    public class SingleEntryInspector : InspectorDisplayPanel
    {
        public TMP_InputField contentField;

        private void Awake()
        {
            contentField.onValueChanged.AddListener(ContentField_OnValueChanged);
        }

        public override void Initialize()
        {
            if (manager.file.Entries.ContainsKey(id))
                contentField.SetTextWithoutNotify(manager.file.Entries[id].content);
        }

        public override void Uninitialize()
        {
            contentField.SetTextWithoutNotify(string.Empty);
        }

        public override bool ShouldOpen(string id)
        {
            var defines = manager.CurrentVersion.defines
                .Where(x => x is MultiEntryTranslationDefines)
                .SelectMany(x => x.GetDefines());

            return defines.Contains(id);
        }

        private void ContentField_OnValueChanged(string text)
        {
            if (manager.file.Entries.ContainsKey(id))
                manager.file.Entries[id].content = text;
        }
    }
}