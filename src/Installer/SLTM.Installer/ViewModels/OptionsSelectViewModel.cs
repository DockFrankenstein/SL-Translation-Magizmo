using ReactiveUI;
using SLTM.Installer.Services;

namespace SLTM.Installer.ViewModels
{
    public class OptionsSelectViewModel : PageViewModelBase
    {
        public OptionsSelectViewModel(InstallerApp installer)
        {
            this.installer = installer;

            _nextButton = new ButtonData("Install");

            _nextButton.OnClick += () =>
            {
                _ = installer.Install();
            };
        }

        public InstallerApp installer;

        ButtonData _nextButton;
        public override ButtonData NextButton => _nextButton;

        public override void OnOpenPage()
        {
            installer.CreateDesktopShortcut = CreateDesktopShortcut;
        }

        bool _createDesktopShortcut = true;
        public bool CreateDesktopShortcut
        {
            get => _createDesktopShortcut;
            set
            {
                this.RaiseAndSetIfChanged(ref _createDesktopShortcut, value);
                installer.CreateDesktopShortcut = value;
            }
        }
    }
}