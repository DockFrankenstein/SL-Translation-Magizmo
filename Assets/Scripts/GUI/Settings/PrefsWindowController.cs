using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Settings
{
    public class PrefsWindowController : MonoBehaviour
    {
        private bool _visible;
        public bool Visible 
        {
            get => _visible;
            set
            {
                if (_visible == value) return;
                _visible = value;
                document.rootVisualElement.style.display = _visible switch
                {
                    false => DisplayStyle.None,
                    _ => DisplayStyle.Flex,
                };

                if (_visible)
                {
                    SetPanel(0);
                    return;
                }
            }
        }

        public UIDocument document;
        public Panel[] panels;

        ScrollView _contentScroll;
        RadioButtonGroup _menuSelection;
        Button _backButton;

        public int SelectedPanelIndex { get; private set; } = -1;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        private void Awake()
        {           
            var root = document.rootVisualElement;

            _menuSelection = root.Q<RadioButtonGroup>("menu-selection");
            _backButton = root.Q<Button>("back-button");
            _contentScroll = root.Q<ScrollView>("content-scroll");

            _backButton.clicked += _backButton_clicked;
            _menuSelection.RegisterValueChangedCallback(args =>
            {
                if (args.target == _menuSelection)
                    SetPanel(_menuSelection.value);
            });

            foreach (var item in panels)
                item.panel = root.Q(item.panelName);

            root.style.display = DisplayStyle.None;
        }

        private void _backButton_clicked()
        {
            Visible = false;
        }

        public Panel GetPanel(int index)
        {
            if (panels.Length == 0) 
                return null;

            if (index < 0) 
                return panels[0];

            if (index >= panels.Length)
                return panels[panels.Length - 1];

            return panels[index];
        }

        public void SetPanel(int index)
        {
            if (SelectedPanelIndex == index) return;

            foreach (var item in panels)
                item.panel.style.display = DisplayStyle.None;

            var panel = GetPanel(index);
            if (panel == null) return;

            panel.panel.style.display = DisplayStyle.Flex;
            _contentScroll.scrollOffset = Vector2.zero;
        }

        [System.Serializable]
        public class Panel
        {
            public string panelName;
            [HideInInspector] public VisualElement panel;
        }
    }
}