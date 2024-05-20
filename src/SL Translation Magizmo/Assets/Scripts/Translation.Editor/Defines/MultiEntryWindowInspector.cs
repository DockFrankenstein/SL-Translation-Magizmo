using qASIC.EditorTools;
using Project.Translation.Mapping;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;
using qASIC;

using static Project.Translation.Mapping.MultiEntryTranslationMapping;
using Project.Editor.Translation.Defines.Tools;

namespace Project.Editor.Translation.Defines
{
    [Serializable]
    public class MultiEntryWindowInspector
    {
        public MultiEntryWindowInspector(MultiEntryWindow window)
        {
            this.window = window;
            window.OnAssetReload += ReloadSelection;
        }

        public MultiEntryWindow window;

        [SerializeField] Vector2 scrollPosition;

        private object _selectedItem;
        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                init = true;
            }
        }

        bool init = true;

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
                            FieldGUI(defineItem);
                            break;
                        case MultiEntryWindowTool tool:
                            ToolGUI(tool);
                            break;
                    }

                    EditorGUILayout.Space();
                    GUILayout.FlexibleSpace();
                }

                init = false;
            }
        }

        void LineGUI(Line item)
        {
            HeaderGUI("Line");
            item.lineId = EditorGUILayout.DelayedTextField("Line ID", item.lineId);

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Define"))
                window.tree.CreateLine(item);

            if (window.SingleLineMode && item.fields.Count > 0)
            {
                EditorGUILayout.Space();
                qGUIEditorUtility.HorizontalLineLayout();

                FieldGUI(new DefineFieldContext()
                {
                    line = item,
                    field = item.fields[0],
                });
            }   
        }

        GUIContent c_dynamicValues = new GUIContent("Dynamic Values", "List of avaliable tags representing dynamic values (e.g. [name] or {0}).");

        ReorderableList l_dynamicValues;

        void FieldGUI(DefineFieldContext item)
        {
            if (init)
            {
                l_dynamicValues = new ReorderableList(item.field.dynamicValues, typeof(string), true, true, true, true)
                {
                    elementHeight = EditorGUIUtility.singleLineHeight * 4f + 
                        EditorGUIUtility.standardVerticalSpacing * 4f,
                };
                l_dynamicValues.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect = rect.BorderBottom(EditorGUIUtility.standardVerticalSpacing);

                    var tagRect = rect.ResizeToTop(EditorGUIUtility.singleLineHeight);
                    var descriptionLabelRect = tagRect.NextLine();
                    var descriptionRect = rect.BorderTop(EditorGUIUtility.singleLineHeight * 2f + 
                        EditorGUIUtility.standardVerticalSpacing * 2f);

                    var val = item.field.dynamicValues[index];

                    val.tag = EditorGUI.DelayedTextField(tagRect, "Tag", val.tag);
                    EditorGUI.LabelField(descriptionLabelRect, "Description");
                    val.description = EditorGUI.DelayedTextField(descriptionRect, val.description, EditorStyles.textArea);
                };

                l_dynamicValues.drawHeaderCallback += (rect) => EditorGUI.LabelField(rect, c_dynamicValues);
            }

            HeaderGUI("Mapped Field");
            item.field.id = EditorGUILayout.DelayedTextField("ID", item.field.id);

            EditorGUILayout.Space();
            GUILayout.Label("Display Name", EditorStyles.boldLabel);
            item.field.autoDisplayName = EditorGUILayout.Toggle("Auto Display Name", item.field.autoDisplayName);

            using (new EditorGUI.DisabledScope(item.field.autoDisplayName))
                item.field.displayName = EditorGUILayout.DelayedTextField("Display Name", item.field.displayName);

            EditorGUILayout.Space();

            item.field.addToList = EditorGUILayout.Toggle("Add To List", item.field.addToList);
            item.field.notYetAddedToSL = EditorGUILayout.Toggle("Not Yet Added To SL", item.field.notYetAddedToSL);

            l_dynamicValues.DoLayoutList();
        }

        void ToolGUI(MultiEntryWindowTool tool)
        {
            using (new EditorChangeChecker.ChangeCheckPause())
            {
                HeaderGUI(tool.Name);

                if (init)
                {
                    tool.Window = window;
                    tool.Initialize();
                }

                tool.OnGUI();
            }
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
                        .SelectMany(x => x.fields)
                        .Where(x => x.guid == define.field.guid)
                        .FirstOrDefault();

                    if (newDefine == null)
                    {
                        SelectedItem = null;
                        break;
                    }

                    define.field = newDefine;
                    define.line = window.asset.lines
                        .Where(x => x.fields.Contains(newDefine))
                        .First();

                    _selectedItem = define;
                    break;
            }

            init = true;
        }

        void HeaderGUI(string itemName)
        {
            GUILayout.Label(itemName, Styles.Header);
            qGUIEditorUtility.HorizontalLineLayout();
        }

        public struct DefineFieldContext
        {
            public MappedField field;
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