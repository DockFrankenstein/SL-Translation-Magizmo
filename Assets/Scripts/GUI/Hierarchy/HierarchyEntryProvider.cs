﻿using System.Linq;
using UnityEngine;
using qASIC;
using Project.Translation;

namespace Project.GUI.Hierarchy
{
    public sealed class HierarchyEntryProvider : HierarchyItemProvider
    {
        [SerializeField] TranslationManager manager;
        [SerializeField] MappingLayout[] mappingLayouts;

        public override HierarchyItem[] GetItems()
        {
            MappingLayout layout = mappingLayouts.FirstOrDefault();
            foreach (var item in mappingLayouts)
            {
                if (item.version.version > layout.version.version) 
                    break;

                layout = item;
            }

            if (layout == null)
                return new HierarchyItem[0];

            var fields = layout.version.containers
                .Where(x => !x.Hide)
                .SelectMany(x => x.GetMappedFields())
                .GroupBy(x => x.id)
                .Select(x => x.First());

            var items = layout.items
                .Select(x =>
                {
                    if (x.type == HierarchyItem.ItemType.Normal)
                    {
                        var field = fields.Where(y => y.id == x.id).FirstOrDefault();

                        if (field != null)
                        {
                            x = new HierarchyItem(x)
                            {
                                displayText = field.autoDisplayName ? PUtility.GenerateDisplayName(x.id) : x.displayText,
                            };
                        }
                    }

                    return x;
                });

            return items.ToArray();
        }

        private void Reset()
        {
            manager = FindObjectOfType<TranslationManager>();
        }
    }
}
