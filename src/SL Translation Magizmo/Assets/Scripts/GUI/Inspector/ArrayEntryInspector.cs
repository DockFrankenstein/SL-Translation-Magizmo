using Project.Translation.Mapping;
using UnityEngine.UIElements;
using Project.Translation.Data;
using Project.UI;

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

        protected override void Awake()
        {
            base.Awake();
            _contentList = new AppReorderableList<string>(Container.Q<ListView>("content"));
            _unusedBySLField = Container.Q<Label>("unused-warning");

            _contentList.MakeItem += () =>
                new TextField().WithMargin(4f);

            _contentList.OnChanged += () =>
            {
                if (entry != null)
                {
                    entry.content = _contentList.Source.ToEntryContent();
                    MarkFileDirty();
                }
            };
        }

        public override void Initialize()
        {
            base.Initialize();
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
    }
}