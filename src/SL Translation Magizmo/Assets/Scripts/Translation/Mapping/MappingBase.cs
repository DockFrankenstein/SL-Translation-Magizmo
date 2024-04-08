using UnityEngine;
using Project.Translation.Data;
using System;

namespace Project.Translation.Mapping
{
    public abstract class MappingBase : ScriptableObject
    {
        public string fileName;

        public virtual MappedField NameField { get => null; }

        public abstract MappedField[] GetMappedFields();

        public abstract void Import(SaveFile file, string txt);

        public abstract string Export(Func<int, MappedField, string> getTextContent);

        public virtual bool Hide { get; } = false;
    }
}
