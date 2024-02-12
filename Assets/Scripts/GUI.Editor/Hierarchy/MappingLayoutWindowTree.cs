using Project.GUI.Hierarchy;
using qASIC.EditorTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using qASIC;
using System.Linq;

namespace Project.GUI.Editor.Hierarchy
{
    public class MappingLayoutWindowTree : TreeView
    {
        public MappingLayoutWindowTree(TreeViewState state, MappingLayoutWindow window) : base(state)
        {
            this.window = window;
            showAlternatingRowBackgrounds = true;
            rowHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2f;

            foldoutOverride += (Rect position, bool expandedState, GUIStyle style) =>
            {
                var foldoutIcon = EditorGUIUtility.IconContent(expandedState ? "IN_foldout_on@2x" : "IN_foldout@2x");
                position = position.MoveX(8f).ResizeToLeft(22f);
                if (UnityEngine.GUI.Button(position, foldoutIcon, EditorStyles.label))
                    expandedState = !expandedState;

                return expandedState;
            };

            Reload();
            ExpandAllBetter();
        }

        MappingLayoutWindow window;

        protected override TreeViewItem BuildRoot() =>
            new TreeViewItem(0, -1);

        bool _isSearching;

        List<Item> _headerItems = new List<Item>();

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>();
            rows.Clear();
            _headerItems.Clear();

            _isSearching = !string.IsNullOrWhiteSpace(searchString);

            if (window.asset != null)
            {
                var items = window.asset.items;

                if (_isSearching)
                {
                    List<List<HierarchyItem>> splitItems = new List<List<HierarchyItem>>()
                    {
                        new List<HierarchyItem>(),
                    };

                    foreach (var item in window.asset.items)
                    {
                        var list = splitItems.Last();

                        if (list.Count == 0)
                        {
                            switch (item.type)
                            {
                                case HierarchyItem.ItemType.Normal:
                                    list.Add(null);
                                    break;
                                default:
                                    list.Add(item);
                                    continue;
                            }
                        }

                        switch (item.type)
                        {
                            case HierarchyItem.ItemType.Normal:
                                list.Add(item);
                                break;
                            default:
                                splitItems.Add(new List<HierarchyItem>()
                                {
                                    item,
                                });
                                break;
                        }
                    }

                    items = splitItems
                        .Select(x =>
                        {
                            var first = x.FirstOrDefault();
                            x.RemoveAt(0);

                            x = qGUIEditorUtility.SortSearchList(x, x => x.id, searchString)
                                .ToList();

                            if (first != null)
                                x.Insert(0, first);

                            return x;
                        })
                        .SelectMany(x => x)
                        .ToList();
                }

                bool hideItems = false;
                foreach (var item in items)
                {
                    var treeItem = new Item(item);
                    bool isHeader = item.type == HierarchyItem.ItemType.Header;

                    if (item.type == HierarchyItem.ItemType.Header)
                    {
                        _headerItems.Add(treeItem);
                        hideItems = !IsExpanded(treeItem.id);
                    }

                    if (!hideItems || isHeader)
                    {
                        rows.Add(treeItem);
                        rootItem.AddChild(treeItem);
                    }
                }
            }

            return rows;
        }

        protected override void ContextClickedItem(int id)
        {
            var menu = new GenericMenu();

            menu.AddItem("Expand All", false, ExpandAllBetter);
            menu.AddItem("Collapse All", false, CollapseAllBetter);

            menu.ShowAsContext();
        }

        void ExpandAllBetter()
        {
            foreach (var item in _headerItems)
                SetExpanded(item.id, true);
        }

        void CollapseAllBetter()
        {
            foreach (var item in _headerItems)
                SetExpanded(item.id, false);
        }

        public void CreateNewItem()
        {
            CreateNewItem(new HierarchyItem(string.Empty, "Header")
            {
                type = HierarchyItem.ItemType.Header,
            });
        }

        public void CreateNewItem(HierarchyItem item)
        {
            if (window.asset == null) return;

            var index = FindItem(GetSelection().Last(), rootItem) is Item treeItem ?
                window.asset.items.IndexOf(treeItem.item) + 1 :
                window.asset.items.Count;

            window.asset.items.Insert(index, item);
            Reload();
        }

        protected override bool CanRename(TreeViewItem item)
        {
            return item is Item hierarchyItem && hierarchyItem.item.type != HierarchyItem.ItemType.Separator;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (!(FindItem(args.itemID, rootItem) is Item item)) return;

            if (args.acceptedRename)
            {
                switch (item.item.type)
                {
                    case HierarchyItem.ItemType.Normal:
                        item.item.id = args.newName;
                        break;
                    default:
                        item.item.displayText = args.newName;
                        break;
                }
            }

            window.SetAssetDirty();
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            if (item is Item treeItem && treeItem.item.type == HierarchyItem.ItemType.Header)
                return 46f;

            return base.GetCustomRowHeight(row, item);
        }

        protected override void DoubleClickedItem(int id)
        {
            var item = FindItem(id, rootItem);
            if (item == null) return;

            if (CanRename(item))
                BeginRename(item);
        }

        protected override bool CanChangeExpandedState(TreeViewItem item)
        {
            return item is Item treeItem && treeItem?.item?.type == HierarchyItem.ItemType.Header;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            switch (args.item)
            {
                case Item item:
                    var baseRect = args.rowRect
                        .ResizeHeightToCenter(EditorGUIUtility.singleLineHeight)
                        .BorderLeft(30f);

                    var typeRect = baseRect
                        .ResizeToLeft(100f);

                    var nameRect = baseRect
                        .BorderLeft(100f + EditorGUIUtility.standardVerticalSpacing);

                    using (var changed = new EditorGUI.ChangeCheckScope())
                    {
                        item.item.type = (HierarchyItem.ItemType)EditorGUI.EnumPopup(typeRect, item.item.type);

                        if (changed.changed)
                            Reload();
                    }

                    switch (item.item.type)
                    {
                        case HierarchyItem.ItemType.Normal:
                            EditorGUI.LabelField(nameRect, item.item.id);
                            break;
                        case HierarchyItem.ItemType.Header:
                            EditorGUI.LabelField(nameRect, item.item.displayText, EditorStyles.whiteLargeLabel);
                            break;
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        var colorRect = args.rowRect.ResizeToLeft(8f);

                        var color = item.item.type switch
                        {
                            HierarchyItem.ItemType.Header => new Color(56f / 255f, 169f / 255f, 255f / 255f),
                            HierarchyItem.ItemType.Separator => new Color(41f / 255f, 41f / 255f, 41f / 255f),
                            _ => new Color(207f / 255f, 207f / 255f, 207f / 255f),
                        };

                        new GUIStyle(EditorStyles.label)
                            .WithBackgroundColor(color)
                            .Draw(colorRect, new GUIContent(string.Empty, ""), false, false, false, false);

                        qGUIEditorUtility.BorderAround(colorRect);

                        qGUIEditorUtility.HorizontalLine(args.rowRect.ResizeToBottom(0f));
                    }

                    break;
                default:
                    base.RowGUI(args);
                    break;
            }

            customFoldoutYOffset = (args.rowRect.height - 22f) / 2f;
        }

        protected override void KeyEvent()
        {
            if (Event.current.keyCode == KeyCode.Delete)
                DeleteSelection();

            base.KeyEvent();
        }

        void DeleteSelection()
        {
            if (window.asset == null) return;

            var selection = GetSelection()
                .Select(x => FindItem(x, rootItem))
                .Where(x => x is Item)
                .Select(x => x as Item);

            foreach (var item in selection)
            {
                window.asset.items.Remove(item.item);
            }

            window.SetAssetDirty();
            Reload();
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("itemIDs", args.draggedItemIDs);
            DragAndDrop.SetGenericData("tree", this);
            DragAndDrop.StartDrag(FindItem(args.draggedItemIDs[0], rootItem).displayName);
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            if (!(args.draggedItem is Item item))
                return false;

            return true;
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (!(DragAndDrop.GetGenericData("tree") is MappingLayoutWindowTree sourceTree) ||
                sourceTree != this)
                return DragAndDropVisualMode.Rejected;

            IList<int> itemID = (IList<int>)DragAndDrop.GetGenericData("itemIDs");

            var items = itemID
                .Select(x => FindItem(x, rootItem) as Item);

            if (items.Any(x => x == null))
                return DragAndDropVisualMode.Rejected;

            items = items
                .OrderBy(x => window.asset.items.IndexOf(x.item))
                .Reverse();

            if (args.performDrop)
            {
                var targetItem = GetDraggedItemTarget(args);

                int targetIndex = targetItem is Item targetHierarchyItem ?
                    window.asset.items.IndexOf(targetHierarchyItem.item) :
                    window.asset.items.Count;

                switch (args.dragAndDropPosition)
                {
                    case DragAndDropPosition.UponItem:
                        targetIndex++;
                        break;
                }

                foreach (var item in items)
                {
                    var itemIndex = window.asset.items.IndexOf(item.item);
                    Move(itemIndex, targetIndex);
                    targetIndex = window.asset.items.IndexOf(item.item);
                }

                SetSelection(itemID);
                window.SetAssetDirty();
            }

            Reload();
            return DragAndDropVisualMode.Move;
        }

        TreeViewItem GetDraggedItemTarget(DragAndDropArgs args)
        {
            if (!args.parentItem.children.IndexInRange(args.insertAtIndex))
                return args.parentItem;

            return args.parentItem.children[args.insertAtIndex];
        }

        void Move(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                return;

            var item = window.asset.items[oldIndex];
            window.asset.items[oldIndex] = null;
            window.asset.items.Insert(newIndex, item);
            window.asset.items.RemoveAt(newIndex < oldIndex ? oldIndex + 1 : oldIndex);
        }

        class Item : TreeViewItem
        {
            public Item(HierarchyItem item)
            {
                this.item = item;
                id = item.guid.GetHashCode();
                depth = 0;

                displayName = item.type switch
                {
                    HierarchyItem.ItemType.Normal => item.id,
                    _ => item.displayText,
                };
            }

            public HierarchyItem item;
        }
    }
}
