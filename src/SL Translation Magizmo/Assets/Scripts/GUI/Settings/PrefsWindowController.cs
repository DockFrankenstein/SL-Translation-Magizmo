using Project.Translation;
using qASIC;
using qASIC.Options.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
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
                document.rootVisualElement.ChangeDispaly(_visible);
                SetPanel(0);
            }
        }

        public UIDocument document;

        public TranslationManager manager;

        ScrollView _contentScroll;
        RadioButtonGroup _menuSelection;
        Button _backButton;

        OptionsMenu optionsMenu;

        Dictionary<string, Page> Pages { get; set; } = new Dictionary<string, Page>();
        List<Page> UsedPages { get; set; } = new List<Page>();

        int CurrentPageIndex { get; set; } = -1;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        void BuildMenu()
        {
            optionsMenu = new OptionsMenu()
                .StartNewSection("Apperance")
                    .AddOption(new OptionsMenuSlider<float>("ui_scale_factor", "UI Scale Factor", 0.2f, 2f))
                    .AddOption(new OptionsMenuToggle("hierarchy_collapsed_default", "Collapse By Default"))
                .FinishSection()
                .StartNewSection("Comparing")
                    .AddOption(new OptionsMenuFieldPath("translation_path", "SL Translation Path"))
                .FinishSection()
                .StartNewSection("Updates")
                    .AddOption(new OptionsMenuToggle("show_auto_update", "New Release Notification"))
                .FinishSection()
                .StartNewSection("Discord")
                    .AddOption(new OptionsMenuToggle("discord_use_activity", "Use Activity"))
                .FinishSection();
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
                if (args.target != _menuSelection) return;

                SetPanel(_menuSelection.value);
            });

            root.style.display = DisplayStyle.None;

            RefreshUI();
        }

        void RefreshUI()
        {
            BuildMenu();

            _menuSelection.choices = optionsMenu.Sections
                .Select(x => x.Header);

            CurrentPageIndex = -1;
            UsedPages.Clear();

            foreach (var page in Pages)
                page.Value.root.ChangeDispaly(false);

            foreach (var section in optionsMenu.Sections)
            {
                if (!Pages.ContainsKey(section.Header))
                {
                    var root = new VisualElement();
                    var header = new Label(section.Header);

                    var newPage = new Page()
                    {
                        root = root,
                        header = header,
                    };

                    header.AddToClassList("header");
                    root.ChangeDispaly(false);
                    root.Add(header);
                    Pages.Add(section.Header, newPage);
                    _contentScroll.contentContainer.Add(root);
                }

                var page = Pages[section.Header];
                foreach (var item in section.GetAllOptions())
                {
                    if (!page.items.ContainsKey(item.name))
                    {
                        var newItem = CreateItem(item);
                        page.items.Add(item.name, newItem);
                        page.root.Add(newItem.Element);

                        newItem.RegisterApplying();
                        newItem.OnApply += obj =>
                        {
                            manager.Options.SetOptionAndApply(item.name, obj);
                        };
                    }

                    var val = manager.Options.GetOption(item.name);
                    page.items[item.name].SetValueWithoutNotify(val);
                }

                UsedPages.Add(page);
            }
        }

        void SetPanel(int index)
        {
            if (UsedPages.Count == 0)
                return;

            index = Mathf.Clamp(index, 0, UsedPages.Count);

            if (UsedPages.IndexInRange(CurrentPageIndex))
                UsedPages[CurrentPageIndex].root.ChangeDispaly(false);

            CurrentPageIndex = index;

            UsedPages[index].root.ChangeDispaly(true);

            _menuSelection.SetValueWithoutNotify(index);
        }

        Page.Item CreateItem(OptionsMenuItem menuItem)
        {
            switch (menuItem)
            {
                case OptionsMenuFieldPath path:
                    var pathField = new TextField(menuItem.displayName);
                    var pathButton = new Button();

                    pathButton.Add(new VisualElement());
                    pathButton.AddToClassList("file-path-open");

                    pathButton.clicked += () =>
                    {
                        var paths = SFB.StandaloneFileBrowser.OpenFolderPanel("", pathField.value, false);

                        if (paths.Length == 0)
                            return;

                        pathField.value = paths[0];
                    };

                    return new Page.Item<string>()
                    {
                        Element = pathField,
                    };
                case OptionsMenuToggle toggle:
                    return new Page.Item<bool>()
                    {
                        Element = new Toggle(toggle.displayName),
                    };
                case OptionsMenuSlider<float> slider:
                    var sliderUi = new Slider(slider.displayName, slider.minValue, slider.maxValue)
                    {
                        showInputField = true,
                    };

                    sliderUi.style.height = 35f;

                    return new Page.Item<float>()
                    {
                        Element = sliderUi,
                        ApplyOnChanged = false,
                        ApplyOnMouseLeave = true,
                        ApplyOnLostFocus = true,
                    };
                case OptionsMenuSlider<int> slider:
                    var intSliderUi = new SliderInt(slider.displayName, slider.minValue, slider.maxValue)
                    {
                        showInputField = true,
                    };

                    intSliderUi.style.height = 35f;

                    return new Page.Item<int>()
                    {
                        Element = intSliderUi,
                        ApplyOnChanged = false,
                        ApplyOnMouseLeave = true,
                        ApplyOnLostFocus = true,
                    };
                case OptionsMenuField<string> floatField:
                    return new Page.Item<string>()
                    {
                        Element = new TextField(floatField.displayName)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        private void _backButton_clicked()
        {
            Visible = false;
        }

        class Page
        {
            public VisualElement root;
            public Label header;
            public Dictionary<string, Item> items = new Dictionary<string, Item>();

            public abstract class Item
            {
                public abstract Type ValueType { get; }

                public VisualElement Element { get; set; }
                public bool ApplyOnChanged { get; set; } = true;
                public bool ApplyOnMouseLeave { get; set; } = false;
                public bool ApplyOnLostFocus { get; set; } = false;

                public event Action<object> OnApply;

                public abstract void SetValueWithoutNotify(object val);

                public abstract void RegisterApplying();

                protected void Apply(object obj) =>
                    OnApply?.Invoke(obj);
            }

            public class Item<T> : Item
            {
                public override Type ValueType => typeof(T);

                public override void SetValueWithoutNotify(object val)
                {
                    (Element as BaseField<T>).SetValueWithoutNotify((T)val);
                }

                public override void RegisterApplying()
                {
                    var field = Element as BaseField<T>;

                    if (ApplyOnChanged)
                        field.RegisterValueChangedCallback(args => Apply(field.value));

                    if (ApplyOnMouseLeave)
                        field.RegisterCallback<MouseCaptureOutEvent>(args => Apply(field.value));

                    if (ApplyOnLostFocus)
                        field.RegisterCallback<FocusOutEvent>(args => Apply(field.value));
                }
            }
        }
    }
}