using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Fab.UITKDropdown;
using System;
using UnityEngine.Serialization;
using qASIC.SettingsSystem;
using qASIC;

namespace Project.GUI.Hierarchy
{
    public class HierarchyController : MonoBehaviour
    {
        [EditorButton(nameof(Refresh), activityType: ButtonActivityType.OnPlayMode)]
        public UIDocument document;
        [FormerlySerializedAs("providers")] 
        public HierarchyItemProvider[] itemProviders;

        ScrollView scroll;

        public HierarchyItem SelectedItem { get; private set; }
        public string SelectedId => SelectedItem?.id;
        public event Action<HierarchyItem> OnSelect;

        public List<HierarchyItem> Items { get; private set; } = new List<HierarchyItem>();

        Button _selectedButton;

        Dictionary<string, List<HierarchyItem>> ItemIds { get; set; } = new Dictionary<string, List<HierarchyItem>>();
        Map<HierarchyItem, VisualElement> UiItems { get; set; } = new Map<HierarchyItem, VisualElement>();

        public List<KeyValuePair<Foldout, VisualElement>> Foldouts { get; private set; } = new List<KeyValuePair<Foldout, VisualElement>>();

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        [OptionsSetting("hierarchy_collapsed_default", false)]
        private static void SettM_CollapsedByDefault(bool value)
        {
            Sett_CollapsedByDefault = value;
        }

        private static bool Sett_CollapsedByDefault { get; set; }

        private void Awake()
        {
            var root = document.rootVisualElement;
            scroll = root.Q<ScrollView>("hierarchy-list");

            foreach (var provider in itemProviders)
                provider.Hierarchy = this;

            Refresh();

            if (Sett_CollapsedByDefault)
                CollapseAll();
        }

        void RegisterUiItem(HierarchyItem item, VisualElement element)
        {
            if (!UiItems.Forward.ContainsKey(item))
                UiItems.Add(item, element);

            if (!ItemIds.ContainsKey(item.id))
                ItemIds.Add(item.id, new List<HierarchyItem>());

            ItemIds[item.id].Add(item);
        }

        public void Refresh()
        {
            scroll.contentContainer.Clear();

            Items = itemProviders
                .SelectMany(x => x.GetItems())
                .ToList();

            UiItems.Clear();
            ItemIds.Clear();
            Select(null as HierarchyItem, false);

            Foldout currentHeader = null;
            VisualElement currentContent = null;

            foreach (var item in Items)
            {
                if (item.type == HierarchyItem.ItemType.Header)
                {
                    var header = new Foldout()
                    {
                        text = item.displayText,
                        value = item.IsExpanded,
                    };

                    header.RegisterValueChangedCallback(args =>
                    {
                        if (args.target == header)
                            item.IsExpanded = header.value;
                    });

                    currentHeader = header;
                    RegisterUiItem(item, currentHeader);
                    scroll.contentContainer.Add(header);
                    currentContent = null;
                    continue;
                }

                if (currentContent == null)
                {
                    currentContent = new VisualElement();
                    scroll.contentContainer.Add(currentContent);

                    var head = currentHeader;
                    var content = currentContent;

                    Foldouts.Add(new KeyValuePair<Foldout, VisualElement>(head, content));

                    head?.RegisterValueChangedCallback(args =>
                    {
                        if (args.target == head)
                            content.ChangeDispaly(head.value);
                    });

                    content.ChangeDispaly(head.value);
                }

                VisualElement element;

                switch (item.type)
                {
                    default:
                        var button = new Button()
                        {
                            text = item.displayText,
                            tooltip = item.displayText,
                        };

                        button.clicked += () =>
                        {
                            const string selectedClass = "hierarchy-selected";

                            Select(item, false);

                            if (_selectedButton != null)
                                _selectedButton.RemoveFromClassList(selectedClass);

                            _selectedButton = button;
                            _selectedButton.AddToClassList(selectedClass);
                        };

                        element = button;
                        break;
                    case HierarchyItem.ItemType.Separator:
                        element = new VisualElement()
                            .WithClass("separator");

                        break;
                }

                currentContent.Add(element);
                RegisterUiItem(item, element);
            }
        }

        public void ExpandAll()
        {
            foreach (var item in Foldouts)
            {
                item.Key.value = true;
            }
        }

        public void CollapseAll()
        {
            foreach (var item in Foldouts)
            {
                item.Key.value = false;
            }
        }

        public void Select(string id, bool autoScroll = true)
        {
            if (!ItemIds.ContainsKey(id)) return;
            var item = ItemIds[id].FirstOrDefault();
            Select(item, autoScroll);
        }

        public void Select(HierarchyItem item, bool autoScroll = true)
        {
            if (SelectedItem == item) return;
            SelectedItem = item;
            OnSelect?.Invoke(item);
        }
    }
}