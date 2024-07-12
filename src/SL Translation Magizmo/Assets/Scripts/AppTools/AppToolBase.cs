using Project.Translation;
using Project.Undo;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.AppTools
{
    public abstract class AppToolBase : MonoBehaviour
    {
        public UIDocument document;

        public abstract string ToolName { get; }

        protected AppToolManager Manager { get; private set; }
        
        protected TranslationManager TranslationManager =>
            Manager.translationManager;
        
        protected UndoManager Undo =>
            Manager.undo;

        internal void SetManager(AppToolManager manager)
        {
            Manager = manager;
        }

        private void Reset()
        {
            document = GetComponentInChildren<UIDocument>();
        }

        private void Awake()
        {
            CloseTool();
        }

        internal void RunSetup()
        {
            SetupTool();
        }

        protected virtual void SetupTool()
        {

        }

        public void ShowTool()
        {
            document.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void CloseTool()
        {
            document.rootVisualElement.style.display = DisplayStyle.None;
        }

        public virtual void RunTool()
        {
            Undo.ClearDirty();
            CloseTool();
        }
    }
}