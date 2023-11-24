using UnityEngine;
using Project.Translation;

namespace Project.UI
{
    public class InspectorDisplayPanel : MonoBehaviour
    {
        [HideInInspector] public TranslationManager manager;
        [HideInInspector] public InspectorDisplay inspector;
        [HideInInspector] public string id;

        public virtual void Initialize()
        {

        }

        public virtual void Uninitialize()
        {

        }

        public virtual bool ShouldOpen(string id) => false;

        public void RepaintPreview() =>
            inspector.RepaintPreview();
    }
}