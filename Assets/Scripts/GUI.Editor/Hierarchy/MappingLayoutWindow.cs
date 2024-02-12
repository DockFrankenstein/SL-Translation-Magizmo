using Project.GUI.Hierarchy;
using qASIC.EditorTools.AssetEditor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Project.GUI.Editor.Hierarchy
{
    public class MappingLayoutWindow : AssetEditorWindow<MappingLayoutWindow, MappingLayout>
    {
        public override string WindowTitle => "Mapping Layout";

        public override string PrefsKeyPrefix => "mapping_layout";

        internal MappingLayoutWindowToolbar toolbar;

        internal MappingLayoutWindowTree tree;
        [SerializeField] TreeViewState treeState;

        #region Shortcuts
        [Shortcut("Maping Layout Window/Save", typeof(MappingLayoutWindow), KeyCode.S, ShortcutModifiers.Alt)]
        private static void Sh_Save(ShortcutArguments args)
        {
            GetWindow().Save();
        }
        #endregion

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!(obj is MappingLayout asset))
                return false;

            OpenAsset(asset);
            return true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            treeState = new TreeViewState();
            tree = new MappingLayoutWindowTree(treeState, this);

            toolbar = new MappingLayoutWindowToolbar(this);
        }

        public override void Save()
        {
            base.Save();

            var relativePath = AssetDatabase.GetAssetPath(asset);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            qASIC.Files.FileManager.SaveFileJSON(path, asset, true);
            AssetDatabase.ImportAsset(relativePath);
            tree.Reload();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            toolbar.OnGUI();
            DrawTreeView(tree);
        }
    }
}