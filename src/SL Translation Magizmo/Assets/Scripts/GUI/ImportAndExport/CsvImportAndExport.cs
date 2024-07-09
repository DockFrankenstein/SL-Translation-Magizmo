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
using Project.Translation.Mapping.Manifest;
using Fab.UITKDropdown;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Translation.ImportAndExport
{
    public class CsvImportAndExport : ImportAndExportBase, IImporter, IExporter
    {
        const string IGNORE_NEXT_CONTENT = "###";
        const string ID_COLUMN_ID = "id";
        const string VALUE_COLUMN_ID = "value";

        public enum ColumnOrder
        {
            Id = 1,
            DisplayName = 2,
            OriginalTranslation = 3,
            Value = 4,
            DynamicValues = 5,
        }

        [SerializeField] HierarchyEntryProvider entryProvider;
        [SerializeField] ErrorWindow error;

        [Label("Exporting")]
        [SerializeField] UIDocument exportDocument;

        [Label("Importing")]
        [SerializeField] UIDocument importDocument;

        public Action OnExport;

        public string Name => "CSV";

        Button _exportButton;
        Button _exportCloseButton;
        TextField _exportPath;
        Button _exportPathOpen;
        Toggle _exportCreateCategories;
        AppReorderableList<ColumnOrder> _exportColumnsOrder;

        Button _importButton;
        Button _importCloseButton;
        TextField _importPath;
        Button _importPathOpen;
        DropdownField _importIdColumn;
        DropdownField _importValueColumn;
        ScrollView _importPreview;

        string _importFileTxt;
        bool _ignoreFirstTableRow;
        Table2D _currentImportTable;
        ErrorWindow.Prompt _importError;

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
            var exportRoot = exportDocument.rootVisualElement;
            exportRoot.ChangeDispaly(false);

            _exportButton = exportRoot.Q<Button>("export-button");
            _exportCloseButton = exportRoot.Q<Button>("close");
            _exportPath = exportRoot.Q<TextField>("path");
            _exportPathOpen = exportRoot.Q<Button>("path-open");
            _exportCreateCategories = exportRoot.Q<Toggle>("create-categories");
            _exportColumnsOrder = new AppReorderableList<ColumnOrder>(exportRoot.Q<ListView>("columns-order"), columnsOrder)
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
                                for (uint i = 0; i < 5; i++)
                                    table.SetCell(i, row, IGNORE_NEXT_CONTENT);

                                row++;

                                table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.Id), row, "Id");
                                table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.DisplayName), row, section.sectionName);
                                table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.OriginalTranslation), row, "Original");
                                table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.Value), row, "Translation");
                                table.SetCell((uint)columnsOrder.IndexOf(ColumnOrder.DynamicValues), row, "Dynamic Values");
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

                exportRoot.ChangeDispaly(false);
                OnExport?.Invoke();
            };

            _exportCloseButton.clicked += () =>
            {
                exportRoot.ChangeDispaly(false);
            };

            _exportPathOpen.clicked += () =>
            {
                var path = StandaloneFileBrowser.SaveFilePanel("", _exportPath.value, "translation", "csv");

                if (string.IsNullOrEmpty(path))
                    return;

                _exportPath.value = path;
            };

            _exportColumnsOrder.MakeItem += () => new Label();
            _exportColumnsOrder.OnBindItem += (e, i, v) => (e as Label).text = v switch
            {
                ColumnOrder.Id => "Id",
                ColumnOrder.DisplayName => "Display Name",
                ColumnOrder.OriginalTranslation => "Original Translation",
                ColumnOrder.Value => "Value",
                ColumnOrder.DynamicValues => "Dynamic Values",
                _ => "NONE",
            };

            //Importing
            var importRoot = importDocument.rootVisualElement;
            importRoot.ChangeDispaly(false);

            _importButton = importRoot.Q<Button>("import-button");
            _importCloseButton = importRoot.Q<Button>("close");
            _importPath = importRoot.Q<TextField>("path");
            _importPathOpen = importRoot.Q<Button>("path-open");
            _importIdColumn = importRoot.Q<DropdownField>("id-column");
            _importValueColumn = importRoot.Q<DropdownField>("value-column");
            _importPreview = importRoot.Q<ScrollView>("preview");

            _importButton.clicked += () =>
            {
                if (_currentImportTable == null)
                {
                    Import();

                    if (_importError != null)
                    {
                        error.CreatePrompt(_importError);
                        return;
                    }
                }

                if (_importIdColumn.index < 0 ||
                    _importIdColumn.index >= _currentImportTable.RowsCount)
                {
                    error.CreatePrompt("Import Error", "Please set a valid id column.");
                    return;
                }

                if (_importValueColumn.index < 0 ||
                    _importValueColumn.index >= _currentImportTable.RowsCount)
                {
                    error.CreatePrompt("Import Error", "Please set a valid value column.");
                    return;
                }

                bool _ignoreNext = false;
                for (int i = _ignoreFirstTableRow ? 1 : 0; i < _currentImportTable.RowsCount; i++)
                {
                    var id = _currentImportTable.GetCell(_importIdColumn.index, i);
                    var value = _currentImportTable.GetCell(_importValueColumn.index, i);

                    if (id == IGNORE_NEXT_CONTENT)
                    {
                        _ignoreNext = true;
                        i++;
                        continue;
                    }

                    if (_ignoreNext)
                    {
                        _ignoreNext = false;
                        continue;
                    }

                    //Ignore if id is not valid
                    if (!manager.CurrentVersion.MappedFields.ContainsKey(id))
                        continue;

                    if (!manager.File.Entries.ContainsKey(id))
                        manager.File.Entries.Add(id, new Data.SaveFile.EntryData(id));

                    manager.File.Entries[id].content = value;
                }

                FinalizeImport();
                importRoot.ChangeDispaly(false);
            };

            _importCloseButton.clicked += () =>
            {
                importRoot.ChangeDispaly(false);
            };

            _importPath.RegisterValueChangedCallback(args =>
            {
                if (args.target != _importPath) return;

                //Ignore if it's the same path
                if (args.previousValue == args.newValue)
                    return;

                var path = args.newValue;

                if (!File.Exists(path))
                {
                    ClearImportPreview();
                    return;
                }

                Import();

                UpdatePreview();

                for (int i = 0; i < _currentImportTable.ColumnsCount; i++)
                {
                    var content = _currentImportTable.GetCell(i, 0);

                    switch (content)
                    {
                        case ID_COLUMN_ID:
                            _importIdColumn.index = i;
                            break;
                        case VALUE_COLUMN_ID:
                            _importValueColumn.index = i;
                            break;
                    }
                }
            });

            _importPathOpen.clicked += () =>
            {
                var paths = StandaloneFileBrowser.OpenFilePanel("", _exportPath.value, "csv", false);

                if (paths.Length == 0)
                    return;

                _importPath.value = paths[0];
            };

            _importIdColumn.choices = new List<string>();
            _importValueColumn.choices = new List<string>();
        }

        void ClearImportPreview()
        {
            _importFileTxt = string.Empty;
            _currentImportTable = null;
            UpdatePreview();
        }

        void UpdatePreview()
        {
            _importPreview.Clear();
            _importIdColumn.choices.Clear();
            _importValueColumn.choices.Clear();

            if (_currentImportTable == null)
                return;

            var namesRow = new VisualElement()
                .WithClass("grid-row");
            _importPreview.Add(namesRow);

            namesRow.Add(CreateCell(string.Empty, "grid-row-name"));

            for (int i = 0; i < _currentImportTable.ColumnsCount; i++)
                namesRow.Add(CreateCell(Table2D.GetColumnName(i), "grid-column-name"));

            for (int row = 0; row < _currentImportTable.RowsCount; row++)
            {
                var rowElement = new VisualElement()
                    .WithClass("grid-row");
                _importPreview.Add(rowElement);

                rowElement.Add(CreateCell((row + 1).ToString(), "grid-row-name"));

                for (int column = 0; column < _currentImportTable.ColumnsCount; column++)
                    rowElement.Add(CreateCell(_currentImportTable.GetCell(column, row)));
            }

            for (int i = 0; i < _currentImportTable.ColumnsCount; i++)
            {
                var columnName = Table2D.GetColumnName(i);
                _importIdColumn.choices.Add(columnName);
                _importValueColumn.choices.Add(columnName);
            }


            VisualElement CreateCell(string content, string style = "grid-cell")
            {
                var label = new Label(content)
                {
                    enableRichText = false,
                };

                label.AddToClassList(style);
                return label;
            }
        }

        void Import()
        {
            _importError = null;

            try
            {
                var path = _importPath.value;

                using (var stream = new StreamReader(path))
                {
                    var txt = stream.ReadToEnd();
                    if (_importFileTxt == txt) return;

                    _importFileTxt = txt;
                    _currentImportTable = _parser.Deserialize(txt);
                }
            }
            catch (Exception e)
            {
                _importError = new ErrorWindow.Prompt("Import Error", $"There was an error while loading the file:\n{e}");
            }
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
                        new Item(ID_COLUMN_ID, VALUE_COLUMN_ID),
                        new Item(),
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
                    originalTranslation = manager.ComparisonManager.TryGetEntryData(hierarchyItem.id, out string content) ?
                        content :
                        string.Empty,
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
            importDocument.rootVisualElement.ChangeDispaly(true);
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