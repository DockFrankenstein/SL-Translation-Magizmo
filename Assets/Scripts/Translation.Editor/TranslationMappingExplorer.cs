using Project.Translation.Mapping;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using qASIC;
using System.Linq;
using System;
using UnityEditor.IMGUI.Controls;
using qASIC.EditorTools;

namespace Project.Translation.EditorWindows
{
    public class TranslationMappingExplorer : EditorWindow
    {
        [MenuItem("Window/Project/Translations/Mapping Explorer")]
        public static TranslationMappingExplorer OpenWindow()
        {
            TranslationMappingExplorer window = (TranslationMappingExplorer)GetWindow(typeof(TranslationMappingExplorer), false);
            window.titleContent = new GUIContent("Mapping Explorer");
            window.Show();
            return window;
        }

        internal List<MappedField> defines = new List<MappedField>();

        TranslationVersion currentVersion;

        TreeViewState treeState;
        TranslationMappingExplorerTree tree;

        [NonSerialized] bool _init = false;
        void Initialize()
        {
            _init = true;

            if (treeState == null)
                treeState = new TreeViewState();

            if (tree == null)
                tree = new TranslationMappingExplorerTree(treeState, this);

            if (currentVersion == null)
                currentVersion = TranslationProjectSettings.Instance.translationVersions.LastOrDefault();

            RefreshMappings();
            tree.Reload();
        }

        private void OnGUI()
        {
            if (!_init)
                Initialize();

            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                    RefreshMappings();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(currentVersion?.version ?? "NONE", EditorStyles.toolbarButton))
                {
                    GenericMenu menu = new GenericMenu();

                    foreach (var item in TranslationProjectSettings.Instance.translationVersions)
                    {
                        menu.AddItem(item.version, item == currentVersion, () =>
                        {
                            currentVersion = item;
                            RefreshMappings();
                        });
                    }

                    menu.ShowAsContext();
                }

                GUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            tree.searchString = EditorGUILayout.TextField(tree.searchString, EditorStyles.toolbarSearchField);

            qGUIEditorUtility.HorizontalLineLayout();

            DrawTreeView(tree);
        }

        void RefreshMappings()
        {
            defines = currentVersion.GetMappedFields().ToList();
            tree.Reload();
        }

        protected void DrawTreeView(TreeView tree)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            Rect rect = GUILayoutUtility.GetLastRect();
            tree?.OnGUI(rect);
        }
    }
}