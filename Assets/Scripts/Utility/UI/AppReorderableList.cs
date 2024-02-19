using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

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

                if (e is BaseField<T> valItem)
                    valItem.SetValueWithoutNotify(val);

                OnBindItem?.Invoke(e, val);
            };

            List.unbindItem += (e, i) =>
            {
                fields[e] = -1;
            };

            List.itemsAdded += _ => OnChanged?.Invoke();
            List.itemsRemoved += _ => OnChanged?.Invoke();
            List.itemIndexChanged += (_, _) => OnChanged?.Invoke();
        }

        public Position RemoveButtonPosition { get; set; } = Position.Relative;

        VisualElement CreateItem()
        {
            var item = MakeItem();

            item.style.marginBottom = 0f;
            item.style.marginLeft = 0f;
            item.style.marginRight = 0f;
            item.style.marginTop = 0f;
            item.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var btn = new Button();

            btn.AddToClassList("minus-button");

            btn.style.position = RemoveButtonPosition;
            btn.style.right = 0f;
            btn.style.width = 42f;
            btn.style.height = 42f;
            btn.transform.scale = Vector3.one * 0.6f;

            item.style.height = 42f;
            var transformVal = item.style.translate.value;
            transformVal.y = 4f;
            item.transform.position += new Vector3(0f, 4f);

            if (List.showAddRemoveFooter)
                item.Add(btn);

            fields.Add(item, -1);

            if (item is BaseField<T> valItem)
            {
                valItem.RegisterValueChangedCallback(args =>
                {
                    if (args.target == valItem)
                    {
                        Source[fields[item]] = valItem.value;
                        OnChanged?.Invoke();
                    }
                });
            }

            btn.clicked += () =>
            {
                Source.RemoveAt(fields[item]);
                List.RefreshItems();
                OnChanged?.Invoke();
            };

            return item;
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