using SLTM.Installer.Services;
using System;

namespace SLTM.Installer.ViewModels
{
    public class UninstallConfirmViewModel : PageViewModelBase
    {
        public UninstallConfirmViewModel(InstallerApp installer)
        {
            this.installer = installer;

            _backButton = new ButtonData("Cancel", true);
            _nextButton = new ButtonData("Uninstall", true);

            _backButton.OnClick += () =>
            {
                Environment.Exit(0);
            };

            _nextButton.OnClick += () =>
            {
                OnStartUninstall?.Invoke();
                installer.Uninstall();
                OnFinishUninstall?.Invoke();
            };
        }

        InstallerApp installer;

        ButtonData _backButton;
        public override ButtonData BackButton => _backButton;

        ButtonData _nextButton;
        public override ButtonData NextButton => _nextButton;

        public event Action OnStartUninstall;
        public event Action OnFinishUninstall;
    }
}
