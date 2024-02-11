using Project.Translation;
using UnityEngine;
using qASIC;
using Project.GUI.Preview;
using Project.GUI.Hierarchy;

namespace Project.GUI.Inspector
{
    public class InspectorDisplay : MonoBehaviour
    {
        public TranslationManager manager;
        public HierarchyController hierarchy;
        public InspectorDisplayPanel[] panels;
        public PreviewSceneManager previewSceneManager;

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
            {
                item.manager = manager;
                item.inspector = this;
            }
        }

        public void RepaintPreview()
        {
            var appFile = manager.file;
            if (appFile != null)
                previewSceneManager.ReloadActiveScenes(appFile);
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

        private void Hierarchy_OnSelect(HierarchyItem item)
        {
            _selectedId = item.id;
            ReloadInspector();
        }
    }
}