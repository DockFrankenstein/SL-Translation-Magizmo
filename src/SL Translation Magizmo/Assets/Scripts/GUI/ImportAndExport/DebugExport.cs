using Project.Settings;
using SFB;
using System;
using System.Linq;
using Project.Translation;
using Project.Translation.ImportAndExport;

namespace Project.GUI.ImportAndExport
{
    public class DebugExport : ImportAndExportBase, IExporter
    {
        public event Action OnExport;

        public string Name => "Debug";

        public void BeginExport()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            ExportPath = paths[0];
            try
            {
                Export();
            }
            catch (Exception e)
            {
                Error.CreateExportExceptionPrompt(e);
            }
        }

        public void Export()
        {
            TranslationManager.CurrentVersion.Export(TranslationManager.File, ExportPath, args =>
            {
                return $"{string.Join(".", args.container.fileName.Split('.').SkipLast(1))}_{args.index + 1}";
            });

            FinalizeExport();
            OnExport?.Invoke();
        }
    }
}