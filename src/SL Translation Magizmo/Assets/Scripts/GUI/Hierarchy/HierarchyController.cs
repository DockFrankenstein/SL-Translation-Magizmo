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
            Items = itemProviders
                .SelectMany(x => x.GetItems())
                .ToList();

            UiItems.Clear();
            ItemIds.Clear();

            var foldoutIndex = -1;
            var itemIndex = 0;

            foreach (var item in Items)
            {
                if (item.type == HierarchyItem.ItemType.Header)
                {
                    if (Foldouts.IndexInRange(foldoutIndex))
                    {
                        var prevContent = Foldouts[foldoutIndex].Value;
                        var childCount = prevContent.childCount;
                        for (int i = itemIndex; i < childCount; i++)
                            prevContent.RemoveAt(prevContent.childCount - 1);
                    }

                    foldoutIndex++;
                    itemIndex = 0;
                    if (!Foldouts.IndexInRange(foldoutIndex))
                    {
                        var header = new Foldout()
                        {
                            text = item.displayText,
                            value = item.IsExpanded,
                        };

                        var newContent = new VisualElement();

                        header.RegisterValueChangedCallback(args =>
                        {
                            if (args.target == header)
                            {
                                item.IsExpanded = header.value;
                                newContent.ChangeDispaly(header.value);
                            }
                        });

                        Foldouts.Add(new KeyValuePair<Foldout, VisualElement>(header, newContent));

                        RegisterUiItem(item, Foldouts[foldoutIndex].Key);
                        scroll.contentContainer.Add(header);
                        scroll.contentContainer.Add(newContent);

                        newContent.ChangeDispaly(header.value);
                    }

                    if (Foldouts.IndexInRange(foldoutIndex))
                    {
                        Foldouts[foldoutIndex].Key.text = item.displayText;
                        item.IsExpanded = Foldouts[foldoutIndex].Key.value;
                    }

                    continue;
                }

                if (foldoutIndex == -1)
                {
                    foldoutIndex++;
                    if (Foldouts.IndexInRange(foldoutIndex))
                        scroll.contentContainer.Remove(Foldouts[foldoutIndex].Key);

                    if (!Foldouts.IndexInRange(foldoutIndex))
                    {
                        var container = new VisualElement();
                        Foldouts.Add(new KeyValuePair<Foldout, VisualElement>(null, container));
                        scroll.contentContainer.Add(container);
                    }
                }

                var content = Foldouts[foldoutIndex].Value;

                switch (item.type)
                {
                    default:
                        while (itemIndex < content.childCount &&
                            !(content.ElementAt(itemIndex) is Button))
                        {
                            content.RemoveAt(itemIndex);
                        }

                        if (itemIndex >= content.childCount)
                        {
                            var button = new Button()
                            {
                                text = item.displayText,
                                tooltip = item.displayText,
                            };

                            button.clicked += () => ButtonClicked(button);

                            content.Add(button);
                            RegisterUiItem(item, button);
                            itemIndex++;
                            break;
                        }

                        var oldButton = content.ElementAt(itemIndex) as Button;

                        oldButton.text = item.displayText;
                        oldButton.tooltip = item.displayText;
                        itemIndex++;
                        RegisterUiItem(item, oldButton);
                        break;
                    case HierarchyItem.ItemType.Separator:
                        if (content.Children().IndexInRange(itemIndex) &&
                            content.ElementAt(itemIndex).ClassListContains("separator"))
                        {
                            RegisterUiItem(item, content.ElementAt(itemIndex));
                            itemIndex++;
                            break;
                        }

                        var separator = new VisualElement()
                            .WithClass("separator");

                        content.Add(separator);
                        RegisterUiItem(item, separator);
                        itemIndex++;

                        break;
                }
            }

            var lastFoldoutContent = Foldouts[foldoutIndex].Value;
            while (itemIndex < lastFoldoutContent.childCount)
                lastFoldoutContent.RemoveAt(itemIndex);

            foldoutIndex++;
            while (foldoutIndex < Foldouts.Count)
            {
                if (scroll.contentContainer.Contains(Foldouts[foldoutIndex].Key))
                    scroll.contentContainer.Remove(Foldouts[foldoutIndex].Key);

                if (scroll.contentContainer.Contains(Foldouts[foldoutIndex].Value))
                    scroll.contentContainer.Remove(Foldouts[foldoutIndex].Value);

                Foldouts.RemoveAt(foldoutIndex);
            }

            Select(_selectedButton != null && UiItems.Reverse.ContainsKey(_selectedButton) ?
                UiItems.Reverse[_selectedButton] :
                null);
        }

        void ButtonClicked(Button btn)
        {
            const string selectedClass = "hierarchy-selected";

            var item = UiItems.Reverse[btn];

            Select(item, false);

            if (_selectedButton != null)
                _selectedButton.RemoveFromClassList(selectedClass);

            _selectedButton = btn;
            _selectedButton.AddToClassList(selectedClass);
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