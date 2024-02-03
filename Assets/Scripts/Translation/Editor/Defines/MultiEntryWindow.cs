using Project.Translation.Defines;
using qASIC.EditorTools.AssetEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using Unity.VisualScripting;
using qASIC.EditorTools;
using qASIC;
using qASIC.Input.Prompts;
using UnityEditor.Callbacks;
using qASIC.Input.Map.Internal;
using UnityEditor.ShortcutManagement;

namespace Project.Editor.Translation.Defines
{
    public class MultiEntryWindow : AssetEditorWindow<MultiEntryWindow, MultiEntryTranslationDefines>, IHasCustomMenu
    {
        public override string WindowTitle => asset?.name ?? "Multi Entry Asset Editor";

        public override string PrefsKeyPrefix => "sltm_multientry";

        internal MultiEntryWindowTree tree;
        [SerializeField] TreeViewState treeState;
        [SerializeField] MultiColumnHeaderState treeHeaderState;

        internal MultiEntryWindowToolbar toolbar;
        [SerializeField] internal MultiEntryWindowInspector inspector;

        #region Shortcuts
        [Shortcut("Multi Entry File Editor/Save", typeof(MultiEntryWindow), KeyCode.S, ShortcutModifiers.Alt)]
        private static void Sh_Save(ShortcutArguments args)
        {
            GetWindow().Save();
        }

        [Shortcut("Multi Entry File Editor/Create New Line", typeof(MultiEntryWindow), KeyCode.W, ShortcutModifiers.Alt)]
        private static void Sh_NewLine(ShortcutArguments args)
        {
            GetWindow().tree.CreateLine();
        }
        #endregion

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!(obj is MultiEntryTranslationDefines asset))
                return false;

            OpenAsset(asset);
            return true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            bool firstHeaderInit = treeHeaderState == null;
            var newHeaderState = MultiEntryWindowTree.CreateDefaultMultiColumnHeaderState();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(treeHeaderState, newHeaderState))
                MultiColumnHeaderState.OverwriteSerializedFields(treeHeaderState, newHeaderState);

            treeHeaderState = newHeaderState;

            var treeHeader = new MultiColumnHeader(newHeaderState);
            if (firstHeaderInit)
                treeHeader.ResizeToFit();

            treeState = new TreeViewState();
            tree = new MultiEntryWindowTree(treeState, treeHeader, this);

            toolbar = new MultiEntryWindowToolbar(this);
            inspector = new MultiEntryWindowInspector(this);
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
            using (new GUILayout.HorizontalScope())
            {
                DrawTreeView(tree);

                qGUIEditorUtility.VerticalLineLayout();

                using (new GUILayout.VerticalScope(GUILayout.Width(350f)))
                {
                    inspector.OnGUI();
                }
            }
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem("Regenerate guid", false, () =>
            {
                if (asset == null) return;

                foreach (var line in asset.lines)
                {
                    line.guid = System.Guid.NewGuid().ToString();
                    foreach (var define in line.defines)
                        define.guid = System.Guid.NewGuid().ToString();
                }

                Save();
            });
        }
    }
}