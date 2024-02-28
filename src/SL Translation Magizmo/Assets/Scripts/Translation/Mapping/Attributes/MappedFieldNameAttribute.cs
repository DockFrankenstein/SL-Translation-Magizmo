using System;

namespace Project.Translation.Mapping
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MappedFieldNameAttribute : Attribute
    {
        public MappedFieldNameAttribute(string name)
        {
            Id = name;
        }

        public string Id { get; private set; }

        public MappedField GetMappedField() =>
            new MappedField(Id);
    }
}