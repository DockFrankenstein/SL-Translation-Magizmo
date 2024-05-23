using Project.Translation;
using Project.Translation.Data;
using UnityEngine;
using Project.Translation.Mapping;
using Project.GUI.Top;

namespace Project.GUI.Inspector
{
    public class InspectorEntryNameProvider : InspectorNameProvider
    {
        [SerializeField] TranslationManager manager;

        public override bool TryGetName(IApplicationObject obj, out string name)
        {
            if (obj is SaveFile.EntryData entry)
            {
                if (TopMenuView.Sett_ShowIds)
                {
                    name = entry.entryId;
                    return true;
                }

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