using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            List.makeItem += () =>
            {
                var item = MakeItem();

                var btn = new Button();

                btn.AddToClassList("minus-button");

                btn.style.width = 42f;
                btn.style.height = 42f;
                btn.transform.scale = Vector3.one * 0.6f;

                item.style.height = 42f;
                var transformVal = item.style.translate.value;
                transformVal.y = 4f;
                item.transform.position += new Vector3(0f, 4f);

                item.Add(btn);
                fields.Add(item, -1);

                item.RegisterValueChangedCallback(args =>
                {
                    if (args.target == item)
                    {
                        Source[fields[item]] = item.value;
                        OnChanged?.Invoke();
                    }
                });

                btn.clicked += () =>
                {
                    Source.RemoveAt(fields[item]);
                    List.RefreshItems();
                    OnChanged?.Invoke();
                };

                return item;
            };

            List.destroyItem += (a) =>
            {
                if (fields.ContainsKey(a))
                    fields.Remove(a);
            };

            List.bindItem += (e, i) =>
            {
                fields[e] = i;
                (e as BaseField<T>).SetValueWithoutNotify(Source[i]);
            };

            List.unbindItem += (e, i) =>
            {
                fields[e] = -1;
            };

            List.itemsAdded += _ => OnChanged?.Invoke();
            List.itemsRemoved += _ => OnChanged?.Invoke();
            List.itemIndexChanged += (_, _) => OnChanged?.Invoke();
        }

        public ListView List { get; private set; }

        public Func<BaseField<T>> MakeItem;
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