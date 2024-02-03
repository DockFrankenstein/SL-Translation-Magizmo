using UnityEngine;
using System.Linq;
using System;
using TMPro;
using Project.Utility.UI;

namespace Project.GUI.Inspector
{
    public sealed class ManifestInspector : InspectorDisplayPanel
    {
        [SerializeField] bool fallbackVersion;
        [SerializeField] string[] versions;
        [SerializeField][Tooltip("Id of the field in hierarchy")] string hierarchyId;
        [SerializeField] Field[] fields;
        [SerializeField] List[] lists;

        private void Awake()
        {
            foreach (var item in fields)
                item.Setup(this);

            foreach (var item in lists)
                item.Setup(this);
        }

        public override bool ShouldOpen(string id)
        {
            return hierarchyId == id && //ID matches
                (fallbackVersion || versions.Contains(manager.CurrentVersion.version)); //Correct version
        }

        public override void Initialize()
        {
            foreach (var item in fields)
                item.Initialize(this);

            foreach (var item in lists)
                item.Initialize(this);
        }

        public override void Uninitialize()
        {
            foreach (var item in fields)
                item.Uninitialize(this);

            foreach (var item in lists)
                item.Uninitialize(this);
        }

        [Serializable]
        private class Field
        {
            public TMP_InputField field;
            [Tooltip("Id of the field in file")] public string id;

            public void Setup(ManifestInspector inspector)
            {
                if (field != null)
                {
                    field.onValueChanged.AddListener((a) => Field_OnValueChanged(inspector, a));
                }
            }

            public void Initialize(ManifestInspector inspector)
            {
                if (inspector.manager.file.Entries.ContainsKey(id))
                    field.SetTextWithoutNotify(inspector.manager.file.Entries[id].content);
            }

            public void Uninitialize(ManifestInspector inspector)
            {
                field.SetTextWithoutNotify(string.Empty);
            }

            private void Field_OnValueChanged(ManifestInspector inspector, string text)
            {
                if (inspector.manager.file.Entries.ContainsKey(id))
                    inspector.manager.file.Entries[id].content = text;

                inspector.RepaintPreview();
            }
        }

        [Serializable]
        private class List
        {
            public ReorderableListUI list;
            public string id;

            public void Setup(ManifestInspector inspector)
            {
                if (list != null)
                    list.OnChange.AddListener(() => Field_OnValueChanged(inspector));
            }

            public void Initialize(ManifestInspector inspector)
            {
                if (inspector.manager.file.Entries.ContainsKey(id))
                {
                    var entry = inspector.manager.file.Entries[id];
                    list.ChangeValuesWithoutNotify(entry.content.EntryContentToArray().ToList());
                }
            }

            public void Uninitialize(ManifestInspector inspector)
            {

            }

            private void Field_OnValueChanged(ManifestInspector inspector)
            {
                if (inspector.manager.file.Entries.ContainsKey(id))
                    inspector.manager.file.Entries[id].content = list.Values.ToEntryContent();

                inspector.RepaintPreview();
            }
        }
    }
}