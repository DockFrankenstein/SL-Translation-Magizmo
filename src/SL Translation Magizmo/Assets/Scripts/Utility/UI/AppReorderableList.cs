using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

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
            };

            List.bindItem += (e, i) =>
            {
                fields[e] = i;
                var val = Source[i];

                if (e.Children().FirstOrDefault() is BaseField<T> valItem)
                    valItem.SetValueWithoutNotify(val);

                OnBindItem?.Invoke(e, val);
            };

            List.unbindItem += (e, i) =>
            {
                fields[e] = -1;
            };

            List.itemsAdded += _ =>
            {
                OnChanged?.Invoke();
            };
            List.itemsRemoved += _ => OnChanged?.Invoke();
            List.itemIndexChanged += (_, _) => OnChanged?.Invoke();
        }

        public Position RemoveButtonPosition { get; set; } = Position.Relative;

        VisualElement CreateItem()
        {
            var root = new VisualElement();
            var item = MakeItem();

            root.style.flexDirection = FlexDirection.Row;

            root.Add(item);

            //item.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
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
                    OnChanged?.Invoke();
                };

                root.Add(removeButton);
            }

            fields.Add(root, -1);

            if (item is BaseField<T> valItem)
            {
                valItem.RegisterValueChangedCallback(args =>
                {
                    if (args.target == valItem)
                    {
                        Source[fields[root]] = valItem.value;
                        OnChanged?.Invoke();
                    }
                });
            }

            return root;
        }

        public ListView List { get; private set; }

        public Func<VisualElement> MakeItem;
        public Action<VisualElement, T> OnBindItem;
        public Action OnChanged;

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