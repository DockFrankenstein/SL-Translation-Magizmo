using System;

namespace Project.Translation.Mapping
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MappedFieldNameAttribute : Attribute
    {
        public MappedFieldNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public MappedField GetDefineField() =>
            new MappedField(Name);
    }
}