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
    public class TranslationDefinesDisplay : MonoBehaviour
    {
        [SerializeField] TranslationManager manager;
        [Prefab] public ScrollViewButton scrollItemPrefab;
        [SerializeField] Transform itemParent;

        public TranslationVersion version;

        public Dictionary<string, ScrollViewButton> Buttons { get; set; } = new Dictionary<string, ScrollViewButton>();

        private void Awake()
        {
            PopulateList();
        }

        public void PopulateList()
        {
            var items = version.GetDefines();

            foreach (var item in items)
            {
                var scrollItem = Instantiate(scrollItemPrefab, itemParent);
                scrollItem.text.text = item;
                scrollItem.OnSelected += x =>
                {
                    if (!x)
                    {
                        scrollItem.ChangeStateSilent(true);
                        return;
                    }

                    if (manager.SelectedItem != null && Buttons.ContainsKey(manager.SelectedItem))
                        Buttons[manager.SelectedItem].ChangeStateSilent(false);

                    manager.SelectedItem = item;
                };

                Buttons.Add(item, scrollItem);
            }
        }
    }
}