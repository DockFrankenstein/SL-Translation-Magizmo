using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using Project.Undo;

namespace Project.UI
{
    public class AppReorderableList<T>
    {
        Dictionary<VisualElement, int> fields = new Dictionary<VisualElement, int>();

        public AppReorderableList(ListView list) : this(list, new List<T>()) { }

        public AppReorderableList(ListView list, List<T> source)
        {
            List = list;
            Source = source;

            List.itemsSource = source;
            List.makeItem += CreateItem;

            List.destroyItem += (a) =>
            {
                if (fields.ContainsKey(a))
                    fields.Remove(a);

                OnDestroyItem?.Invoke(a.Children().FirstOrDefault());
            };

            List.bindItem += (e, i) =>
            {
                fields[e] = i;
                var val = Source[i];

                var item = e.Children().FirstOrDefault();
                var field = GetField == null ?
                    item as BaseField<T> :
                    GetField(item);

                if (field != null)
                {
                    field.SetValueWithoutNotify(val);
                }

                OnBindItem?.Invoke(item, i, val);
            };

            List.unbindItem += (e, i) =>
            {
                fields[e] = -1;
                OnUnbindItem?.Invoke(e.Children().FirstOrDefault());
            };

            List.itemsAdded += _ =>
            {
                _lastEditedItem = -1;
                OnUndoEvent?.Invoke();
                OnChanged?.Invoke();
            };

            List.itemsRemoved += _ =>
            {
                _lastEditedItem = -1;
                OnUndoEvent?.Invoke();
                OnChanged?.Invoke();
            };

            List.itemIndexChanged += (_, _) =>
            {
                _lastEditedItem = -1;
                OnUndoEvent?.Invoke();
                OnChanged?.Invoke();
            };
        }

        public IEnumerable<VisualElement> GetElements() =>
            fields
            .OrderBy(x => x.Value)
            .Select(x => x.Key.Children().FirstOrDefault());

        public Position RemoveButtonPosition { get; set; } = Position.Relative;

        VisualElement CreateItem()
        {
            var root = new VisualElement();
            var item = MakeItem();

            root.style.flexDirection = FlexDirection.Row;

            root.Add(item);

            item.style.flexGrow = 1f;
            item.style.flexShrink = 1f;

            if (List.showAddRemoveFooter)
            {
                var removeButton = new Button();

                removeButton.AddToClassList("minus-button");

                removeButton.style.position = RemoveButtonPosition;
                removeButton.style.right = 0f;
                removeButton.style.width = 42f;
                removeButton.style.height = 42f;
                removeButton.transform.scale = Vector3.one * 0.6f;
                removeButton.style.alignSelf = Align.Center;

                removeButton.clicked += () =>
                {
                    Source.RemoveAt(fields[root]);
                    List.RefreshItems();
                    _lastEditedItem = -1;
                    OnUndoEvent?.Invoke();
                    OnChanged?.Invoke();
                };

                root.Add(removeButton);
            }

            fields.Add(root, -1);

            var field = GetField == null ?
                item as BaseField<T> :
                GetField(item);

            if (field != null)
            {
                field.RegisterValueChangedCallback(args =>
                {
                    if (args.target == field)
                    {
                        var index = fields[root];
                        Source[index] = field.value;

                        if (_lastEditedItem != index)
                        {
                            _lastEditedItem = index;
                            OnUndoEvent?.Invoke();
                        }

                        OnChanged?.Invoke();
                    }
                });
            }

            return root;
        }

        public ListView List { get; private set; }

        public event Func<VisualElement> MakeItem;
        public event Action<VisualElement> OnDestroyItem;
        public event Func<VisualElement, BaseField<T>> GetField;
        public event Action<VisualElement, int, T> OnBindItem;
        public event Action<VisualElement> OnUnbindItem;
        public event Action OnChanged;
        public event Action OnUndoEvent;

        int _lastEditedItem = -1;

        List<T> _source;
        public List<T> Source
        {
            get => _source;
            set
            {
                _source = value;
                List.itemsSource = _source;
            }
        }
    }
}