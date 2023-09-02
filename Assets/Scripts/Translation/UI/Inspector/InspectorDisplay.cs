using Project.Translation;
using UnityEngine;
using qASIC;
using Project.Translation.UI;

namespace Project.UI
{
    public class InspectorDisplay : MonoBehaviour
    {
        public TranslationManager manager;
        public HierarchyDisplay hierarchy;
        public InspectorDisplayPanel[] panels;

        int _currentPanelIndex = -1;
        string _selectedId = string.Empty;

        public InspectorDisplayPanel CurrentPanel =>
            panels.IndexInRange(_currentPanelIndex) ?
            panels[_currentPanelIndex] :
            null;

        private void Awake()
        {
            hierarchy.OnSelect += Hierarchy_OnSelect;
            manager?.OnImport.AddListener(ReloadInspector);
            manager?.OnLoad.AddListener(ReloadInspector);

            foreach (var item in panels)
                item.manager = manager;
        }

        public void ReloadInspector()
        {
            if (CurrentPanel != null)
            {
                CurrentPanel.Uninitialize();
                CurrentPanel.gameObject.SetActive(false);
                CurrentPanel.id = string.Empty;
            }

            foreach (var panel in panels)
            {
                if (!panel.ShouldOpen(_selectedId)) continue;
                _currentPanelIndex = System.Array.IndexOf(panels, panel);
                CurrentPanel.id = _selectedId;
                CurrentPanel.Initialize();
                CurrentPanel.gameObject.SetActive(true);
            }
        }

        private void Hierarchy_OnSelect(string id)
        {
            _selectedId = id;
            ReloadInspector();
        }
    }
}