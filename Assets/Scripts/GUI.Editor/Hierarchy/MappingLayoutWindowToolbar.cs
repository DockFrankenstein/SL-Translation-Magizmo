using Project.GUI.Hierarchy;
using qASIC.EditorTools;
using qASIC.EditorTools.AssetEditor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project.GUI.Editor.Hierarchy
{
    public class MappingLayoutWindowToolbar : AssetEditorToolbar<MappingLayoutWindow, MappingLayout>
    {
        public MappingLayoutWindowToolbar(MappingLayoutWindow window) : base(window)
        {

        }

        protected override void OnLeftGUI()
        {
            if (GUILayout.Button(qGUIEditorUtility.PlusIcon, EditorStyles.toolbarButton))
                window.tree.CreateNewItem();

            if (GUILayout.Button("Generate Items From Version", EditorStyles.toolbarButton) && window.asset != null)
            {
                foreach (var item in window.asset.version.GetMappedFields())
                {
                    if (window.asset.items.Any(x => x.id == item.id)) continue;
                    window.asset.items.Add(new HierarchyItem(item.id));
                    window.SetAssetDirty();
                    window.tree.Reload();
                }
            }
        }

        protected override void OnRightGUI()
        {
            window.tree.searchString = EditorGUILayout.TextField(window.tree.searchString, EditorStyles.toolbarSearchField);

            GUIAutoSaveButton();
            GUISaveButton();
        }
    }
}