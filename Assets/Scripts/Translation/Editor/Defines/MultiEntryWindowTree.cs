using qASIC.EditorTools;
using qASIC.Input.Prompts.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using qASIC;
using System.Linq;
using Project.Translation.Defines;
using static Project.Translation.Defines.MultiEntryTranslationDefines;

namespace Project.Editor.Translation.Defines
{
    internal class MultiEntryWindowTree : TreeView
    {
        const float ADD_REMOVE_BUTTON_WIDTH = 18f;

        MultiEntryWindow window;

        public MultiEntryWindowTree(TreeViewState state, MultiColumnHeader multiColumnHeader, MultiEntryWindow window) : base(state, multiColumnHeader)
        {
            this.window = window;
            rowHeight = 48f;
            showAlternatingRowBackgrounds = true;
            showBorder = true;

            Reload();
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(""),
                    width = EditorGUIUtility.singleLineHeight,
                    maxWidth = EditorGUIUtility.singleLineHeight,
                    minWidth = EditorGUIUtility.singleLineHeight,
                    autoResize = false,
                    canSort = false,
                    allowToggleVisibility = false,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Line ID"),
                    width = 100f,
                    autoResize = true,
                    canSort = false,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Define Id"),
                    width = 200f,
                    autoResize = true,
                    canSort = false,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Auto Name"),
                    width = 72f,
                    maxWidth = 72f,
                    minWidth = 72f,
                    autoResize = false,
                    canSort = false,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Display Name"),
                    width = 200f,
                    autoResize = true,
                    allowToggleVisibility = true,
                    canSort = false,
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(""),
                    width = ADD_REMOVE_BUTTON_WIDTH,
                    maxWidth = ADD_REMOVE_BUTTON_WIDTH,
                    minWidth = ADD_REMOVE_BUTTON_WIDTH,
                    autoResize = false,
                    canSort = false,
                    allowToggleVisibility = false,
                },
            };

            return new MultiColumnHeaderState(columns);
        }

        protected override bool CanMultiSelect(TreeViewItem item) =>
            false;

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem(0, -1);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>();
            rows.Clear();

            foreach (var line in window.asset.lines)
            {
                var item = new LineItem(line);
                rows.Add(item);
                root.AddChild(item);

                foreach (var define in line.defines)
                {
                    var defineItem = new DefineItem(item, define);
                    root.AddChild(defineItem);
                    rows.Add(defineItem);
                }
            }

            return rows;
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            switch (item)
            {
                case DefineItem:
                    return 24f;
                default:
                    return base.GetCustomRowHeight(row, item);
            }
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            switch (args.item)
            {
                case LineItem lineItem:
                    using (new EditorChangeChecker.ChangeCheck(window.SetAssetDirty))
                    {
                        for (int i = 0; i < args.GetNumVisibleColumns(); i++)
                            LineCellGUI(args.GetCellRect(i), lineItem, args.GetColumn(i), ref args);
                    }

                    var bottomLineRect = args.rowRect
                        .BorderRight(ADD_REMOVE_BUTTON_WIDTH + EditorGUIUtility.standardVerticalSpacing * 2f)
                        .ResizeToBottom(EditorGUIUtility.singleLineHeight)
                        .MoveY(-EditorGUIUtility.standardVerticalSpacing);

                    var addDefineRect = bottomLineRect
                        .ResizeToRight(80f);

                    if (GUI.Button(addDefineRect, "Add Define"))
                    {
                        CreateDefine(lineItem);
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        var colorRect = args.rowRect.ResizeToLeft(8f);
                        new GUIStyle(EditorStyles.label).WithBackgroundColor(new Color(3f / 255f, 252f / 255f, 161f / 255f)).Draw(colorRect, GUIContent.none, false, false, false, false);
                        qGUIEditorUtility.VerticalLine(colorRect.ResizeToRight(0f));
                        qGUIEditorUtility.HorizontalLine(args.rowRect.ResizeToTop(0f), 4f);
                        qGUIEditorUtility.HorizontalLine(args.rowRect.ResizeToBottom(0f));
                    }
                    break;
                case DefineItem defineItem:
                    using (new EditorChangeChecker.ChangeCheck(window.SetAssetDirty))
                    {
                        for (int i = 0; i < args.GetNumVisibleColumns(); i++)
                            DefineCellGUI(args.GetCellRect(i), defineItem, args.GetColumn(i), ref args);
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        qGUIEditorUtility.HorizontalLine(args.rowRect.ResizeToBottom(0f));
                    }

                    break;
                default:
                    base.RowGUI(args);
                    break;
            }
        }

        void LineCellGUI(Rect cellRect, LineItem item, int columnIndex, ref RowGUIArgs args)
        {
            switch (columnIndex)
            {
                case 1:
                    item.line.lineId = EditorGUI.DelayedTextField(cellRect
                        .ResizeHeightToCenter(EditorGUIUtility.singleLineHeight)
                        .Border(EditorGUIUtility.standardVerticalSpacing, 0f), item.line.lineId);
                    break;
                case 5:
                    var minusRect = cellRect
                        .ResizeWidthToCenter(ADD_REMOVE_BUTTON_WIDTH)
                        .ResizeHeightToCenter(ADD_REMOVE_BUTTON_WIDTH);

                    if (GUI.Button(minusRect, new GUIContent(qGUIEditorUtility.MinusIcon), EditorStyles.label))
                        DeleteLine(item);
                    break;
            }
        }

        void DefineCellGUI(Rect cellRect, DefineItem item, int columnIndex, ref RowGUIArgs args)
        {
            switch (columnIndex)
            {
                //Id
                case 2:
                    var idRect = cellRect
                        .ResizeHeightToCenter(EditorGUIUtility.singleLineHeight)
                        .Border(EditorGUIUtility.standardVerticalSpacing, 0f);

                    item.define.id = EditorGUI.DelayedTextField(idRect, item.define.id);
                    break;
                //Auto name
                case 3:
                    var autoRect = cellRect
                        .ResizeHeightToCenter(EditorGUIUtility.singleLineHeight)
                        .ResizeWidthToCenter(EditorGUIUtility.singleLineHeight);

                    item.define.autoDisplayName = EditorGUI.Toggle(autoRect, item.define.autoDisplayName);
                    break;
                //Display name
                case 4:
                    var nameRect = cellRect
                        .ResizeHeightToCenter(EditorGUIUtility.singleLineHeight)
                        .Border(EditorGUIUtility.standardVerticalSpacing, 0f);

                    switch (item.define.autoDisplayName)
                    {
                        case true:
                            using (new EditorGUI.DisabledGroupScope(true))
                                EditorGUI.TextField(nameRect, item.define.displayName);
                            break;
                        case false:
                            item.define.displayName = EditorGUI.DelayedTextField(nameRect, item.define.displayName);
                            break;
                    }
                    break;
                //Utility
                case 5:
                    var removeRect = cellRect
                        .ResizeHeightToCenter(ADD_REMOVE_BUTTON_WIDTH)
                        .ResizeWidthToCenter(ADD_REMOVE_BUTTON_WIDTH);

                    if (GUI.Button(removeRect, new GUIContent(qGUIEditorUtility.MinusIcon), EditorStyles.label))
                        DeleteDefine(item);
                    break;
            }
        }

        public void CreateLine()
        {
            if (window.asset == null) return;
            window.asset.lines.Add(new Line()
            {
                defines = new List<DefineField>(new DefineField[] { new DefineField() }),
            });

            Reload();
        }

        public void DeleteLine(LineItem line)
        {
            if (window.asset == null) return;
            window.asset.lines.Remove(line.line);
            Reload();
        }

        public void CreateDefine(LineItem line) =>
            DeleteDefine(line.line);

        public void DeleteDefine(Line line)
        {
            line.defines.Add(new DefineField());
            Reload();
        }

        public void DeleteDefine(DefineItem item)
        {
            item.line.line.defines.Remove(item.define);

            Reload();
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count == 1)
            {
                var item = FindItem(selectedIds[0], rootItem);
                switch (item)
                {
                    case LineItem lineItem:
                        window.inspector.SelectedItem = lineItem.line;
                        break;
                    case DefineItem defineItem:
                        window.inspector.SelectedItem = new MultiEntryWindowInspector.DefineFieldContext()
                        {
                            defineField = defineItem.define,
                            line = defineItem.line.line,
                        };
                        break;
                }
            }

            base.SelectionChanged(selectedIds);
        }

        internal class LineItem : TreeViewItem
        {
            public LineItem()
            {
                depth = 0;
            }

            public LineItem(Line line)
            {
                id = line.guid.GetHashCode();
                this.line = line;
            }

            public Line line;
        }

        internal class DefineItem : TreeViewItem
        {
            public DefineItem()
            {
                depth = 0;
            }

            public DefineItem(LineItem line, DefineField define)
            {
                this.line = line;
                this.define = define;
                id = define.guid.GetHashCode();
            }

            public LineItem line;
            public DefineField define;
        }
    }
}