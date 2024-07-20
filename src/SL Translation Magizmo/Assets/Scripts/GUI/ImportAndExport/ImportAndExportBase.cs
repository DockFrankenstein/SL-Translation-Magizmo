using Project.Translation;
using Project.Undo;
using UnityEngine;

namespace Project.GUI.ImportAndExport
{
    public class ImportAndExportBase : MonoBehaviour
    {
        public ImportAndExportManager ImportAndExportManager { get; internal set; }
        public TranslationManager TranslationManager { get; internal set; }
        public UndoManager Undo { get; internal set; }
        public ErrorWindow Error { get; internal set; }
        public NotificationManager Notifications { get; internal set; }

        public string ImportPath { get; set; }
        public string ExportPath { get; set; }

        public void FinalizeImport()
        {
            
        }

        public void FinalizeExport()
        {
            Notifications.NotifyExport(ExportPath);
        }
    }
}