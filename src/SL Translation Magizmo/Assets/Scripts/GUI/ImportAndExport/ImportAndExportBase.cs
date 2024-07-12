using Project.Undo;
using UnityEngine;

namespace Project.Translation.ImportAndExport
{
    public class ImportAndExportBase : MonoBehaviour
    {
        [SerializeField] protected TranslationManager manager;
        [SerializeField] protected UndoManager undo;

        public void FinalizeImport()
        {
            undo.ClearDirty();
        }
    }
}