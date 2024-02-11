using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Fab.UITKDropdown;
using System;

namespace Project.GUI.Hierarchy
{
    public class HierarchyController : MonoBehaviour
    {
        public UIDocument document;
        public HierarchyItemProvider[] providers;

        ScrollView scroll;

        public HierarchyItem SelectedItem { get; private set; }
        public string SelectedId => SelectedItem?.id;
        public event Action<HierarchyItem> OnSelect;

        public List<HierarchyItem> Items { get; private set; } = new List<HierarchyItem>();

        Button _selectedButton;

        Dictionary<string, List<HierarchyItem>> ItemIds { get; set; } = new Dictionary<string, List<HierarchyItem>>();
        Dictionary<HierarchyItem, VisualElement> UiItems { get; set; } = new Dictionary<HierarchyItem, VisualElement>();

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        private void Awake()
        {
            var root = document.rootVisualElement;
            scroll = root.Q<ScrollView>("hierarchy-list");

            Refresh();
        }

        void RegisterUiItem(HierarchyItem item, VisualElement element)
        {
            if (!UiItems.ContainsKey(item))
                UiItems.Add(item, element);

            if (!ItemIds.ContainsKey(item.id))
                ItemIds.Add(item.id, new List<HierarchyItem>());

            ItemIds[item.id].Add(item);
        }

        public void Refresh()
        {
            scroll.contentContainer.Clear();

            Items = providers
                .SelectMany(x => x.GetItems())
                .ToList();

            Foldout currentHeader = null;
            VisualElement currentContent = null;

            foreach (var item in Items)
            {
                if (item.type == HierarchyItem.ItemType.Header)
                {
                    var header = new Foldout()
                    {
                        text = item.displayText,
                    };

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

                    head?.RegisterValueChangedCallback(args =>
                    {
                        Debug.Log(head.value);
                        if (args.target == head)
                            content.style.display = head.value ? DisplayStyle.Flex : DisplayStyle.None;
                    });
                }

                VisualElement element;

                switch (item.type)
                {
                    default:
                        var button = new Button()
                        {
                            text = item.displayText,
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