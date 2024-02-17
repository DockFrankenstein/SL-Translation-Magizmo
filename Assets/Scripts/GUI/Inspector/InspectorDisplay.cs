using Project.Translation;
using UnityEngine;
using qASIC;
using Project.GUI.Preview;
using Project.GUI.Hierarchy;
using UnityEngine.UIElements;

namespace Project.GUI.Inspector
{
    public class InspectorDisplay : MonoBehaviour
    {
        public UIDocument document;

        [Space]
        public TranslationManager manager;
        public HierarchyController hierarchy;
        public InspectorDisplayPanel[] inspectors;
        public PreviewSceneManager previewSceneManager;

        public IApplicationObject SelectedObject { get; private set; }
        public InspectorDisplayPanel CurrentPanel { get; private set; }

        VisualElement _nothingSelectedPanel;
        VisualElement _selectedPanel;
        public VisualElement ContentContainer { get; private set; }
        Label _itemName;

        private void Awake()
        {
            var root = document.rootVisualElement;

            _nothingSelectedPanel = root.Q("inspector-none");
            _selectedPanel = root.Q("inspector-selected");
            _itemName = root.Q<Label>("inspector-name");
            ContentContainer = root.Q("inspector-content");

            hierarchy.OnSelect += Hierarchy_OnSelect;
            manager?.OnImport.AddListener(ReloadInspector);
            manager?.OnLoad.AddListener(ReloadInspector);

            foreach (var item in inspectors)
            {
                item.manager = manager;
                item.inspector = this;
            }

            ReloadInspector();
        }

        public void RepaintPreview()
        {
            var appFile = manager.file;
            if (appFile != null)
                previewSceneManager.ReloadActiveScenes(appFile);
        }

        public void ReloadInspector()
        {
            var nothingSelected = SelectedObject == null;

            if (!nothingSelected)
                _itemName.text = SelectedObject.Name;

            _selectedPanel.style.display = nothingSelected ?
                DisplayStyle.None :
                DisplayStyle.Flex;

            _nothingSelectedPanel.style.display = nothingSelected ?
                DisplayStyle.Flex :
                DisplayStyle.None;

            if (CurrentPanel != null)
            {
                CurrentPanel.Uninitialize();
            }

            CurrentPanel = null;

            foreach (var panel in inspectors)
            {
                if (!panel.ShouldOpen(SelectedObject)) continue;
                CurrentPanel = panel;
                CurrentPanel.Initialize();
                break;
            }
        }

        private void Hierarchy_OnSelect(HierarchyItem item)
        {
            SelectedObject = item.Item as IApplicationObject ?? item;
            ReloadInspector();
        }
    }
}