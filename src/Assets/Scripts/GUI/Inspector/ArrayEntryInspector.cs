using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Project.Translation.Mapping;
using Project.Utility.UI;
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

        protected override void Awake()
        {
            base.Awake();
            _contentList = new AppReorderableList<string>(Container.Q<ListView>("content"));
            _contentList.MakeItem += () => new TextField();

            _contentList.OnChanged += () =>
            {
                if (entry != null)
                {
                    entry.content = _contentList.Source.ToEntryContent();
                    MarkAsDirty();
                }
            };
        }

        public override void Initialize()
        {
            base.Initialize();
            entry = inspector.SelectedObject as SaveFile.EntryData;
            _contentList.Source.AddRange(entry.content.EntryContentToArray());
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
            entry = null;
            _contentList.Source.Clear();
        }
    }
}