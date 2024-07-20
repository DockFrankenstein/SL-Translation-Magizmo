using UnityEngine;
using SFB;
using System;
using System.Linq;

namespace Project.Translation.ImportAndExport
{
    public class DebugExport : ImportAndExportBase, IExporter
    {
        [SerializeField] NotificationManager notifications;
        [SerializeField] ErrorWindow error;

        public event Action OnExport;

        public string Name => "Debug";

        public void BeginExport()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", Settings.GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            ExportPath = paths[0];
            try
            {
                Export();
            }
            catch (Exception e)
            {
                error.CreateExportExceptionPrompt(e);
            }
        }

        public void Export()
        {
            manager.CurrentVersion.Export(manager.File, ExportPath, args =>
            {
                return $"{string.Join(".", args.container.fileName.Split('.').SkipLast(1))}_{args.index + 1}";
            });

            notifications.NotifyExport(ExportPath);
            OnExport?.Invoke();
        }
    }
}