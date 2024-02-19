using Project.UI;
using SFB;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System;
using System.Linq;
using Project.Serialization;
using Project.GUI.Hierarchy;
using Project.GUI.Inspector;
using JetBrains.Annotations;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using PlasticPipe.PlasticProtocol.Messages;
using Project.Translation.Mapping.Manifest;

namespace Project.Translation.ImportAndExport
{
    public class CsvExportAndImport : ImportAndExportBase, IImporter, IExporter
    {
        public enum ColumnOrder
        {
            Id = 0,
            DisplayName = 1,
            OriginalTranslation = 2,
            Value = 3,
            DynamicValues = 4,
        }

        [SerializeField] TranslationManager manager;
        [SerializeField] HierarchyEntryProvider entryProvider;
        [SerializeField] ErrorWindow error;

        [Label("Exporting")]
        [SerializeField] UIDocument exportDocument;

        public Action OnExport;

        public string Name => "CSV";

        Button _exportButton;
        Button _exportCloseButton;
        TextField _exportPath;
        Button _exportPathOpen;
        Toggle _exportCreateCategories;
        AppReorderableList<ColumnOrder> _exportColumnsOrder;

        List<ColumnOrder> columnsOrder = new List<ColumnOrder>()
        {
            ColumnOrder.Id,
            ColumnOrder.DisplayName,
            ColumnOrder.OriginalTranslation,
            ColumnOrder.Value,
            ColumnOrder.DynamicValues,
        };

        CsvParser _parser = new CsvParser();

        private void Awake()
        {
            var root = exportDocument.rootVisualElement;
            root.ChangeDispaly(false);

            _exportButton = root.Q<Button>("export-button");
            _exportCloseButton = root.Q<Button>("close");
            _exportPath = root.Q<TextField>("path");
            _exportPathOpen = root.Q<Button>("path-open");
            _exportCreateCategories = root.Q<Toggle>("create-categories");
            _exportColumnsOrder = new AppReorderableList<ColumnOrder>(root.Q<ListView>("columns-order"), columnsOrder)
            {
                RemoveButtonPosition = Position.Absolute,
            };

            _exportButton.clicked += () =>
            {
                var path = _exportPath.value;

                if (string.IsNullOrWhiteSpace(path))
                {
                    error.CreatePrompt("Invalid Path", "Please set an export path.");
                    return;
                }

                if (new ColumnOrder[]
                    {
                        ColumnOrder.Id,
                        ColumnOrder.DisplayName,
                        ColumnOrder.OriginalTranslation,
                        ColumnOrder.Value,
                        ColumnOrder.DynamicValues,
                    }.Except(columnsOrder).Count() > 0)
                {
                    error.CreatePrompt("Export Error", "There are missing items in the column order. This is an application error, please report this issue.");
                    return;
                }

                try
                {
                    Table2D table = new Table2D();
                    uint row = 0;
                    var sections = ProvideItems();

                    foreach (var section in sections)
                    {
                        //Create category header
                        if (_exportCreateCategories.value)
                        {
                            if (row != 0) row++;
                            if (!string.IsNullOrEmpty(section.sectionName))
                            {
                                table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.DisplayName), row, section.sectionName);
                                row++;
                            }
                        }

                        foreach (var item in section.items)
                        {
                            table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.Id), row, item.id);
                            table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.DisplayName), row, item.displayName);
                            table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.OriginalTranslation), row, item.originalTranslation);
                            table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.Value), row, item.value);
                            table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.DynamicValues), row, item.dynamicValues);
                            row++;
                        }
                    }

                    var txt = _parser.Serialize(table);

                    File.WriteAllText(path, txt);
                }
                catch (Exception e)
                {
                    error.CreatePrompt("Export Error", $"There was a problem while exporting to CSV.\n{e}");
                    return;
                }

                root.ChangeDispaly(false);
                OnExport?.Invoke();
            };

            _exportCloseButton.clicked += () =>
            {
                root.ChangeDispaly(false);
            };

            _exportPathOpen.clicked += () =>
            {
                var path = StandaloneFileBrowser.SaveFilePanel("", _exportPath.value, "translation", "csv");

                if (string.IsNullOrEmpty(path))
                    return;

                _exportPath.value = path;
            };

            _exportColumnsOrder.MakeItem += () => new Label();
            _exportColumnsOrder.OnBindItem += (e, v) => (e as Label).text = v switch
            {
                ColumnOrder.Id => "Id",
                ColumnOrder.DisplayName => "Display Name",
                ColumnOrder.OriginalTranslation => "Original Translation",
                ColumnOrder.Value => "Value",
                ColumnOrder.DynamicValues => "Dynamic Values",
                _ => "NONE",
            };
        }

        List<Section> ProvideItems()
        {
            var layout = entryProvider.GetCurrentLayout();

            if (layout == null)
                return new List<Section>();

            var sections = new List<Section>()
            {
                new Section()
                {
                    items = new List<Item>()
                    {
                        new Item("sl_version", manager.CurrentVersion.version.ToString()),
                    },
                }
            };

            Section currentSection = null;

            foreach (var item in layout.items)
            {
                switch (item.type)
                {
                    case HierarchyItem.ItemType.Header:
                        if (currentSection == null)
                        {
                            currentSection = new Section(item.displayText);
                            break;
                        }

                        sections.Add(currentSection);
                        currentSection = new Section(item.displayText);
                        break;
                    case HierarchyItem.ItemType.Normal:
                        if (currentSection == null)
                        {
                            currentSection = new Section();
                        }

                        //Manifest
                        if (item.id == ManifestInspector.MANIFEST_ITEM_ID)
                        {
                            var manifest = manager.CurrentVersion.containers
                                .Where(x => x is ManifestMappingBase)
                                .Select(x => x as ManifestMappingBase)
                                .FirstOrDefault();

                            if (manifest == null)
                                break;

                            var fields = manifest.GetMappedFields();

                            foreach (var field in fields)
                            {
                                currentSection.items.Add(CreateItem(new HierarchyItem(field.id, field.GetFinalName())
                                {
                                    type = HierarchyItem.ItemType.Normal,
                                }));
                            }

                            break;
                        }

                        currentSection.items.Add(CreateItem(item));
                        break;
                }
            }

            if (currentSection != null)
                sections.Add(currentSection);

            return sections;


            Item CreateItem(HierarchyItem hierarchyItem)
            {
                var item = new Item()
                {
                    id = hierarchyItem.id,
                    originalTranslation = string.Empty, //TODO: implement this when we start supporting previewing
                };

                if (manager.File.Entries.TryGetValue(hierarchyItem.id, out var val))
                    item.value = val.content;

                if (manager.CurrentVersion.MappedFields.TryGetValue(hierarchyItem.id, out var field))
                {
                    item.displayName = field.GetFinalName();

                    var lines = field.dynamicValues
                        .Select(x => $"{x.tag} - {x.description}");

                    item.dynamicValues = string.Join("\n", lines);
                }

                return item;
            }
        }

        public void BeginExport()
        {
            exportDocument.rootVisualElement.ChangeDispaly(true);
        }

        public void BeginImport()
        {
            
        }

        public class Section
        {
            public Section() { }
            public Section(string sectionName)
            {
                this.sectionName = sectionName;
            }

            public string sectionName;
            public List<Item> items = new List<Item>();
        }

        public class Item
        {
            public Item() { }
            public Item(string id) : this(id, string.Empty, string.Empty, string.Empty, string.Empty) { }
            public Item(string id, string value) : this(id, string.Empty, string.Empty, value, string.Empty) { }
            public Item(string id, string value, string displayName) : this(id, displayName, string.Empty, value, string.Empty) { }

            public Item(string id, string displayName, string originalTranslation, string value, string dynamicValues)
            {
                this.id = id;
                this.displayName = displayName;
                this.originalTranslation = originalTranslation;
                this.value = value;
                this.dynamicValues = dynamicValues;
            }

            public string id;
            public string displayName;
            public string originalTranslation;
            public string value;
            public string dynamicValues;
        }
    }
}