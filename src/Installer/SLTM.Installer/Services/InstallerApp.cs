using System;

namespace SLTM.Installer.Services
{
    public class InstallerApp
    {
        public InstallerApp()
        {
            Updater = new AutoUpdater()
            {
                OutputPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                TargetFileName = "Windows.zip",
            };
        }

        public AutoUpdater Updater { get; set; }

        public bool CreateDesktopShortcut { get; set; }
    }
}