using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using File = System.IO.File;

namespace SLTM.Installer.Services
{
    public class InstallerApp
    {
        const string APP_REGISTRY_KEY = "SL Translation Magizmo";
        public const string APP_FILE_NAME = "SL Translation Magizmo.exe";

        public const string ARGS_UPDATE = "--update";
        public const string ARGS_DEBUG_UNINSTALL = "--debug-uninstall";

        public enum Mode
        {
            Install,
            Uninstall,
            Update,
        }

        public InstallerApp()
        {
            Updater = new AutoUpdater()
            {
                OutputPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}/SL Translation Magizmo",
                TargetFileName = "Windows.zip",
            };

            Arguments = new HashSet<string>(Environment.GetCommandLineArgs().Distinct());
            DetermineMode();

            if (AppMode == Mode.Update)
            {
                Updater.OutputPath = Path.GetDirectoryName(Environment.ProcessPath);
                _ = Update();
            }
        }

        void DetermineMode()
        {
            AppMode = Mode.Install;

            var appPath = Path.GetDirectoryName(Environment.ProcessPath);

            if (File.Exists($"{appPath}/{APP_FILE_NAME}") || Arguments.Contains(ARGS_DEBUG_UNINSTALL))
            {
                AppMode = Arguments.Contains(ARGS_UPDATE) ?
                    Mode.Update :
                    Mode.Uninstall;
            }
        }

        public Mode AppMode { get; private set; }

        public AutoUpdater Updater { get; set; }

        public bool CreateDesktopShortcut { get; set; }

        public HashSet<string> Arguments { get; }

        public string RootPath => Updater.OutputPath;
        public string ExePath => $"{RootPath}/SL Translation Magizmo.exe";

        public Action OnProcessBegin;
        public Action OnProcessFinish;
        public Action<Exception> OnException;

        public void Uninstall()
        {
            try
            {
                OnProcessBegin?.Invoke();

                if (Arguments.Contains(ARGS_DEBUG_UNINSTALL))
                {
                    OnProcessFinish?.Invoke();
                    return;
                }

                DeregisterAppInSystem();

                var rootPath = Path.GetDirectoryName(Environment.ProcessPath);

                foreach (var item in Directory.GetFiles(rootPath))
                    if (item != Environment.ProcessPath)
                        File.Delete(item);

                foreach (var item in Directory.GetDirectories(rootPath))
                    Directory.Delete(item, true);

                AppDomain.CurrentDomain.ProcessExit += (_, _) => ExitAndFinalizeUninstall();
            }
            catch (Exception e)
            {
                OnException?.Invoke(e);
            }

            OnProcessFinish?.Invoke();
        }

        public void ExitAndFinalizeUninstall()
        {
            if (Arguments.Contains(ARGS_DEBUG_UNINSTALL))
                return;

            Process.Start(new ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Environment.ProcessPath + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
        }

        public async Task Install()
        {
            try
            {
                OnProcessBegin?.Invoke();

                await Updater.DownloadUpdate();

                var exePath = ExePath
                    .Replace('\\', '/');

                RegisterAppInSystem();

                if (CreateDesktopShortcut)
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
                OnException?.Invoke(e);
            }

            OnProcessFinish?.Invoke();
        }

        public async Task Update()
        {
            try
            {
                OnProcessBegin?.Invoke();
                await Updater.DownloadUpdate();
            }
            catch (Exception e)
            {
                OnException?.Invoke(e);
            }

            OnProcessFinish?.Invoke();
        }

        public void RegisterAppInSystem()
        {
            if (OperatingSystem.IsWindows())
            {
                var newappkey = GetWindowsUninstallRegistryKey()
                    .CreateSubKey(APP_REGISTRY_KEY);

                newappkey.SetValue("DisplayIcon", ExePath);
                newappkey.SetValue("DisplayName", "SL Translation Magizmo");
                newappkey.SetValue("DisplayVersion", Updater.NewVersion);
                newappkey.SetValue("Publisher", "Dock Frankenstein");
                newappkey.SetValue("UninstallString", $"{RootPath}/Uninstall.exe");
            }
        }

        public void DeregisterAppInSystem()
        {
            if (OperatingSystem.IsWindows())
            {
                var key = GetWindowsUninstallRegistryKey();
                key.DeleteSubKeyTree(APP_REGISTRY_KEY, false);
            }
        }

        [SupportedOSPlatform("windows")]
        RegistryKey GetWindowsUninstallRegistryKey() =>
            Registry.LocalMachine.OpenSubKey("SOFTWARE")
                    .OpenSubKey("Microsoft")
                    .OpenSubKey("Windows")
                    .OpenSubKey("CurrentVersion")
                    .OpenSubKey("Uninstall", true);
    }
}