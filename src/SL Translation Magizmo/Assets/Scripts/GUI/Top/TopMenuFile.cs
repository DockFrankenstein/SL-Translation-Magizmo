using Project.GUI.ImportAndExport;
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
        [SerializeField] ImportAndExportManager importAndExport;

        protected override string ButtonName => "file";

        protected override void CreateMenu()
        {
            menu.AppendAction("Save", _ => manager.Save(), DropdownMenuAction.AlwaysEnabled, CreateDataForInput(prompts, manager.i_save));
            menu.AppendAction("Save As", _ => manager.SaveAs(), DropdownMenuAction.AlwaysEnabled, CreateDataForInput(prompts, manager.i_saveAs));
            menu.AppendAction("Open", _ => manager.Open(), DropdownMenuAction.AlwaysEnabled, CreateDataForInput(prompts, manager.i_load));

            if (manager.RecentFiles != null)
                foreach (var item in manager.RecentFiles.Take(10))
                    menu.AppendAction($"Open Recent/{item.Replace("/", "\\").Replace("\\", "\\\\")}", _ => manager.Open(item));

            menu.AppendSeparator();

            switch (importAndExport.QuickExportAvaliable)
            {
                case false:
                    menu.AppendAction("Export", _ => { }, DropdownMenuAction.AlwaysDisabled, userData: CreateDataForInput(prompts, importAndExport.i_quickExport));
                    break;
                case true:
                    menu.AppendAction($"Export {importAndExport.LastExporter.Name}", _ => importAndExport.QuickExport(), DropdownMenuAction.AlwaysEnabled, userData: CreateDataForInput(prompts, importAndExport.i_quickExport));
                    break;
            }


            foreach (var item in importAndExport.Components)
                if (item is IExporter exporter)
                    menu.AppendAction($"Export As/{exporter.Name}", _ => importAndExport.BeginExport(exporter));

            foreach (var item in importAndExport.Components)
                if (item is IImporter importer)
                    menu.AppendAction($"Import/{importer.Name}", _ => importAndExport.BeginImport(importer));

            menu.AppendSeparator();
            menu.AppendAction("Properties", _ => properties.Open());
        }
    }
}