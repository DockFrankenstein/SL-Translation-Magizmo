using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools;
using System.Linq;
using System;

namespace qASIC.Input.Prompts.Internal
{
    public class PromptsVariantTreeView : TreeView
    {
        PromptLibraryWindow window;

        public PromptsVariantTreeView(TreeViewState state, PromptLibraryWindow window) : base(state)
        {
            this.window = window;
            Reload();
        }

        #region Creation
        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = -1, depth = -1 };
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>();
            rows.Clear();

            if (window.asset != null)
            {
                for (int i = 0; i < window.asset.Variants.Count; i++)
                {
                    var item = window.asset.Variants[i];
                    TreeViewItem treeItem = new TreeViewItem(i, 0, item.name);
                    root.AddChild(treeItem);
                    rows.Add(treeItem);
                }
            }

            return rows;
        }
        #endregion

        #region Selection
        public event Action<int[]> OnChangeSelection;

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            OnChangeSelection?.Invoke(selectedIds.ToArray());
        }

        protected override bool CanMultiSelect(TreeViewItem item) =>
            true;
        #endregion

        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);

            Rect deleteButtonRect = new Rect(args.rowRect)
                .ResizeToRight(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            if (GUI.Button(deleteButtonRect, qGUIEditorUtility.MinusIcon, EditorStyles.label))
            {
                window.asset.Variants.RemoveAt(args.item.id);
                window.SetAssetDirty();
                Reload();
            }

            if (Event.current.type == EventType.Repaint)
            {
                qGUIEditorUtility.HorizontalLine(args.rowRect.ResizeToBottom(0f));
            }
        }

        #region Renaming
        protected override bool CanRename(TreeViewItem item) =>
            true;

        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item) =>
            rowRect
            .MoveX(1f)
            .MoveY(1f);

        protected override void RenameEnded(RenameEndedArgs args)
        {
            window.asset.Variants[args.itemID].name = args.newName;
            window.SetAssetDirty();
            Reload();
        }
        #endregion

        #region Moving
        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("itemIDs", args.draggedItemIDs.ToArray());
            DragAndDrop.SetGenericData("tree", this);
            DragAndDrop.StartDrag(string.Join(",", args.draggedItemIDs.Select(x => FindItem(x, rootItem).displayName)));
        }

        protected override bool CanStartDrag(CanStartDragArgs args) =>
            true;

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (!(DragAndDrop.GetGenericData("tree") is PromptsVariantTreeView sourceTree) ||
                sourceTree != this)
                return DragAndDropVisualMode.Rejected;

            int[] itemIDs = ((int[])DragAndDrop.GetGenericData("itemIDs"))
                .Where(x => window.asset.Variants.IndexInRange(x))
                .ToArray();

            if (itemIDs.Length == 0)
                return DragAndDropVisualMode.Rejected;

            if (args.performDrop)
            {
                var targetItemIndex = args.parentItem?.children.IndexInRange(args.insertAtIndex) == true ?
                        args.parentItem.children[args.insertAtIndex].id - 1 :
                        (args.parentItem?.id ?? window.asset.Variants.Count - 1);
            
                //When items are being dragged after the last item
                if (args.insertAtIndex >= window.asset.Variants.Count)
                    targetItemIndex = window.asset.Variants.Count - 1;

                //If the targeted item is one of the ones that are
                //being moved, find the next lower one
                while (itemIDs.Contains(targetItemIndex) && targetItemIndex > 0)
                    targetItemIndex--;

                var targetItem = window.asset.Variants.IndexInRange(targetItemIndex) ?
                    window.asset.Variants[targetItemIndex] :
                    null;

                //Make a copy of moving items
                var copiedItems = itemIDs
                    .Select(x => window.asset.Variants[x])
                    .ToList();

                //Remove moving items
                foreach (var item in copiedItems)
                    window.asset.Variants.Remove(item);

                //Add items one by one after each other
                int index = targetItem == null ?
                    0 :
                    window.asset.Variants.IndexOf(targetItem) + 1;

                List<int> idsToSelect = new List<int>();
                foreach (var item in copiedItems)
                {
                    idsToSelect.Add(index);
                    window.asset.Variants.Insert(index++, item);
                }

                //Select the moved items
                SetSelection(idsToSelect);
            }

            Reload();
            return DragAndDropVisualMode.Move;
        }
        #endregion

        #region Context Menu
        protected override void ContextClicked()
        {
            DisplayContextMenu();
        }

        void DisplayContextMenu()
        {
            var ids = GetSelection();
            var noSelection = ids.Count == 0;
            var mixedSelection = ids.Count > 1;
            var singleSelection = ids.Count == 1;

            GenericMenu menu = new GenericMenu();

            menu.AddItem("Add", false, () =>
            {
                int index = singleSelection ?
                    ids[0] + 1 :
                    window.asset.Variants.Count;

                var newItem = new PromptsVariant();
                window.asset.Variants.Insert(index, newItem);
                window.SetAssetDirty();
                Reload();
                SetSelection(new int[] { index });
                BeginRename(FindItem(index, rootItem));
            });

            menu.AddToggableItem("Remove", false, () =>
            {
                if (mixedSelection &&
                    !EditorUtility.DisplayDialog("Are you sure", $"Do you want to delete these {ids.Count} items?", "Yes", "No"))
                    return;

                var variants = ids
                    .Select(x => window.asset.Variants[x]);

                foreach (var item in variants)
                    window.asset.Variants.Remove(item);

                window.SetAssetDirty();
                Reload();
            }, !noSelection);

            menu.AddSeparator("");

            menu.AddToggableItem("Ensure Key Types", false, EnsureKeyTypesOfSelectedVariants, !noSelection);

            menu.ShowAsContext();
        }
        #endregion

        #region Utility
        public void EnsureKeyTypesOfSelectedVariants()
        {
            var items = GetSelection()
                .Where(x => window.asset.Variants.IndexInRange(x))
                .Select(x => window.asset.Variants[x]);

            foreach (var variant in items)
                variant.EnsureKeyTypes();

            if (items.Count() > 0)
                window.SetAssetDirty();
        }
        #endregion
    }
}
