using Project.Translation.Mapping;
using Project.Translation.Data;
using UnityEngine.UIElements;

namespace Project.GUI.Inspector
{
    public class SingleEntryInspector : InspectorDisplayPanel
    {
        public override bool ShouldOpen(IApplicationObject obj) =>
            obj is SaveFile.EntryData data &&
            manager.CurrentVersion.MappedFields.TryGetValue(data.entryId, out MappedField field) &&
            field.mappingContainer is MultiEntryTranslationMapping;

        TextField _contentField;
        TextField _contentComparisonField;
        Label _unusedBySLField;

        SaveFile.EntryData entry;
        MappedField entryField;

        protected override void Awake()
        {
            base.Awake();

            _contentField = Container.Q<TextField>("content");
            _contentComparisonField = Container.Q<TextField>("content-comparison");
            _unusedBySLField = Container.Q<Label>("unused-warning");

            _contentField.RegisterValueChangedCallback(args =>
            {
                if (args.target == _contentField && entry != null)
                    entry.content = _contentField.value;

                MarkFileDirty();
            });
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

            manager.ComparisonManager.OnChangeCurrent += _ => UpdateContentComparison();
        }

        void UpdateContentComparison()
        {
            if (entry == null) return;

            bool exists = manager.ComparisonManager.TryGetEntryData(entry.entryId, out string content);
            _contentComparisonField.SetValueWithoutNotify(content);
            _contentComparisonField.label = manager.ComparisonManager.CurrentTranslation?.displayName ?? string.Empty;
            _contentComparisonField.ChangeDispaly(exists);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            entry = null;
            manager.ComparisonManager.OnChangeCurrent -= _ => UpdateContentComparison();
        }
    }
}