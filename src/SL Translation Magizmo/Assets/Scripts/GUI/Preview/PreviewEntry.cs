using Project.GUI.Hierarchy;
using Project.Translation;
using Project.Translation.Data;
using qASIC;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.GUI.Preview
{
    public class PreviewEntry : MonoBehaviour
    {
        [Label("Assign")]
        public TMP_Text text;
        public TMP_Text idNameText;
        public GameObject multiEntryPanel;
        public GameObject outlineGroup;
        public Button button;

        [Label("Settings")]
        [SerializeField] bool interactable = true;
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

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            text.text = mainId.defaultValue;

            if (outlineGroup != null)
                outlineGroup.SetActive(interactable);

            if (button != null)
                button.enabled = interactable;

            if (multiEntryPanel != null)
                multiEntryPanel.SetActive(interactable && otherIds.Length > 0);
        }

        private void Awake()
        {
            if (outlineGroup != null)
                outlineGroup.SetActive(interactable);

            if (button != null)
                button.enabled = interactable;

            if (multiEntryPanel != null)
                multiEntryPanel.SetActive(interactable && otherIds.Length > 0);

            Reload();
        }

        public void Reload()
        {
            if (text == null || manager == null) return;

            var currTarget = GetCurrentTarget();
            var txt = currTarget.defaultValue;

            switch (currTarget.content)
            {
                case null:
                    if (manager.File.Entries.TryGetValue(currTarget.entryId, out var content))
                        if (!string.IsNullOrWhiteSpace(content.content))
                            txt = content.content;

                    break;
                default:
                    txt = currTarget.content.GetContent(manager, currTarget.entryId);
                    break;
            }

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
            Reload();
        }

        public void ChangeTargetBy(int moveAmount = 1)
        {
            selectedIndex += moveAmount;

            while (selectedIndex < 0)
                selectedIndex += otherIds.Length + 1;

            while (selectedIndex >= otherIds.Length + 1)
                selectedIndex -= otherIds.Length + 1;

            Reload();
        }
    }
}