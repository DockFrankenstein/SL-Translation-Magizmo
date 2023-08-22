using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Callbacks;
using qASIC.Input.Map;
using qASIC.EditorTools.AssetEditor;
using qASIC.EditorTools;
using UnityEditor.IMGUI.Controls;

namespace qASIC.Input.Prompts.Internal
{
    public class PromptLibraryWindow : AssetEditorWindow<PromptLibraryWindow, PromptLibrary>
    {
        public override string WindowTitle => "Prompt Library Window";
        public override string PrefsKeyPrefix => "cablebox_prompt";
        public override Vector2 MinWindowSize => new Vector2(800f, 300f);

        internal PromptLibraryWindowToolbar toolbar;
        [SerializeField] internal PromptLibraryWindowInspector inspector = new PromptLibraryWindowInspector();
        
        internal PromptsVariantTreeView variantTree;
        [SerializeField] TreeViewState variantTreeState;

        internal PromptTreeView promptTree;
        [SerializeField] TreeViewState promptTreeState;
        [SerializeField] MultiColumnHeaderState promptTreeHeaderState;

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!(obj is PromptLibrary library))
                return false;

            OpenAsset(library);
            return true;
        }

        [MenuItem("Window/qASIC/Input/Prompt Library Editor")]
        static PromptLibraryWindow MenuOpenWindow() =>
            CreateWindow();

        protected override void Initialize()
        {
            base.Initialize();

            //Variant Tree
            variantTreeState = new TreeViewState();
            variantTree = new PromptsVariantTreeView(variantTreeState, this);

            //Prompt Tree
            bool firstPromptHeaderStateInitialization = promptTreeHeaderState == null;
            var newPromptTreeHeaderState = PromptTreeView.CreateDefaultMultiColumnHeaderState();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(promptTreeHeaderState, newPromptTreeHeaderState))
                MultiColumnHeaderState.OverwriteSerializedFields(promptTreeHeaderState, newPromptTreeHeaderState);

            promptTreeHeaderState = newPromptTreeHeaderState;

            var promptTreeHeader = new MultiColumnHeader(newPromptTreeHeaderState);
            if (firstPromptHeaderStateInitialization)
                promptTreeHeader.ResizeToFit();

            promptTreeState = new TreeViewState();
            promptTree = new PromptTreeView(promptTreeState, promptTreeHeader, this);

            //Others
            toolbar = new PromptLibraryWindowToolbar(this);
            inspector.Initialize(this); 
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            toolbar.OnGUI();

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(GUILayout.Width(200f)))
                {
                    DrawTreeView(variantTree);
                }

                qGUIEditorUtility.VerticalLineLayout();

                DrawTreeView(promptTree);

                qGUIEditorUtility.VerticalLineLayout();

                using (new GUILayout.VerticalScope(GUILayout.Width(350f)))
                {
                    inspector.OnGUI();
                }
            }
        }

        public override void Save()
        {
            base.Save();

            var relativePath = AssetDatabase.GetAssetPath(asset);
            var path = $"{Application.dataPath}/{relativePath.Remove(0, 7)}";
            Files.FileManager.SaveFileJSON(path, asset, true);
            AssetDatabase.ImportAsset(relativePath);
        }
    }
}