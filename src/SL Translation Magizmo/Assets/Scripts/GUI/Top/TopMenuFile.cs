using Project.GUI.Settings;
using Project.Translation.ImportAndExport;
using qASIC.Input.Prompts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.GUI.Top
{
    public class TopMenuFile : TopMenu
    {
        [Space]
        [SerializeField] FilePropertiesWindow properties;
        [SerializeField] PromptLibrary prompts;

        [Label("Import And Export")]
        public List<ImportAndExportBase> importers = new List<ImportAndExportBase>();
        public List<ImportAndExportBase> exporters = new List<ImportAndExportBase>();

        protected override string ButtonName => "file";

        protected override void CreateMenu()
        {
            menu.AppendAction("Save", _ => manager.Save(), DropdownMenuAction.AlwaysEnabled, CreateDataForInput(prompts, manager.i_save));
            menu.AppendAction("Save As", _ => manager.SaveAs(), DropdownMenuAction.AlwaysEnabled, userData: CreateDataForInput(prompts, manager.i_saveAs));
            menu.AppendAction("Open", _ => manager.Open(), DropdownMenuAction.AlwaysEnabled, CreateDataForInput(prompts, manager.i_load));

            if (manager.RecentFiles != null)
                foreach (var item in manager.RecentFiles.Take(10))
                    menu.AppendAction($"Open Recent/{item.Replace("/", "\\").Replace("\\", "\\\\")}", _ => manager.Open(item));

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