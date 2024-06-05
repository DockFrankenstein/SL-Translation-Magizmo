using UnityEngine;
using Project.Translation.Data;
using System;
using System.Linq;

namespace Project.Translation.Mapping
{
    public abstract class MappingBase : ScriptableObject
    {
        public string fileName;

        public virtual MappedField NameField { get => null; }

        public virtual MappedField[] GetMappedFields() =>
            GetAllMappedFields()
            .Where(x => x.Status == MappedField.SetupStatus.Used)
            .ToArray();

        public abstract MappedField[] GetAllMappedFields();

        public abstract void Import(SaveFile file, string txt);

        public abstract string Export(Func<int, MappedField, string> getTextContent);

        public virtual bool Hide { get; } = false;
    }
}
