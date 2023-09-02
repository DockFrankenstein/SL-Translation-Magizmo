using UnityEngine;
using Project.Translation;
using System.Linq;
using Project.Translation.Defines;

namespace Project.UI
{
    public class InspectorDisplayPanel : MonoBehaviour
    {
        [HideInInspector] public TranslationManager manager;
        [HideInInspector] public string id;

        public virtual void Initialize()
        {

        }

        public virtual void Uninitialize()
        {

        }

        public virtual bool ShouldOpen(string id)
        {
            var defines = manager.CurrentVersion.defines
                .Where(x => x is ArrayEntryTranslationDefines)
                .SelectMany(x => x.GetDefines());

            return defines.Contains(id);
        }
    }
}