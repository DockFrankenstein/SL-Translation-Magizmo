using Project.Translation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC;

namespace Project.UI
{
    public class InspectorDisplay : MonoBehaviour
    {
        public TranslationManager manager;
        public InspectorDisplayPanel[] panels;

        int _currentPanelIndex = -1;

        public InspectorDisplayPanel CurrentPanel =>
            panels.IndexInRange(_currentPanelIndex) ?
            panels[_currentPanelIndex] :
            null;

        private void Awake()
        {
            manager.OnSelectionChange += Manager_OnSelectionChange;

            foreach (var item in panels)
            {
                item.manager = manager;
            }
        }

        private void Manager_OnSelectionChange(string id)
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