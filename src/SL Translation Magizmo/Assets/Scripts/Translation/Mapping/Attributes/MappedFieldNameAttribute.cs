using System;

namespace Project.Translation.Mapping
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MappedFieldNameAttribute : Attribute
    {
        public MappedFieldNameAttribute(string id)
        {
            Id = id;
        }

        public MappedFieldNameAttribute(string id, string name) : this(id)
        {
            Name = name;
        }

        public string Id { get; private set; }
        public string Name { get; private set; } = null;

        public MappedField GetMappedField()
        {
            var field = new MappedField(Id);

            if (Name != null)
            {
                field.displayName = Name;
                field.autoDisplayName = false;
            }

            return field;
        }
    }
}