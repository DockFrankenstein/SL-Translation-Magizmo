using UnityEngine;
using SFB;
using System;
using System.Linq;

namespace Project.Translation.ImportAndExport
{
    public class DebugExport : ImportAndExportBase, IExporter
    {
        [SerializeField] NotificationManager notifications;

        public event Action OnExport;

        public string Name => "Debug";

        public void BeginExport()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("", Settings.GeneralSettings.TranslationPath, false);

            if (paths.Length == 0)
                return;

            manager.CurrentVersion.Export(manager.File, paths[0], args =>
            {
                return $"{string.Join(".", args.container.fileName.Split('.').SkipLast(1))}_{args.index + 1}";
            });

            notifications.NotifyExport(paths[0]);
            OnExport?.Invoke();
        }
    }
}