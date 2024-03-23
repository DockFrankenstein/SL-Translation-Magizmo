using Project.GUI.Hierarchy;
using Project.Translation;
using Project.Translation.Data;
using qASIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
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

        int targetIndex;
        int targetValueIndex;

        string[] currentValues = new string[0];

        MappedIdTarget GetCurrentTarget()
        {
            var i = targetIndex % (otherIds.Length + 1);
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
            Reload();
        }

        public void Reload()
        {
            if (text == null || manager == null) return;

            var currTarget = GetCurrentTarget();

            LoadTargetValues();

            var txt = currTarget.defaultValue;
            if (currentValues.Length > 0)
            {
                if (!currentValues.IndexInRange(targetValueIndex))
                    targetValueIndex = currentValues.Length - 1;

                if (!string.IsNullOrWhiteSpace(currentValues[targetValueIndex])) 
                    txt = currentValues[targetValueIndex];
            }

            text.text = txt;

            if (idNameText != null)
            {
                idNameText.text = currentValues.Length > 1 ?
                    $"{currTarget.entryId}:{targetValueIndex}" :
                    currTarget.entryId;
            }

            if (outlineGroup != null)
                outlineGroup.SetActive(interactable);

            if (button != null)
                button.enabled = interactable;

            if (multiEntryPanel != null)
                multiEntryPanel.SetActive(interactable && (otherIds.Length > 0 || currentValues.Length > 1));
        }

        void LoadTargetValues()
        {
            currentValues = new string[0];
            var currTarget = GetCurrentTarget();

            switch (currTarget.content)
            {
                case null:
                    if (manager.File.Entries.TryGetValue(currTarget.entryId, out var content))
                            currentValues = new string[] { content.content };

                    break;
                default:
                    currentValues = currTarget.content.GetContent(manager, currTarget.entryId);
                    break;
            }
        }

        public void Select()
        {
            if (hierarchy == null) return;
            hierarchy.Select(GetCurrentTarget().entryId, true);
        }

        public void ChangeTarget(string id)
        {
            targetValueIndex = id.GetIdIndex();
            id = id.GetBaseId();

            var target = otherIds
                .Where(x => x.entryId == id)
                .FirstOrDefault();

            targetIndex = Array.IndexOf(otherIds, target) + 1;

            LoadTargetValues();

            if (targetValueIndex < 0)
                targetValueIndex = 0;

            if (targetValueIndex >= currentValues.Length)
                targetValueIndex = currentValues.Length - 1;

            Reload();
        }

        public void ChangeTargetBy(int moveAmount = 1)
        {
            bool isNegative = moveAmount < 0;
            int moveAmountAbs = isNegative ? -moveAmount : moveAmount;

            while (moveAmountAbs > 0)
            {
                moveAmountAbs--;

                targetValueIndex += isNegative ? -1 : 1;

                if (targetValueIndex >= currentValues.Length)
                {
                    targetValueIndex = 0;
                    targetIndex++;

                    if (targetIndex >= otherIds.Length + 1)
                        targetIndex = 0;

                    LoadTargetValues();
                    continue;
                }

                if (targetValueIndex < 0)
                {
                    targetIndex--;

                    if (targetIndex < 0)
                        targetIndex = otherIds.Length;

                    LoadTargetValues();
                    targetValueIndex = currentValues.Length - 1;
                }
            }

            Reload();
        }
    }
}