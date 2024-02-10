using qASIC.EditorTools;
using Project.Translation.Defines;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;

using static Project.Translation.Defines.MultiEntryTranslationDefines;

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
                        case Line lineItem:
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

        void LineGUI(Line item)
        {
            HeaderGUI("Line");
            item.lineId = EditorGUILayout.DelayedTextField("Line ID", item.lineId);

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Define"))
                window.tree.CreateLine(item);
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

            EditorGUILayout.Space();

            item.defineField.addToList = EditorGUILayout.Toggle("Add To List", item.defineField.addToList);
        }

        internal void ReloadSelection()
        {
            switch (SelectedItem)
            {
                case Line line:
                    var newLine = window.asset.lines
                        .Where(x => x.guid == line.guid)
                        .FirstOrDefault();

                    if (newLine == null)
                    {
                        SelectedItem = null;
                        break;
                    }

                    _selectedItem = newLine;
                    break;
                case DefineFieldContext define:
                    var newDefine = window.asset.lines
                        .SelectMany(x => x.defines)
                        .Where(x => x.guid == define.defineField.guid)
                        .FirstOrDefault();

                    if (newDefine == null)
                    {
                        SelectedItem = null;
                        break;
                    }

                    define.defineField = newDefine;
                    define.line = window.asset.lines
                        .Where(x => x.defines.Contains(newDefine))
                        .First();

                    _selectedItem = define;
                    break;
            }
        }

        void HeaderGUI(string itemName)
        {
            GUILayout.Label(itemName, Styles.Header);
            qGUIEditorUtility.HorizontalLineLayout();
        }

        public struct DefineFieldContext
        {
            public DefineField defineField;
            public Line line;
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