using UnityEngine;
using qASIC.EditorTools;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using UnityEditor;

namespace qASIC.Input.Prompts.Internal
{
    internal class PromptTreeView : TreeView
    {
        PromptLibraryWindow window;

        public PromptsVariant SelectedVariant { get; set; }

        public PromptTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, PromptLibraryWindow window) : base(state, multiColumnHeader)
        {
            this.window = window;
            window.variantTree.OnChangeSelection += VariantTree_OnChangeSelection;
            rowHeight = 48f;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            multiColumnHeader.sortingChanged += MultiColumnHeader_sortingChanged;

            Reload();
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Icon"),
                    width = 48f,
                    maxWidth = 48f,
                    minWidth = 48f,
                    autoResize = false,
                    canSort = false,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Path"),
                    width = 200f,
                    autoResize = true,
                    canSort = false,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Display Name"),
                    width = 200f,
                    autoResize = true,
                    allowToggleVisibility = true,
                    canSort = false,
                },
            };

            return new MultiColumnHeaderState(columns);
        }

        private void MultiColumnHeader_sortingChanged(MultiColumnHeader multiColumnHeader)
        {
            
        }

        private void VariantTree_OnChangeSelection(int[] ids)
        {
            var oneSelected = ids.Length == 1 && window.asset.Variants.IndexInRange(ids[0]);

            SelectedVariant = oneSelected ?
                window.asset.Variants[ids[0]] :
                null;

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>();
            rows.Clear();

            if (SelectedVariant != null)
            {
                foreach (var prompt in SelectedVariant.Prompts)
                {
                    var item = new PromptTreeViewItem(prompt.Value);
                    root.AddChild(item);
                    rows.Add(item);
                }
            }

            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            switch (args.item)
            {
                case PromptTreeViewItem promptItem:
                    using (new EditorChangeChecker.ChangeCheck(window.SetAssetDirty))
                    {
                        for (int i = 0; i < args.GetNumVisibleColumns(); i++)
                            PromptItemCellGUI(args.GetCellRect(i), promptItem, args.GetColumn(i), ref args);
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        qGUIEditorUtility.HorizontalLine(args.rowRect.ResizeToBottom(0f));
                    }
                    break;
                default:
                    base.RowGUI(args);
                    break;
            }
        }

        void PromptItemCellGUI(Rect cellRect, PromptTreeViewItem item, int columnIndex, ref RowGUIArgs args)
        {
            switch (columnIndex)
            {
                case 0:
                    cellRect = cellRect
                        .ResizeWidthToCenter(cellRect.height);

                        SelectedVariant.Prompts[item.key].sprite = (Sprite)EditorGUI.ObjectField(cellRect, SelectedVariant.Prompts[item.key].sprite, typeof(Sprite), false);
                    break;
                case 1:
                    GUI.Label(cellRect, item.key);
                    break;
                case 2:
                    cellRect = cellRect
                        .ResizeHeightToCenter(EditorGUIUtility.singleLineHeight);

                    SelectedVariant.Prompts[item.key].displayName = EditorGUI.DelayedTextField(cellRect, GUIContent.none, SelectedVariant.Prompts[item.key].displayName);
                    break;
            }
        }
    }

    internal class PromptTreeViewItem : TreeViewItem
    {
        public PromptTreeViewItem()
        {
            depth = 0;
        }

        public PromptTreeViewItem(PromptsVariant.Prompt prompt)
        {
            id = prompt.key.GetHashCode();
            displayName = prompt.key;
            key = prompt.key;
        }

        public string key;
    }
}