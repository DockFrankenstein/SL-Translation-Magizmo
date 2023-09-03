using UnityEngine;
using Project.Translation.Data;

namespace Project.Translation.Defines
{
    public abstract class DefinesBase : ScriptableObject
    {
        public string fileName;

        public abstract DefineField[] GetDefines();

        public abstract void Import(AppFile file, string txt);
        public abstract string Export(AppFile file);

        public virtual bool Hide { get; } = false;
    }
}
