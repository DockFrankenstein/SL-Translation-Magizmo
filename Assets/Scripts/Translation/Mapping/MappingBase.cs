using UnityEngine;
using Project.Translation.Data;

namespace Project.Translation.Mapping
{
    public abstract class MappingBase : ScriptableObject
    {
        public string fileName;

        public abstract MappedField[] GetMappedFields();

        public abstract void Import(SaveFile file, string txt);
        public abstract string Export(SaveFile file);
        public abstract string ExportDebug();

        public virtual bool Hide { get; } = false;
    }
}
