using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Fab.UITKDropdown;
using System;
using UnityEngine.Serialization;
using qASIC.Options;
using qASIC;
using qASIC.Input;
using Project.GUI.Top;
using Project.Undo;
using Project.Translation.Data;

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
        public HierarchySearchProvider[] searchProviders;

        [Header("Shortcuts")]
        [SerializeField] float repeatWaitTime = 1f;
        [SerializeField] float repeatTime = 0.1f;
        public InputMapItemReference i_previous;
        public InputMapItemReference i_next;

        [Header("Undo")]
        [SerializeField] UndoManager undo;

        ScrollView scroll;
        TextField search;
        VisualElement contentSearch;
        VisualElement contentNormal;

        public HierarchyItem SelectedItem { get; private set; }
        public string SelectedId => SelectedItem?.id;
        public event Action<HierarchyItem> OnSelect;

        public List<HierarchyItem> Items { get; private set; } = new List<HierarchyItem>();
        public List<HierarchyItem> SearchItems { get; private set; } = new List<HierarchyItem>();

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

        [Option("hierarchy_collapsed_default")]
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

            undo.OnUndo.AddListener(OnUndo);
            undo.OnRedo.AddListener(OnRedo);
        }

        void OnUndo()
        {
            var index = undo.GetHeadPosition();
            if (!undo.Steps.IndexInRange(index)) return;

            var step = undo.Steps[index];
            SelectFromUndoStep(step);
        }

        void OnRedo()
        {
            var index = undo.GetHeadPosition() - 1;
            if (!undo.Steps.IndexInRange(index)) return;

            var step = undo.Steps[index];
            SelectFromUndoStep(step);
        }

        void SelectFromUndoStep(UndoStep step)
        {
            if (step.Context is HierarchyItem obj && SelectedId != obj.id)
            {
                Select(obj.id);
            }

            if (step.Context is SaveFile.EntryData data && SelectedId != data.entryId)
            {
                Select(data.entryId);
            }
        }

        void UpdateSearch()
        {
            var searchPairs = CreateSearchPairs(search.value);

            bool prevIsSearching = IsSearching;
            IsSearching = searchPairs
                .Any(x => !string.IsNullOrWhiteSpace(x.Value));

            contentNormal.ChangeDispaly(!IsSearching);
            contentSearch.ChangeDispaly(IsSearching);

            _searchButtons.Clear();

            if (!IsSearching)
            {
                if (prevIsSearching)
                    ChangeSelectedButton(SelectedItem != null && UiItems.Forward.TryGetValue(SelectedItem, out var selectedEl) ?
                        selectedEl as Button :
                        null);
                return;
            }

            //When searching
            var items = Items
                .Where(x => x.type == HierarchyItem.ItemType.Normal)
                .AsEnumerable();

            var providersDictionary = searchProviders
                .SelectMany(x => x.Names.Select(y => new KeyValuePair<string, HierarchySearchProvider>(y, x)))
                .SelectMany(x => new KeyValuePair<string, HierarchySearchProvider>[] 
                { 
                    new KeyValuePair<string, HierarchySearchProvider>(x.Key, x.Value), 
                    new KeyValuePair<string, HierarchySearchProvider>($"{x.Key}*", x.Value) 
                })
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.First().Value);

            foreach (var pair in searchPairs)
            {
                if (!providersDictionary.TryGetValue(pair.Key, out var provider))
                    continue;

                switch (pair.Key.EndsWith("*"))
                {
                    case true:
                        items = items
                            .SortSearchListRegex(provider.GetSearchString, pair.Value);
                        break;
                    case false:
                        items = items
                            .SortSearchList(provider.GetSearchString, pair.Value);
                        break;
                }
            }

            var itemArray = items.ToArray();
            EnsureSearchItemCount(itemArray.Length);

            var buttons = contentSearch.Children().ToArray();

            for (int i = 0; i < buttons.Length; i++)
            {
                var exists = i < itemArray.Length;
                buttons[i].ChangeDispaly(exists);
                if (exists)
                {
                    var btn = buttons[i] as Button;
                    btn.text = GetItemDisplayName(itemArray[i]);
                    _searchButtons.Set(itemArray[i], btn);
                }
            }

            SearchItems = items.ToList();
            Select(SelectedItem);
        }

        List<KeyValuePair<string, string>> CreateSearchPairs(string searchString)
        {
            IEnumerable<string> searchStringItems = new string[] { searchString };

            string[] names = searchProviders
                .SelectMany(x => x.Names)
                .SelectMany(x => new string[] { $"{x}:", $"{x}*:" })
                .ToArray();

            foreach (var name in names)
                searchStringItems = searchStringItems
                    .SplitWithSplits(name);

            var items = searchStringItems
                .ToArray();

            var list = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < items.Length; i++)
            {
                var val = items[i];
                if (names.Contains(val))
                {
                    list.Add(new KeyValuePair<string, string>(val.Substring(0, val.Length - 1), string.Empty));
                    continue;
                }

                if (list.Count == 0)
                    list.Add(new KeyValuePair<string, string>("name", string.Empty));

                list[list.Count - 1] = new KeyValuePair<string, string>(list[list.Count - 1].Key, val);
            }

            return list;
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

            var items = (IsSearching ? SearchItems : Items)
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
                UiItems.Set(item, element);

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

                        Foldouts.Set(header, newContent);

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
                        Foldouts.Set(null, container);
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

            UpdateSearch();

            if (IsSearching)
                return;

            Select(_selectedButton != null && UiItems.Backward.ContainsKey(_selectedButton) ?
                UiItems.Backward[_selectedButton] :
                null);
        }

        void ButtonClicked(Button btn)
        {
            switch (IsSearching)
            {
                case true:
                    Select(_searchButtons.Backward[btn], false);
                    break;
                case false:
                    if (Foldouts.Backward.TryGetValue(btn.parent, out var foldout) &&
                        _tempFoldout == foldout)
                        _tempFoldout = null;

                    Select(UiItems.Backward[btn], false);
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

            if (!IsSearching && 
                item != null && 
                UiItems.Forward.TryGetValue(item, out var uiItem))
            {
                ChangeSelectedButton(uiItem as Button);

                if (_tempFoldout != null &&
                    Foldouts.Backward.TryGetValue(uiItem.parent, out var btnFoldout) &&
                    btnFoldout != _tempFoldout)
                {
                    _tempFoldout.value = false;

                    //If new button is below, move scroll so it will stay on the same level that it was clicked
                    if (Foldouts.Forward.IndexOf(x => x.Key == btnFoldout) > Foldouts.Forward.IndexOf(x => x.Key == _tempFoldout))
                        scroll.scrollOffset -= Vector2.up * Foldouts.Forward[_tempFoldout].worldBound.height;

                    _tempFoldout = null;
                }

                if (Foldouts.Backward.TryGetValue(uiItem.parent, out var foldout))
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

            if (IsSearching &&
                item != null &&
                _searchButtons.Forward.TryGetValue(item, out var btn))
            {
                ChangeSelectedButton(btn);
            }

            OnSelect?.Invoke(item);
        }

        string GetItemDisplayName(HierarchyItem item) =>
            TopMenuView.Sett_ShowIds ? item.id : item.displayText;
    }
}