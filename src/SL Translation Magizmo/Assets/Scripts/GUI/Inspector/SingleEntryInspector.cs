using Project.Translation.Mapping;
using Project.Translation.Data;
using UnityEngine.UIElements;
using UnityEngine;

namespace Project.GUI.Inspector
{
    public class SingleEntryInspector : InspectorDisplayPanel
    {
        public override bool ShouldOpen(IApplicationObject obj) =>
            obj is SaveFile.EntryData data &&
            manager.CurrentVersion.MappedFields.TryGetValue(data.entryId, out MappedField field) &&
            field.mappingContainer is MultiEntryTranslationMapping;

        TextField _contentField;

        SaveFile.EntryData entry;

        protected override void Awake()
        {
            base.Awake();

            _contentField = Container.Q<TextField>("content");

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
            _contentField.SetValueWithoutNotify(entry.content);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            entry = null;
        }
    }
}