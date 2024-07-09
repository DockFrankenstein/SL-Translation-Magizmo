using Project.Translation.Mapping;
using UnityEngine.UIElements;
using Project.Translation.Data;
using Project.UI;
using System.Collections.Generic;
using qASIC;

namespace Project.GUI.Inspector
{
    public class ArrayEntryInspector : InspectorDisplayPanel
    {
        public override bool ShouldOpen(IApplicationObject obj) =>
            obj is SaveFile.EntryData data &&
            manager.CurrentVersion.MappedFields.TryGetValue(data.entryId, out MappedField field) &&
            field.mappingContainer is ArrayEntryTranslationMapping;

        AppReorderableList<string> _contentList;

        SaveFile.EntryData entry;
        MappedField entryField;
        Label _unusedBySLField;

        Dictionary<VisualElement, ArrayItem> _items = new Dictionary<VisualElement, ArrayItem>();

        protected override void Awake()
        {
            base.Awake();
            _contentList = new AppReorderableList<string>(Container.Q<ListView>("content"));
            _unusedBySLField = Container.Q<Label>("unused-warning");

            _contentList.MakeItem += MakeItem;
            _contentList.GetField += GetField;
            _contentList.OnBindItem += OnBindItem;

            _contentList.OnChanged += () =>
            {
                if (entry != null)
                {
                    entry.content = _contentList.Source.ToEntryContent();
                    MarkFileDirty();
                }
            };

            _contentList.OnDestroyItem += x =>
            {
                _items.Remove(x);
            };

            manager.ComparisonManager.OnChangeCurrent += UpdateComponentTranslation;
        }

        BaseField<string> GetField(VisualElement el) =>
            _items[el].field;

        VisualElement MakeItem()
        {
            var root = new VisualElement()
                .WithMargin(4f);
            
            var field = new TextField();
            var comparisonContent = new TextField();

            comparisonContent.multiline = true;
            comparisonContent.isReadOnly = true;
            comparisonContent.ChangeDispaly(false);
            comparisonContent.style.marginTop = 4f;

            comparisonContent.AddToClassList("content-compare-content");

            _items.Add(root, new ArrayItem()
            {
                field = field,
                comparisonField = comparisonContent,
            });

            root.Add(field);
            root.Add(comparisonContent);

            return root;
        }

        void OnBindItem(VisualElement element, int index, string value)
        {
            UpdateTranslationPreview(element, index);
        }

        void UpdateComponentTranslation()
        {
            int index = 0;
            foreach (var item in _contentList.GetElements())
            {
                UpdateTranslationPreview(item, index);
                index++;
            }
        }

        void UpdateTranslationPreview(VisualElement element, int index)
        {
            if (!_items.ContainsKey(element)) return;
            var data = _items[element];

            string[] arrayContent = new string[0];
            bool exists = manager.ComparisonManager.TryGetEntryData(entry.entryId, out string content);

            if (exists)
            {
                arrayContent = content.EntryContentToArray();
                exists = arrayContent.IndexInRange(index);
            }

            data.comparisonField.ChangeDispaly(exists);

            if (exists)
            {
                data.comparisonField.label = manager.ComparisonManager.CurrentName;
                data.comparisonField.value = arrayContent[index];
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            _items.Clear();

            entry = inspector.SelectedObject as SaveFile.EntryData;
            entryField = manager.CurrentVersion.MappedFields.TryGetValue(entry.entryId, out var f) ?
                f :
                null;

            _unusedBySLField.ChangeDispaly(entryField?.notYetAddedToSL ?? false);
            _contentList.Source.AddRange(entry.content.EntryContentToArray());
            _contentList.List.RefreshItems();
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            entry = null;
            _contentList.Source.Clear();
        }

        class ArrayItem
        {
            public TextField field;
            public TextField comparisonField;
        }
    }
}