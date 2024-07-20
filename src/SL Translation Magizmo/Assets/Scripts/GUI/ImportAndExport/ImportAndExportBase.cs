using Project.Undo;
using UnityEngine;

namespace Project.Translation.ImportAndExport
{
    public class ImportAndExportBase : MonoBehaviour
    {
        [SerializeField] protected TranslationManager manager;
        [SerializeField] protected UndoManager undo;

        public string ImportPath { get; set; }
        public string ExportPath { get; set; }

        public void FinalizeImport()
        {
            undo.ClearDirty();
        }
    }
}