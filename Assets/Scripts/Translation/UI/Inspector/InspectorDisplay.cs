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

        public InspectorDisplayPanel CurrentPanel =>
            panels.IndexInRange(_currentPanelIndex) ?
            panels[_currentPanelIndex] :
            null;

        private void Awake()
        {
            hierarchy.OnSelect += Hierarchy_OnSelect;

            foreach (var item in panels)
                item.manager = manager;
        }

        private void Hierarchy_OnSelect(string id)
        {
            if (CurrentPanel != null)
            {
                CurrentPanel.Uninitialize();
                CurrentPanel.gameObject.SetActive(false);
                CurrentPanel.id = string.Empty;
            }

            foreach (var panel in panels)
            {
                if (!panel.ShouldOpen(id)) continue;
                _currentPanelIndex = System.Array.IndexOf(panels, panel);
                CurrentPanel.id = id;
                CurrentPanel.Initialize();
                CurrentPanel.gameObject.SetActive(true);
            }
        }
    }
}