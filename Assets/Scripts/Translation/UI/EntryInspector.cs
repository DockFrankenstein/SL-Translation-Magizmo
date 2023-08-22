using TMPro;
using UnityEngine;

namespace Project.Translation.UI
{
    public class EntryInspector : MonoBehaviour
    {
        public TranslationManager manager;

        public TMP_InputField contentField;

        private void Awake()
        {
            manager.OnSelectionChange += Manager_OnSelectionChange;
            contentField.onValueChanged.AddListener(ContentField_OnValueChanged);
        }

        private void Manager_OnSelectionChange(string entryId)
        {
            var contentText = string.Empty;

            if (entryId != null && manager.file.Entries.ContainsKey(entryId))
                contentText = manager.file.Entries[entryId].content;

            contentField.text = contentText;
        }

        private void ContentField_OnValueChanged(string text)
        {
            if (manager.SelectedItem != null && manager.file.Entries.ContainsKey(manager.SelectedItem))
                manager.file.Entries[manager.SelectedItem].content = text;
        }
    }
}