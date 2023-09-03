using System;

namespace Project.Translation.Defines
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DefineNameAttribute : Attribute
    {
        public DefineNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public DefineField GetDefineField() =>
            new DefineField(Name);
    }
}