using ReactiveUI;
using SLTM.Installer.Services;

namespace SLTM.Installer.ViewModels
{
    public class PickLocationPageViewModel : PageViewModelBase
    {
        public PickLocationPageViewModel(InstallerApp installer)
        {
            _installer = installer;
            _installLocation = _installer.Updater.OutputPath;
        }

        InstallerApp _installer;

        public override ButtonData NextButton => new ButtonData("Instal");

        string _installLocation;
        public string InstalLocation
        {
            get => _installLocation;
            set
            {
                this.RaiseAndSetIfChanged(ref _installLocation, value);
                _installer.Updater.OutputPath = _installLocation;
            }
        }
    }
}
