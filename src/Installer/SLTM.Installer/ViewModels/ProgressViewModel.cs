using IWshRuntimeLibrary;
using ReactiveUI;
using SLTM.Installer.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SLTM.Installer.ViewModels
{
    public class ProgressViewModel : PageViewModelBase
    {
        public ProgressViewModel(InstallerApp installer)
        {
            this.installer = installer;
        }

        public override ButtonData BackButton => ButtonData.ForceDisabled;
        public override ButtonData NextButton => ButtonData.ForceDisabled;

        InstallerApp installer;

        public event Action<Exception> OnDownloadError;
        public event Action OnFinish;

        private float _progress;
        public float Progress
        { 
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        public override void OnOpenPage()
        {
            _ = DownloadUpdate();
        }

        async Task DownloadUpdate()
        {
            installer.Updater.Progress.ProgressChanged += (_, x) => Progress = x;

            try
            {
                await installer.Updater.DownloadUpdate();
                OnFinish?.Invoke();

                var exePath = $"{installer.Updater.OutputPath}/SL Translation Magizmo/SL Translation Magizmo.exe"
                    .Replace('\\', '/');

                if (installer.CreateDesktopShortcut)
                {
                    var shortcutPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/SL Translation Magizmo.lnk";

                    var shell = new WshShell();
                    var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                    shortcut.TargetPath = exePath;
                    shortcut.Save();
                }
            }
            catch (Exception e)
            {
                OnDownloadError?.Invoke(e);
            }

            installer.Updater.Progress.ProgressChanged -= (_, x) => Progress = x;
        }
    }
}