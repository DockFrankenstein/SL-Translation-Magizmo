using Project.GUI.Hierarchy;
using Project.Translation;
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

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        public void Select()
        {
            if (hierarchy == null) return;
            hierarchy.Select(entryID, true);
        }
    }
}