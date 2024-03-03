using Project.Translation;
using Project.Translation.Data;
using UnityEngine;
using Project.Translation.Mapping;

namespace Project.GUI.Inspector
{
    public class InspectorEntryNameProvider : InspectorNameProvider
    {
        [SerializeField] TranslationManager manager;

        public override bool TryGetName(IApplicationObject obj, out string name)
        {
            if (obj is SaveFile.EntryData entry)
            {
                name = manager.CurrentVersion.MappedFields.TryGetValue(entry.entryId, out MappedField field) ?
                    field.GetFinalName() :
                    entry.entryId;

                return true;
            }

            name = string.Empty;
            return false;
        }
    }
}