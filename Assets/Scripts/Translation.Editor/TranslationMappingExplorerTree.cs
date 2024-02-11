using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using qASIC.EditorTools;
using System.Linq;
using qASIC;
using Project.Translation.Defines;
using UnityEditor;

namespace Project.Translation.EditorWindows
{
    internal class TranslationMappingExplorerTree : TreeView
    {
        public TranslationMappingExplorerTree(TreeViewState state, TranslationMappingExplorer window) : base(state)
        {
            this.window = window;
            showAlternatingRowBackgrounds = true;
        }

        TranslationMappingExplorer window;

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            bool isSearching = !string.IsNullOrWhiteSpace(searchString);

            var rows = base.BuildRows(root) ?? new List<TreeViewItem>();
            rows.Clear();

            var items = window.defines
                .Where(x => x != null)
                .OrderBy(x => x.id)
                .AsEnumerable();

            if (isSearching)
                items = qGUIEditorUtility.SortSearchList(items, x => x.id ?? string.Empty, searchString);

            foreach (var item in items)
            {
                var treeItem = new Item(item);
                root.AddChild(treeItem);
                rows.Add(treeItem);
            }

            return rows;
        }

        protected override void ContextClickedItem(int id)
        {
            if (!(FindItem(id, rootItem) is Item item)) return;

            GenericMenu menu = new GenericMenu();

            menu.AddItem("Copy ID", false, () => GUIUtility.systemCopyBuffer = item.field.id);

            menu.AddSeparator("");

            menu.AddToggableItem($"File: {item?.field?.definesBase?.name ?? "NULL"}", false, () => { }, false);

            menu.ShowAsContext();
        }

        class Item : TreeViewItem
        {
            public Item(DefineField field)
            {
                this.field = field;
                depth = 0;
                displayName = field.id;
                id = field.guid.GetHashCode();
            }

            public DefineField field;
        }
    }
}