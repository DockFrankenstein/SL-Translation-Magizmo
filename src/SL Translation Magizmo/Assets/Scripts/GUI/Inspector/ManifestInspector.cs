using System.Linq;
using System;
using Project.GUI.Hierarchy;
using UnityEngine.UIElements;
using Project.UI;
using Project.Translation.Data;
using Project.Translation.Mapping.Manifest;
using Project.Translation.Mapping;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using Project.Undo;

namespace Project.GUI.Inspector
{
    public sealed class ManifestInspector : InspectorDisplayPanel
    {
        public const string MANIFEST_ITEM_ID = "manifest";

        public override bool ShouldOpen(IApplicationObject obj) =>
            obj is HierarchyItem item &&
            item.id == "manifest";

        ManifestMappingBase _manifestMapping;

        List<TargetField> mappedFields = new List<TargetField>();

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Initialize()
        {
            base.Initialize();

            _manifestMapping = manager.CurrentVersion.containers
                .Where(x => x is ManifestMappingBase)
                .Select(x => x as ManifestMappingBase)
                .FirstOrDefault();

            var file = manager.File;

            if (_manifestMapping != null)
            {
                var fields = _manifestMapping.DataType.GetFields();

                foreach (var item in fields)
                {
                    var attr = ((MappedFieldNameAttribute[])item.GetCustomAttributes(typeof(MappedFieldNameAttribute), false))
                        .FirstOrDefault();

                    if (attr == null)
                        continue;

                    TargetField field = null;

                    if (item.FieldType == typeof(string[]))
                        field = new TargetFieldStringArray();

                    if (item.FieldType == typeof(string))
                        field = new TargetFieldString();

                    if (field == null) continue;

                    field.field = item;
                    field.attr = attr;

                    var mappedField = attr.GetMappedField();

                    if (!file.Entries.ContainsKey(mappedField.id))
                        file.Entries.Add(mappedField.id, new SaveFile.EntryData(mappedField));

                    field.inspector = this;
                    field.undo = undo;
                    field.entry = file.Entries[mappedField.id];
                    mappedFields.Add(field);
                    Container.Add(field.UiItem);
                    field.LabelText = mappedField.GetFinalName();
                    field.LoadValue(field.entry.content);
                }
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            foreach (var item in mappedFields)
                Container.Remove(item.UiItem);

            mappedFields.Clear();
        }

        abstract class TargetField
        {
            public FieldInfo field;
            public MappedFieldNameAttribute attr;
            public SaveFile.EntryData entry;
            public ManifestInspector inspector;

            public UndoManager undo;

            public abstract VisualElement UiItem { get; }
            public abstract string LabelText { get; set; }

            public abstract void LoadValue(string value);
        }

        abstract class TargetFieldSingleObject<T> : TargetField
        {
            public TargetFieldSingleObject()
            {
                Field.RegisterValueChangedCallback(args =>
                {
                    if (args.target != Field) return;

                    entry.content = Field.value.ToString();

                    if (!undo.IsLatest(_contentUndo))
                    {
                        var tempEntry = entry;
                        _contentUndo = new UndoStep<T>(args.previousValue, a => tempEntry.content = a.ToString())
                        {
                            Context = inspector.inspector.SelectedObject,
                        };

                        undo.AddStep(_contentUndo, inspector);
                    }

                    _contentUndo.newValue = Field.value;
                    undo.UpdateLatestStep(inspector);
                });
            }

            UndoStep<T> _contentUndo;

            public abstract BaseField<T> Field { get; }
            public override VisualElement UiItem => Field;
            public override string LabelText 
            { 
                get => Field.label; 
                set => Field.label = value; 
            }

            public override void LoadValue(string value)
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null && converter.IsValid(value))
                    Field.SetValueWithoutNotify((T)converter.ConvertFromString(value));
            }
        }

        class TargetFieldString : TargetFieldSingleObject<string>
        {
            TextField _field = new TextField();
            public override BaseField<string> Field => _field;
        }

        abstract class TargetFieldArray<T> : TargetField
        {
            public TargetFieldArray()
            {
                _list.MakeItem += () =>
                    MakeItem().WithMargin(4f);

                _list.OnChanged += () =>
                {
                    if (entry != null)
                    {
                        entry.content = _list.Source
                            .Select(x => x?.ToString())
                            .ToEntryContent();

                        if (_lastUndo != null)
                        {
                            _lastUndo.newValue = entry.content;
                            undo.UpdateLatestStep(inspector);
                        }
                    }
                };

                _list.OnUndoEvent += () =>
                {
                    if (entry == null)
                        return;

                    _lastUndo = new UndoStep<string>(entry.content, a => entry.content = a.ToString())
                    {
                        Context = inspector.inspector.SelectedObject,
                    };

                    undo.AddStep(_lastUndo, inspector);
                };
            }

            UndoStep<string> _lastUndo;

            AppReorderableList<T> _list = new AppReorderableList<T>(new ListView()
            {
                showAddRemoveFooter = true,
                showFoldoutHeader = true,
                showBoundCollectionSize = false,
                fixedItemHeight = 50f,
                showBorder = true,
                reorderMode = ListViewReorderMode.Animated,
                reorderable = true,
            });

            public override VisualElement UiItem => _list.List;
            public override string LabelText 
            { 
                get => _list.List.headerTitle; 
                set => _list.List.headerTitle = value;
            }

            public override void LoadValue(string value)
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter == null) return;

                var valueArray = value.EntryContentToArray();

                foreach (var item in valueArray)
                    if (converter.IsValid(item))
                        _list.Source.Add((T)converter.ConvertFromString(item));

                _list.List.RefreshItems();
            }

            public abstract BaseField<T> MakeItem();
        }

        class TargetFieldStringArray : TargetFieldArray<string> 
        {
            public override BaseField<string> MakeItem() =>
                new TextField();
        }
    }
}