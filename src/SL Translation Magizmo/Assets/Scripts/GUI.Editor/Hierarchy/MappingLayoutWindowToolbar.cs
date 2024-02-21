using Project.GUI.Hierarchy;
using qASIC.EditorTools;
using qASIC.EditorTools.AssetEditor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using qASIC;

namespace Project.GUI.Editor.Hierarchy
{
    public class MappingLayoutWindowToolbar : AssetEditorToolbar<MappingLayoutWindow, MappingLayout>
    {
        public MappingLayoutWindowToolbar(MappingLayoutWindow window) : base(window)
        {

        }

        Rect r_sort;

        protected override void OnLeftGUI()
        {
            if (GUILayout.Button(qGUIEditorUtility.PlusIcon, EditorStyles.toolbarButton))
                window.tree.CreateNewItem();

            if (GUILayout.Button("Generate Items From Version", EditorStyles.toolbarButton) && window.asset != null)
            {
                var newItems = new List<HierarchyItem>();
                foreach (var item in window.asset.version.GetMappedFields())
                {
                    if (item.mappingContainer.Hide) continue;
                    if (window.asset.items.Any(x => x.id == item.id)) continue;
                    var hierarchyItem = new HierarchyItem(item.id);
                    newItems.Add(hierarchyItem);
                    window.asset.items.Add(hierarchyItem);
                }

                window.SetAssetDirty();
                window.tree.Reload();
                window.tree.SetSelection(newItems.Select(x => x.guid.GetHashCode()).ToList());
            }

            DisplayMenu("Sort", ref r_sort, menu =>
            {
                menu.AddToggableItem("Sort Selection/By ID", false, () =>
                {
                    window.tree.SortSelected(x => x.id);
                }, window.tree.GetSelection().Count > 0);

                menu.AddToggableItem("Sort Selection/By Name", false, () =>
                {
                    window.tree.SortSelected(x => x.displayText);
                }, window.tree.GetSelection().Count > 0);
            });
        }

        protected override void OnRightGUI()
        {
            window.tree.searchString = EditorGUILayout.TextField(window.tree.searchString, EditorStyles.toolbarSearchField);

            GUIAutoSaveButton();
            GUISaveButton();
        }
    }
}