using Project.GUI.Hierarchy;
using Project.Translation;
using Project.Translation.Data;
using qASIC;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Project.GUI.Preview
{
    public class PreviewEntry : MonoBehaviour
    {
        public TMP_Text text;
        public TMP_Text idNameText;
        public GameObject multiEntryPanel;

        [Label("Targets")]
        public MappedIdTarget mainId = new MappedIdTarget(string.Empty, true);
        public MappedIdTarget[] otherIds;

        [HideInInspector] public TranslationManager manager;
        [HideInInspector] public HierarchyController hierarchy;

        int selectedIndex;

        MappedIdTarget GetCurrentTarget()
        {
            var i = selectedIndex % (otherIds.Length + 1);
            return i == 0 ?
                mainId :
                otherIds[i - 1];
        }

        public IEnumerable<MappedIdTarget> GetListOfTargets() =>
            otherIds.Prepend(mainId);

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            if (multiEntryPanel != null)
                multiEntryPanel.SetActive(otherIds.Length > 0);

            UpdateContent();
        }

        public void UpdateContent()
        {
            if (text == null || manager == null) return;

            var currTarget = GetCurrentTarget();
            var txt = currTarget.defaultValue;

            if (manager.File.Entries.TryGetValue(currTarget.entryId, out var content))
                if (!string.IsNullOrEmpty(content.content))
                    txt = content.content;

            text.text = txt;

            if (idNameText != null)
                idNameText.text = currTarget.entryId;
        }

        public void Select()
        {
            if (hierarchy == null) return;
            hierarchy.Select(GetCurrentTarget().entryId, true);
        }

        public void ChangeTarget(string id)
        {
            var target = otherIds
                .Where(x => x.entryId == id)
                .FirstOrDefault();

            selectedIndex = Array.IndexOf(otherIds, target) + 1;
            UpdateContent();
        }

        public void ChangeTargetBy(int moveAmount = 1)
        {
            selectedIndex += moveAmount;

            while (selectedIndex < 0)
                selectedIndex += otherIds.Length + 1;

            while (selectedIndex >= otherIds.Length + 1)
                selectedIndex -= otherIds.Length + 1;

            UpdateContent();
        }
    }
}