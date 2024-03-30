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
            installer.Updater.Progress.ProgressChanged += (_, x) => Progress = x;
        }

        public override ButtonData BackButton => ButtonData.ForceDisabled;
        public override ButtonData NextButton => ButtonData.ForceDisabled;

        InstallerApp installer;

        //public event Action OnBegin;
        //public event Action OnFinish;

        private float _progress;
        public float Progress
        { 
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        public override void OnOpenPage()
        {
            Progress = 0f;
            //_ = DownloadUpdate();
        }

        //public override void OnClosePage()
        //{
        //}

        //async Task DownloadUpdate()
        //{
        //    OnBegin?.Invoke();

        //    installer.Updater.Progress.ProgressChanged += (_, x) => Progress = x;
        //    await installer.Install();
        //    installer.Updater.Progress.ProgressChanged -= (_, x) => Progress = x;

        //    OnFinish?.Invoke();
        //}
    }
}