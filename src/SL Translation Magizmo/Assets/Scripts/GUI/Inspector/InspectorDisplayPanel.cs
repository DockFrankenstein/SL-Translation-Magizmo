using UnityEngine;
using Project.Translation;
using UnityEngine.UIElements;
using Project.Undo;

namespace Project.GUI.Inspector
{
    public abstract class InspectorDisplayPanel : MonoBehaviour
    {
        public VisualTreeAsset asset;

        [HideInInspector] public TranslationManager manager;
        [HideInInspector] public InspectorDisplay inspector;
        [HideInInspector] public UndoManager undo;

        public TemplateContainer Container { get; protected set; }

        protected virtual void Awake()
        {
            Container = asset.Instantiate();
        }

        public virtual void Initialize()
        {
            inspector.ContentContainer.Add(Container);
        }

        public virtual void Uninitialize()
        {
            inspector.ContentContainer.Remove(Container);
        }

        public virtual bool ShouldOpen(IApplicationObject obj) => false;
    }
}