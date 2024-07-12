using Project.Translation.Mapping;
using Project.Translation.Data;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;
using System;
using Project.Text;
using Project.Undo;

namespace Project.GUI.Inspector
{
    public class SingleEntryInspector : InspectorDisplayPanel
    {
        public override bool ShouldOpen(IApplicationObject obj) =>
            obj is SaveFile.EntryData data &&
            manager.CurrentVersion.MappedFields.TryGetValue(data.entryId, out MappedField field) &&
            field.mappingContainer is MultiEntryTranslationMapping;

        [SerializeField] TextPostProcessing previewPostProcessing;

        TextField _contentField;
        TextField _contentComparisonField;
        Label _unusedBySLField;
        VisualElement _dynamicValues;
        VisualElement _dynamicValuesContent;
        TextField _preview;

        SaveFile.EntryData entry;
        MappedField entryField;

        List<Button> _dynamicValuesButtons = new List<Button>();

        Action _onNextUpdate;

        UndoItem<string> _undoItem;

        protected override void Awake()
        {
            base.Awake();

            _contentField = Container.Q<TextField>("content");
            _contentComparisonField = Container.Q<TextField>("content-comparison");
            _unusedBySLField = Container.Q<Label>("unused-warning");
            _dynamicValues = Container.Q("dynamic-values");
            _dynamicValuesContent = Container.Q("dynamic-values-content");
            _preview = Container.Q<TextField>("preview");

            _contentField.RegisterValueChangedCallback(args =>
            {
                if (args.target == _contentField && entry != null)
                    entry.content = _contentField.value;

                UpdatePreview();

                if (entry != null)
                {
                    if (!undo.IsLatest(_undoItem))
                    {
                        _undoItem = new UndoItem<string>(args.previousValue, a => manager.File.Entries[entry.entryId].content = a);
                        undo.AddStep(_undoItem);
                    }

                    _undoItem.newValue = _contentField.value;
                }

                MarkFileDirty();
            });

            _preview.Query<TextElement>()
                .Build()
                .ForEach(x => x.enableRichText = true);

            undo.OnUndo.AddListener(OnUndoChanged);
            undo.OnRedo.AddListener(OnUndoChanged);
        }

        void OnUndoChanged()
        {
            if (entry == null)
                return;

            _contentField.SetValueWithoutNotify(entry.content);
            UpdatePreview();
        }

        void UpdatePreview()
        {
            _preview.ChangeDispaly(!string.IsNullOrWhiteSpace(_contentField.value));
            _preview.value = previewPostProcessing.ProcessText(_contentField.value);
        }

        private void Update()
        {
            if (_onNextUpdate != null)
            {
                _onNextUpdate();
                _onNextUpdate = null;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            entry = inspector.SelectedObject as SaveFile.EntryData;
            entryField = manager.CurrentVersion.MappedFields.TryGetValue(entry.entryId, out var f) ?
                f :
                null;

            _contentField.SetValueWithoutNotify(entry.content);
            _unusedBySLField.ChangeDispaly(entryField?.notYetAddedToSL ?? false);

            UpdateContentComparison();

            manager.ComparisonManager.OnChangeCurrent += () => UpdateContentComparison();

            _dynamicValues.ChangeDispaly(entryField.dynamicValues.Count > 0);
            foreach (var item in entryField.dynamicValues)
            {
                var button = new Button();
                button.text = $"{item.tag} <color=#aaaaaa><size=18>- {item.description}</size></color>";
                button.clicked += () => InsertTextAtCarret(item.tag);
                _dynamicValuesButtons.Add(button);
                _dynamicValuesContent.Add(button);
            }

            UpdatePreview();
        }

        public void InsertTextAtCarret(string text)
        {
            var index = _contentField.cursorIndex;
            var val = _contentField.value;
            _contentField.value = $"{val.Substring(0, index)}{text}{val.Substring(index, val.Length - index)}";
            _contentField.Focus();

            _onNextUpdate += () =>
                _contentField.SelectRange(index + text.Length, index + text.Length);
        }

        void UpdateContentComparison()
        {
            if (entry == null) return;

            bool exists = manager.ComparisonManager.TryGetEntryData(entry.entryId, out string content);
            _contentComparisonField.SetValueWithoutNotify(content);
            _contentComparisonField.label = manager.ComparisonManager.CurrentName ?? string.Empty;
            _contentComparisonField.ChangeDispaly(exists);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            _undoItem = null;

            entry = null;
            manager.ComparisonManager.OnChangeCurrent -= () => UpdateContentComparison();

            foreach (var item in _dynamicValuesButtons)
                _dynamicValuesContent.Remove(item);

            _dynamicValuesButtons.Clear();
        }
    }
}