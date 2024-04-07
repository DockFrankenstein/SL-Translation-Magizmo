using Project.GUI.Preview.Interfaces;
using qASIC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.GUI.Preview
{
    public class PreviewEntry : PreviewElement, IHasMappedTargets
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
        [ReorderableList] public MappedIdTarget[] otherIds;

        [Label("Replace Ids")]
        [SerializeField] [InspectorName("From")] string idReplaceFrom;
        [SerializeField] [InspectorName("To")] string idReplaceTo;
        [EditorButton(nameof(ReplaceIdsTool))]
        [SerializeField] [InspectorName("Use Regex")] bool idReplaceRegex;

        int SelectedIndex { get; set; }
        int SelectedValueIndex { get; set; }

        string[] currentValues = new string[0];

        MappedIdTarget GetCurrentTarget()
        {
            var i = SelectedIndex % (otherIds.Length + 1);
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

        void ReplaceIdsTool()
        {
            mainId = ReplaceInField(mainId);
            for (int i = 0; i < otherIds.Length; i++)
                otherIds[i] = ReplaceInField(otherIds[i]);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

            MappedIdTarget ReplaceInField(MappedIdTarget target)
            {
                if (idReplaceRegex)
                {
                    target.entryId = Regex.Replace(target.entryId, idReplaceFrom, idReplaceTo);
                    return target;
                }

                target.entryId = target.entryId.Replace(idReplaceFrom, idReplaceTo);
                return target;
            }
        }

        public override void Reload()
        {
            if (text == null || manager == null) return;

            var currTarget = GetCurrentTarget();

            LoadTargetValues();

            var txt = currTarget.defaultValue;
            if (currentValues.Length > 0)
            {
                if (!currentValues.IndexInRange(SelectedValueIndex))
                    SelectedValueIndex = currentValues.Length - 1;

                if (!string.IsNullOrWhiteSpace(currentValues[SelectedValueIndex])) 
                    txt = currentValues[SelectedValueIndex];
            }

            text.text = txt.Replace("\\n", "\n");

            if (idNameText != null)
            {
                idNameText.text = currentValues.Length > 1 ?
                    $"{currTarget.entryId}:{SelectedValueIndex}" :
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

            if (manager.File.Entries.TryGetValue(currTarget.entryId, out var content))
                currentValues = new string[] { content.content };

            switch (currTarget.content)
            {
                case null:
                    break;
                default:
                    var args = new MappedIdContent.GetContentArgs()
                    {
                        manager = manager,
                        id = currTarget.entryId,
                        normalContent = currentValues,
                        defaultContent = currTarget.defaultValue,
                    };

                    currentValues = currTarget.content.GetContent(args, currTarget.contentContext);
                    break;
            }
        }

        public override void Select()
        {
            if (hierarchy == null) return;

            var current = GetCurrentTarget();


            hierarchy.Select(current.useCustomSelectId ? current.customSelectId : current.entryId, true);
        }

        public override void ChangeIndex(int newIndex, bool silent = false)
        {
            base.ChangeIndex(newIndex, silent);

            SelectedIndex = Mathf.Clamp(newIndex, 0, otherIds.Length);
            SelectedValueIndex = 0;

            Reload();
        }

        public void ChangeTarget(string id)
        {
            SelectedValueIndex = id.GetIdIndex();
            id = id.GetBaseId();

            var target = otherIds
                .Where(x => x.entryId == id)
                .FirstOrDefault();

            SelectedIndex = Array.IndexOf(otherIds, target) + 1;

            LoadTargetValues();

            if (SelectedValueIndex < 0)
                SelectedValueIndex = 0;

            if (SelectedValueIndex >= currentValues.Length)
                SelectedValueIndex = currentValues.Length - 1;

            ChangeIndexInLinked(SelectedIndex);
            Reload();
        }

        public void ChangeTargetBy(int moveAmount = 1)
        {
            bool isNegative = moveAmount < 0;
            int moveAmountAbs = isNegative ? -moveAmount : moveAmount;

            while (moveAmountAbs > 0)
            {
                moveAmountAbs--;

                SelectedValueIndex += isNegative ? -1 : 1;

                if (SelectedValueIndex >= currentValues.Length)
                {
                    SelectedValueIndex = 0;
                    SelectedIndex++;

                    if (SelectedIndex >= otherIds.Length + 1)
                        SelectedIndex = 0;

                    LoadTargetValues();
                    continue;
                }

                if (SelectedValueIndex < 0)
                {
                    SelectedIndex--;

                    if (SelectedIndex < 0)
                        SelectedIndex = otherIds.Length;

                    LoadTargetValues();
                    SelectedValueIndex = currentValues.Length - 1;
                }
            }

            ChangeIndexInLinked(SelectedIndex);
            Reload();
        }
    }
}