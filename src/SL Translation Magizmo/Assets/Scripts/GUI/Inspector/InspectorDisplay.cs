using Project.Translation;
using UnityEngine;
using Project.GUI.Preview;
using Project.GUI.Hierarchy;
using UnityEngine.UIElements;
using UnityEngine.Serialization;

namespace Project.GUI.Inspector
{
    public class InspectorDisplay : MonoBehaviour
    {
        public UIDocument document;

        [Space]
        public TranslationManager manager;
        public HierarchyController hierarchy;
        public InspectorDisplayPanel[] inspectors;
        public InspectorNameProvider[] nameProviders;
        [FormerlySerializedAs("previewSceneManager")]
        public PreviewManager preview;

        IApplicationObject _selectedObject;
        public IApplicationObject SelectedObject 
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                SelectedObjectName = GetNameForObject(_selectedObject);
                ReloadInspector();
            }
        }

        public string SelectedObjectName { get; private set; }
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
            manager.OnFileChanged += context =>
            {
                if (context is InspectorDisplay ||
                    context is InspectorDisplayPanel) return;

                ReloadInspector();
            };

            foreach (var item in inspectors)
            {
                item.manager = manager;
                item.inspector = this;
            }

            ReloadInspector();
        }

        public void RepaintPreview()
        {
            var appFile = manager.File;
            if (appFile != null)
                preview.Reload();
        }

        public void ReloadInspector()
        {
            var nothingSelected = SelectedObject == null;

            if (!nothingSelected)
                _itemName.text = SelectedObjectName;

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
            SelectedObject = item?.Item as IApplicationObject ?? item;
        }

        string GetNameForObject(IApplicationObject obj)
        {
            string text = obj?.Name;

            foreach (var provider in nameProviders)
            {
                if (!provider.TryGetName(obj, out string newName)) continue;
                text = newName;
                break;
            }

            return text;
        }
    }
}