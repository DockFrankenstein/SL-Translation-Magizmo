using Project.GUI.Settings;
using Project.Translation.ImportAndExport;
using System.Collections.Generic;
using UnityEngine;

namespace Project.GUI.Top
{
    public class TopMenuFile : TopMenu
    {
        [Space]
        [SerializeField] FilePropertiesWindow properties;

        [Label("Import And Export")]
        public List<ImportAndExportBase> importers = new List<ImportAndExportBase>();
        public List<ImportAndExportBase> exporters = new List<ImportAndExportBase>();

        protected override string ButtonName => "file";

        protected override void CreateMenu()
        {
            menu.AppendAction("Save", _ => manager.Save());
            menu.AppendAction("Save As", _ => manager.SaveAs());
            menu.AppendAction("Open", _ => manager.Open());
            menu.AppendSeparator();

            foreach (var item in importers)
                if (item is IImporter importer)
                    menu.AppendAction($"Import/{importer.Name}", _ => importer.BeginImport());

            foreach (var item in exporters)
                if (item is IExporter exporter)
                    menu.AppendAction($"Export/{exporter.Name}", _ => exporter.BeginExport());

            menu.AppendSeparator();
            menu.AppendAction("Properties", _ => properties.Open());
        }
    }
}