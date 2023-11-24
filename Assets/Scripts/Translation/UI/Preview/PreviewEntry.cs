using TMPro;
using UnityEngine;

namespace Project.Translation.UI.Preview
{
    public class PreviewEntry : MonoBehaviour
    {
        public TMP_Text text;
        public string entryID;

        [HideInInspector] public TranslationManager manager;
        [HideInInspector] public HierarchyDisplay hierarchy;

        public void Select()
        {
            if (hierarchy == null) return;
            hierarchy.Select(entryID, true);
        }
    }
}