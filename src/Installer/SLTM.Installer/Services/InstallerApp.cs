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

        public const string ARGS_UPDATE = "update";
        public const string ARGS_DEBUG_UNINSTALL = "debug-uninstall";
        public const string ARGS_DELETE_AFTER = "delete-after";

        public const string START_MENU_PATH = "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs";
        public const string START_MENU_SHORTCUT_PATH = START_MENU_PATH + "\\SL Translation Magizmo.lnk";

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
                OutputPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\\SL Translation Magizmo",
                TargetFileName = "Windows.zip",
            };

            AppDomain.CurrentDomain.ProcessExit += (_, _) => OnExit();

            Arguments = new HashSet<Argument>();
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-"))
                {
                    Arguments.Add(new Argument() { value = args[i], });
                    continue;
                }

                var arg = new Argument()
                {
                    argument = args[i],
                    value = null,
                };

                while (arg.argument.StartsWith("-"))
                    arg.argument = arg.argument.Substring(1, arg.argument.Length - 1);

                if (i + 1 < args.Length)
                {
                    i++;
                    arg.value = args[i];
                }

                Arguments.Add(arg);
            }

            DeleteAfter = Arguments.Any(x => x.argument == ARGS_DELETE_AFTER);

            DetermineMode();

            if (AppMode == Mode.Update)
            {
                _ = Update();
            }
        }

        void DetermineMode()
        {
            AppMode = Mode.Install;

            var appPath = Path.GetDirectoryName(Environment.ProcessPath);

            if (Arguments.Any(x => x.argument == ARGS_UPDATE))
            {
                AppMode = Mode.Update;
                Updater.OutputPath = Arguments.Where(x => x.argument == ARGS_UPDATE).First().value
                    ?? Path.GetDirectoryName(Environment.ProcessPath);
                return;
            }

            if (File.Exists($"{appPath}/{APP_FILE_NAME}") || Arguments.Any(x => x.argument == ARGS_DEBUG_UNINSTALL))
            {
                AppMode = Mode.Uninstall;
            }
        }

        public Mode AppMode { get; private set; }

        public AutoUpdater Updater { get; set; }

        public bool CreateDesktopShortcut { get; set; }

        public HashSet<Argument> Arguments { get; }

        public string RootPath => Updater.OutputPath;
        public string ExePath => $"{RootPath}/SL Translation Magizmo.exe";

        public Action OnProcessBegin;
        public Action OnProcessFinish;
        public Action<Exception> OnException;

        public bool DeleteAfter { get; private set; }

        public void Uninstall()
        {
            try
            {
                OnProcessBegin?.Invoke();

                if (Arguments.Any(x => x.argument == ARGS_DEBUG_UNINSTALL))
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

                DeleteAfter = true;
            }
            catch (Exception e)
            {
                OnException?.Invoke(e);
            }

            OnProcessFinish?.Invoke();
        }

        public void OnExit()
        {
            if (Arguments.Any(x => x.argument == ARGS_DEBUG_UNINSTALL))
                return;

            if (!DeleteAfter) return;

            Process.Start(new ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Environment.ProcessPath + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
        }

        public void LaunchApp()
        {
            try
            {
                Process.Start(ExePath);
            }
            catch { }
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
                    CreateShortcut($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/SL Translation Magizmo.lnk");
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

                CreateShortcut(START_MENU_SHORTCUT_PATH);
            }
        }

        public void DeregisterAppInSystem()
        {
            if (OperatingSystem.IsWindows())
            {
                var key = GetWindowsUninstallRegistryKey();
                key.DeleteSubKeyTree(APP_REGISTRY_KEY, false);
                if (File.Exists(START_MENU_SHORTCUT_PATH))
                    File.Delete(START_MENU_SHORTCUT_PATH);
            }
        }

        void CreateShortcut(string shortcutPath)
        {
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            shortcut.TargetPath = ExePath
                .Replace('\\', '/');
            shortcut.Save();
        }

        [SupportedOSPlatform("windows")]
        RegistryKey GetWindowsUninstallRegistryKey() =>
            Registry.LocalMachine.OpenSubKey("SOFTWARE")
                    .OpenSubKey("Microsoft")
                    .OpenSubKey("Windows")
                    .OpenSubKey("CurrentVersion")
                    .OpenSubKey("Uninstall", true);

        public struct Argument
        {
            public string argument;
            public string value;
        }
    }
}