using UnityEngine;

namespace Project.Translation.ImportAndExport
{
    public class ImportAndExportBase : MonoBehaviour
    {
        [SerializeField] protected TranslationManager manager;

        public void FinalizeImport()
        {
            manager.MarkFileDirty(this);
        }
    }
}