using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.UI;
using qASIC;
using System.Linq;
using System;
using Project.Translation.Defines;

namespace Project.Translation.UI
{
    public class HierarchyDisplay : MonoBehaviour
    {
        [SerializeField] Transform itemParent;

        [Label("Items")]
        [Prefab] public HierarchyItemDisplay normalPrefab;
        [Prefab] public HierarchyItemDisplay separatorPrefab;
        [Prefab] public HierarchyItemDisplay headerPrefab;

        [Label("Providers")]
        [SerializeField] HierarchyItemProvider[] providers;

        public HierarchyItemDisplay SelectedItemDisplay { get; private set; } = null;
        public HierarchyItem SelectedItem => SelectedItemDisplay?.Item;
        public string SelectedId => SelectedItem?.id;

        public event Action<string> OnSelect;

        public List<HierarchyItemDisplay> Items { get; private set; } = new List<HierarchyItemDisplay>();

        private void Awake()
        {
            PopulateList();
        }

        public void PopulateList()
        {
            while (Items.Count > 0)
                Destroy(Items[0]);

            var items = providers
                .SelectMany(x => x.GetItems());

            foreach (var item in items)
            {
                HierarchyItemDisplay scrollItem;
                switch (item.type)
                {
                    default:
                        scrollItem = Instantiate(normalPrefab, itemParent);
                        scrollItem.UpdateDisplay(item);
                        scrollItem.OnSelected += x =>
                        {
                            if (!x)
                            {
                                scrollItem.ChangeStateSilent(true);
                                return;
                            }

                            Select(scrollItem);
                        };

                        break;
                    case HierarchyItem.ItemType.Separator:
                        scrollItem = Instantiate(separatorPrefab, itemParent);
                        break;
                    case HierarchyItem.ItemType.Header:
                        scrollItem = Instantiate(headerPrefab, itemParent);
                        scrollItem.UpdateDisplay(item);
                        break;
                }

                Items.Add(scrollItem);
            }
        }

        public void Select(string id)
        {
            var item = Items
                .Where(x => x.Item.id == id)
                .FirstOrDefault();

            if (item != null)
                Select(item);
        }

        public void Select(HierarchyItemDisplay item)
        {
            if (SelectedItemDisplay != null)
                SelectedItemDisplay.ChangeStateSilent(false);

            SelectedItemDisplay = item;
            SelectedItemDisplay.ChangeStateSilent(true);
            OnSelect?.Invoke(SelectedId);
        }
    }
}