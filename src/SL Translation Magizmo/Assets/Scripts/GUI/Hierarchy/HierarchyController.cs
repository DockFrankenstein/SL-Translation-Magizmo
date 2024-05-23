using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Fab.UITKDropdown;
using System;
using UnityEngine.Serialization;
using qASIC.SettingsSystem;
using qASIC;
using qASIC.Input;
using Project.GUI.Top;

namespace Project.GUI.Hierarchy
{
    public class HierarchyController : MonoBehaviour
    {
        const string SELECTED_CLASS = "hierarchy-selected";

        [EditorButton(nameof(Refresh), activityType: ButtonActivityType.OnPlayMode)]
        public UIDocument document;
        [FormerlySerializedAs("providers")] 
        public HierarchyItemProvider[] itemProviders;

        [Header("Search")]
        [SerializeField][Min(0)] int startSearchItemCount = 600;

        [Header("Shortcuts")]
        [SerializeField] float repeatWaitTime = 1f;
        [SerializeField] float repeatTime = 0.1f;
        [SerializeField] InputMapItemReference i_previous;
        [SerializeField] InputMapItemReference i_next;

        ScrollView scroll;
        TextField search;
        VisualElement contentSearch;
        VisualElement contentNormal;

        public HierarchyItem SelectedItem { get; private set; }
        public string SelectedId => SelectedItem?.id;
        public event Action<HierarchyItem> OnSelect;

        public List<HierarchyItem> Items { get; private set; } = new List<HierarchyItem>();

        Button _selectedButton;

        Dictionary<string, List<HierarchyItem>> ItemIds { get; set; } = new Dictionary<string, List<HierarchyItem>>();
        Map<HierarchyItem, VisualElement> UiItems { get; set; } = new Map<HierarchyItem, VisualElement>();

        public Map<Foldout, VisualElement> Foldouts { get; private set; } = new Map<Foldout, VisualElement>();

        Action _onNextFrame;

        Foldout _tempFoldout;

        Map<HierarchyItem, Button> _searchButtons = new Map<HierarchyItem, Button>();

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

        public bool IsSearching { get; private set; } = false;

        private void Awake()
        {
            var root = document.rootVisualElement;
            scroll = root.Q<ScrollView>("hierarchy-list");
            search = root.Q<TextField>("search");
            contentSearch = root.Q("content-search");
            contentNormal = root.Q("content-normal");

            search.RegisterValueChangedCallback(_ => UpdateSearch());

            foreach (var provider in itemProviders)
                provider.Hierarchy = this;

            EnsureSearchItemCount(startSearchItemCount);

            StartCoroutine(HandleInput());

            Refresh();

            if (Sett_CollapsedByDefault)
                CollapseAll();
        }

        void UpdateSearch()
        {
            bool prevIsSearching = IsSearching;
            IsSearching = !string.IsNullOrWhiteSpace(search.value);

            contentNormal.ChangeDispaly(!IsSearching);
            contentSearch.ChangeDispaly(IsSearching);

            _searchButtons.Clear();

            if (!IsSearching)
            {
                ChangeSelectedButton(SelectedItem != null && UiItems.Forward.TryGetValue(SelectedItem, out var selectedEl) ?
                    selectedEl as Button :
                    null);
                return;
            }

            //When searching
            var items = Items
                .SortSearchList(x => x.displayText, search.value)
                .ToArray();

            EnsureSearchItemCount(items.Length);

            var buttons = contentSearch.Children().ToArray();

            for (int i = 0; i < buttons.Length; i++)
            {
                var exists = i < items.Length;
                buttons[i].ChangeDispaly(exists);
                if (exists)
                {
                    var btn = buttons[i] as Button;
                    btn.text = GetItemDisplayName(items[i]);
                    _searchButtons.Add(items[i], btn);
                }
            }

            ChangeSelectedButton(SelectedItem != null && _searchButtons.Forward.TryGetValue(SelectedItem, out Button selectedBtn) ?
                selectedBtn :
                null);
        }

        void EnsureSearchItemCount(int itemCount)
        {
            while (contentSearch.childCount < itemCount)
            {
                var btn = new Button();
                btn.style.display = DisplayStyle.None;
                contentSearch.Add(btn);

                btn.clicked += () =>
                {
                    ButtonClicked(btn);
                };
            }
        }

        float _inputPressTime;
        InputEventType _previousInput;
        InputEventType _nextInput;

        private void Update()
        {
            if (_onNextFrame != null)
            {
                var a = _onNextFrame;
                _onNextFrame = null;
                a.Invoke();
            }

            _previousInput = i_previous.GetInputEvent();
            _nextInput = i_next.GetInputEvent();

            var inputs = new InputEventType[]
            {
                _previousInput,
                _nextInput,
            };

            if (inputs.Where(x => x.HasFlag(InputEventType.Pressed) && !x.HasFlag(InputEventType.Down)).Count() != 1)
            {
                _inputPressTime = 0f;
                return;
            }

            _inputPressTime += Time.deltaTime;
        }

        IEnumerator HandleInput()
        {
            while (true)
            {
                RunInput();
                yield return null;

                if (_inputPressTime == 0f)
                    continue;

                while (_inputPressTime != 0f)
                {
                    yield return new WaitForSecondsRealtime(repeatTime);

                    if (_inputPressTime < repeatWaitTime)
                        continue;

                    RunInput();
                }
            }

            void RunInput()
            {
                if (_previousInput.HasFlag(InputEventType.Pressed))
                    SelectBy(-1);

                if (_nextInput.HasFlag(InputEventType.Pressed))
                    SelectBy(1);
            }
        }

        public void SelectBy(int amount)
        {
            if (amount == 0)
                return;

            var items = Items
                .Where(x => x.type == HierarchyItem.ItemType.Normal)
                .ToList();

            if (items.Count == 0)
                return;

            var index = items.IndexOf(SelectedItem);
            index += amount;

            while (index < 0)
                index += items.Count;

            while (index >= items.Count)
                index -= items.Count;

            Select(items[index]);
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
                        var prevContent = Foldouts.ElementAt(foldoutIndex).Value;
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

                        Foldouts.Add(header, newContent);

                        RegisterUiItem(item, Foldouts.ElementAt(foldoutIndex).Key);
                        contentNormal.Add(header);
                        contentNormal.Add(newContent);

                        newContent.ChangeDispaly(header.value);
                    }

                    if (Foldouts.IndexInRange(foldoutIndex))
                    {
                        Foldouts.ElementAt(foldoutIndex).Key.text = item.displayText;
                        item.IsExpanded = Foldouts.ElementAt(foldoutIndex).Key.value;
                    }

                    continue;
                }

                if (foldoutIndex == -1)
                {
                    foldoutIndex++;
                    if (Foldouts.IndexInRange(foldoutIndex))
                        contentNormal.Remove(Foldouts.ElementAt(foldoutIndex).Key);

                    if (!Foldouts.IndexInRange(foldoutIndex))
                    {
                        var container = new VisualElement();
                        Foldouts.Add(null, container);
                        contentNormal.Add(container);
                    }
                }

                var content = Foldouts.ElementAt(foldoutIndex).Value;

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
                                text = GetItemDisplayName(item),
                                tooltip = item.displayText,
                            };

                            button.clicked += () => ButtonClicked(button);

                            content.Add(button);
                            RegisterUiItem(item, button);
                            itemIndex++;
                            break;
                        }

                        var oldButton = content.ElementAt(itemIndex) as Button;

                        oldButton.text = GetItemDisplayName(item);
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

            var lastFoldoutContent = Foldouts.ElementAt(foldoutIndex).Value;
            while (itemIndex < lastFoldoutContent.childCount)
                lastFoldoutContent.RemoveAt(itemIndex);

            foldoutIndex++;
            while (foldoutIndex < Foldouts.Count)
            {
                var foldout = Foldouts.ElementAt(foldoutIndex);
                if (contentNormal.Contains(foldout.Key))
                    contentNormal.Remove(foldout.Key);

                if (contentNormal.Contains(foldout.Value))
                    contentNormal.Remove(foldout.Value);

                Foldouts.RemoveForward(foldout.Key);
            }

            Select(_selectedButton != null && UiItems.Reverse.ContainsKey(_selectedButton) ?
                UiItems.Reverse[_selectedButton] :
                null);
        }

        void ButtonClicked(Button btn)
        {
            switch (IsSearching)
            {
                case true:
                    Select(_searchButtons.Reverse[btn], false);
                    break;
                case false:
                    if (Foldouts.Reverse.TryGetValue(btn.parent, out var foldout) &&
                        _tempFoldout == foldout)
                        _tempFoldout = null;

                    Select(UiItems.Reverse[btn], false);
                    break;
            }

        }

        void ChangeSelectedButton(Button btn)
        {
            if (_selectedButton != null)
                _selectedButton.RemoveFromClassList(SELECTED_CLASS);

            _selectedButton = btn;

            if (_selectedButton != null)
                _selectedButton.AddToClassList(SELECTED_CLASS);
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

            if (!IsSearching && UiItems.Forward.TryGetValue(item, out var uiItem))
            {
                ChangeSelectedButton(uiItem as Button);

                if (_tempFoldout != null &&
                    Foldouts.Reverse.TryGetValue(uiItem.parent, out var btnFoldout) &&
                    btnFoldout != _tempFoldout)
                {
                    _tempFoldout.value = false;

                    //If new button is below, move scroll so it will stay on the same level that it was clicked
                    if (Foldouts.Forward.IndexOf(x => x.Key == btnFoldout) > Foldouts.Forward.IndexOf(x => x.Key == _tempFoldout))
                        scroll.scrollOffset -= Vector2.up * Foldouts.Forward[_tempFoldout].worldBound.height;

                    _tempFoldout = null;
                }

                if (Foldouts.Reverse.TryGetValue(uiItem.parent, out var foldout))
                {
                    if (foldout.value == false)
                    {
                        foldout.value = true;
                        _tempFoldout = foldout;
                    }
                }

                if (autoScroll)
                {
                    //TODO: God forgive me
                    _onNextFrame += () =>
                    {
                        _onNextFrame += () =>
                        {
                            var rect = UiItems.Forward[item].worldBound;
                            var scrollRect = scroll.worldBound;
                            rect.position -= scrollRect.position;

                            if (rect.y < 0)
                                scroll.scrollOffset += Vector2.up * (rect.y);

                            if (rect.y + rect.height > scrollRect.height)
                                scroll.scrollOffset += Vector2.up * (rect.y + rect.height - scrollRect.height);
                        };
                    };
                }
            }

            if (IsSearching && _searchButtons.Forward.TryGetValue(item, out var btn))
            {
                ChangeSelectedButton(btn);
            }

            OnSelect?.Invoke(item);
        }

        string GetItemDisplayName(HierarchyItem item) =>
            TopMenuView.Sett_ShowIds ? item.id : item.displayText;
    }
}