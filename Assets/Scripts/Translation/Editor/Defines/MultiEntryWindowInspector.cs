using qASIC.EditorTools;
using Project.Translation.Defines;
using System;
using UnityEditor;
using UnityEngine;

namespace Project.Editor.Translation.Defines
{
    [Serializable]
    public class MultiEntryWindowInspector
    {
        public MultiEntryWindowInspector(MultiEntryWindow window)
        {
            this.window = window;
        }

        public MultiEntryWindow window;

        [SerializeField] Vector2 scrollPosition;

        public object _selectedItem;
        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
            }
        }

        public void OnGUI()
        {
            using (new EditorChangeChecker.ChangeCheck(window.SetAssetDirty))
            {
                using (var scroll = new GUILayout.ScrollViewScope(scrollPosition))
                {
                    scrollPosition = scroll.scrollPosition;

                    switch (SelectedItem)
                    {
                        case MultiEntryTranslationDefines.Line lineItem:
                            LineGUI(lineItem);
                            break;
                        case DefineFieldContext defineItem:
                            DefineGUI(defineItem);
                            break;
                    }

                    EditorGUILayout.Space();
                    GUILayout.FlexibleSpace();
                }
            }
        }

        void LineGUI(MultiEntryTranslationDefines.Line item)
        {
            HeaderGUI("Line");
            item.lineId = EditorGUILayout.DelayedTextField("Line ID", item.lineId);

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Define"))
                window.tree.AddDefine(item);
        }

        void DefineGUI(DefineFieldContext item)
        {
            HeaderGUI("Define Field");
            item.defineField.id = EditorGUILayout.DelayedTextField("ID", item.defineField.id);

            EditorGUILayout.Space();
            GUILayout.Label("Display Name", EditorStyles.boldLabel);
            item.defineField.autoDisplayName = EditorGUILayout.Toggle("Auto Display Name", item.defineField.autoDisplayName);

            using (new EditorGUI.DisabledScope(item.defineField.autoDisplayName))
                item.defineField.displayName = EditorGUILayout.DelayedTextField("Display Name", item.defineField.displayName);
        }

        void HeaderGUI(string itemName)
        {
            GUILayout.Label(itemName, Styles.Header);
            qGUIEditorUtility.HorizontalLineLayout();
        }

        public struct DefineFieldContext
        {
            public DefineField defineField;
            public MultiEntryTranslationDefines.Line line;
        }

        static class Styles
        {
            public static GUIStyle Header => new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter,
            };
        }
    }
}