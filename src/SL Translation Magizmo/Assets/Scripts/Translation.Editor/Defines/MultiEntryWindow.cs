using Project.Translation.Mapping;
using qASIC.EditorTools.AssetEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using qASIC.EditorTools;
using qASIC;
using UnityEditor.Callbacks;
using UnityEditor.ShortcutManagement;
using System;

namespace Project.Editor.Translation.Defines
{
    public class MultiEntryWindow : AssetEditorWindow<MultiEntryWindow, MultiEntryTranslationMapping>, IHasCustomMenu
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

        public bool SingleLineMode =>
            asset != null &&
            !asset.useSeparationCharacter &&
            Prefs_UseSingleLine;

        #region Prefs
        bool? prefs_startLineCountFromOne = null;
        public bool Prefs_StartLineCountFromOne
        {
            get
            {
                if (prefs_startLineCountFromOne == null)
                    prefs_startLineCountFromOne = EditorPrefs.GetBool("sltm_mew_linefromone", false);

                return prefs_startLineCountFromOne ?? false;
            }
            set
            {
                prefs_startLineCountFromOne = value;
                EditorPrefs.SetBool("sltm_mew_linefromone", value);
            }
        }

        bool? prefs_useSingleLine = null;
        public bool Prefs_UseSingleLine
        {
            get
            {
                if (prefs_useSingleLine == null)
                    prefs_useSingleLine = EditorPrefs.GetBool("sltm_mew_singleline", true);

                return prefs_useSingleLine ?? true;
            }
            set
            {
                prefs_useSingleLine = value;
                EditorPrefs.SetBool("sltm_mew_singleline", value);
                tree.Reload();
            }
        }
        #endregion

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!(obj is MultiEntryTranslationMapping asset))
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

        public Action OnAssetReload;

        public override void Save()
        {
            base.Save();

            var relativePath = AssetDatabase.GetAssetPath(asset);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            qASIC.Files.FileManager.SaveFileJSON(path, asset, true);
            AssetDatabase.ImportAsset(relativePath);

            OnAssetReload?.Invoke();
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
                    foreach (var define in line.fields)
                        define.guid = System.Guid.NewGuid().ToString();
                }

                Save();
            });
        }
    }
}