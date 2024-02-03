using System.Collections.Generic;
using UnityEngine;
using qASIC;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Project.Utility.UI
{
    public class ReorderableListUI : MonoBehaviour
    {
        public RectTransform listTransform;
        public RectTransform itemsHolder;
        public RectTransform headerTransform;

        [Space]
        public float headerHeight = 35f;
        public float headerSpacing = 2f;
        public float additionalHeight = 4f;
        public float minimumItemsHolderHeight = 40f;

        [Space]
        public ReorderableListUIItem itemTemplate;
        private List<ReorderableListUIItem> items = new List<ReorderableListUIItem>();

        [Label("Values")]
        //[OnValueChanged(nameof(UpdateItems))]
        //[EditorButton(nameof(UpdateItems))]
        [SerializeField] List<string> values = new List<string>();

        [Label("Events")]
        public UnityEvent OnChange;

        public List<string> Values
        {
            get => values;
            set
            {
                values = value;
                UpdateItems();
            }
        }

        public void ChangeValuesWithoutNotify(List<string> values)
        {
            this.values = values;
            UpdateItems(true);
        }

        bool _init = false;

        private void Awake()
        {
            if (itemTemplate != null)
            {
                itemTemplate.gameObject.SetActive(false);
                foreach (var value in values)
                {
                    var listItem = CreateNewItem();
                    listItem.input.SetTextWithoutNotify(value);
                }
            }

            _init = true;
        }

        private void Update()
        {
            if (headerTransform != null)
                headerTransform.sizeDelta = new Vector2(headerTransform.sizeDelta.x, headerHeight);

            if (itemsHolder != null)
                itemsHolder.offsetMax = new Vector2(itemsHolder.offsetMax.x, -headerHeight - headerSpacing);

            if (listTransform != null && itemsHolder != null)
            {
                var childHeight = 0f;
                for (int i = 0; i < itemsHolder.childCount; i++)
                {
                    var child = itemsHolder.GetChild(i) as RectTransform;
                    if (!child.gameObject.activeInHierarchy) continue;
                    childHeight += child.sizeDelta.y;
                }

                var newSize = new Vector2(listTransform.sizeDelta.x, headerHeight + headerSpacing + additionalHeight +
                    Mathf.Max(childHeight, minimumItemsHolderHeight));

                bool changed = newSize != listTransform.sizeDelta;
                listTransform.sizeDelta = newSize;

                if (changed)
                    LayoutGroupController.Refresh();
            }
        }

        public void UpdateItems(bool silent = false)
        {
            if (!_init)
                return;

            if (values.Count > items.Count)
            {
                var times = values.Count - items.Count;
                for (int i = 0; i < times; i++)
                    CreateNewItem();
            }


            while (items.Count > values.Count)
            {
                var item = items[values.Count];
                items.Remove(item);
                Destroy(item.gameObject);
            }

            for (int i = 0; i < items.Count; i++)
            {
                items[i].input.SetTextWithoutNotify(values[i]);
                items[i].upButton.interactable = i != 0;
                items[i].downButton.interactable = i < items.Count - 1;
            }

            if (!silent)
                OnChange.Invoke();
        }

        ReorderableListUIItem CreateNewItem()
        {
            var listItem = Instantiate(itemTemplate, itemsHolder);
            listItem.gameObject.SetActive(true);
            listItem.reorderableList = this;
            items.Add(listItem);
            return listItem;
        }

        public void MoveItem(ReorderableListUIItem item, int amount)
        {
            int currentIndex = items.IndexOf(item);
            int newIndex = currentIndex + amount;
            newIndex = Mathf.Clamp(newIndex, 0, items.Count - 1);

            if (currentIndex == newIndex)
                return;

            var value = values[currentIndex];
            values[currentIndex] = null;
            values.Insert(newIndex < currentIndex ? newIndex : newIndex + 1, value);
            values.RemoveAt(newIndex < currentIndex ? currentIndex + 1 : currentIndex);

            UpdateItems();
        }

        public void DeleteItem(ReorderableListUIItem item)
        {
            var index = items.IndexOf(item);
            if (!values.IndexInRange(index))
                return;

            values.RemoveAt(index);
            UpdateItems();
        }

        public void UpdateItem(ReorderableListUIItem item)
        {
            var index = items.IndexOf(item);
            values[index] = item.input.text;
            UpdateItems();
        }

        public void AddValue()
        {
            values.Add(string.Empty);
            UpdateItems();
        }
    }
}