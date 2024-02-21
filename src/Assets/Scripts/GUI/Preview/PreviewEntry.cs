using Project.GUI.Hierarchy;
using Project.Translation;
using Project.Translation.Data;
using TMPro;
using UnityEngine;

namespace Project.GUI.Preview
{
    public class PreviewEntry : MonoBehaviour
    {
        public TMP_Text text;
        public string entryID;

        [HideInInspector] public TranslationManager manager;
        [HideInInspector] public HierarchyController hierarchy;

        string _defaultText;

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            _defaultText = text?.text ?? string.Empty;
        }

        public void UpdateContent(SaveFile file)
        {
            if (text == null) return;
            var txt = _defaultText;

            if (file.Entries.TryGetValue(entryID, out var content))
                if (!string.IsNullOrEmpty(content.content))
                    txt = content.content;

            text.text = txt;
        }

        public void Select()
        {
            if (hierarchy == null) return;
            hierarchy.Select(entryID, true);
        }
    }
}