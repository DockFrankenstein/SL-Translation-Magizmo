﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SLTM.Installer.Services
{
    public class AutoUpdater
    {
        const string DOWNLOAD_URL = "https://github.com/DockFrankenstein/SL-Translation-Magizmo/releases/download/{0}/{1}";
        const string RELEASES_URL = "https://api.github.com/repos/DockFrankenstein/SL-Translation-Magizmo/releases";

        public const string ARGS_RELEASE = "release";
        public const string ARGS_BETA = "beta";
        

        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; } = null;
        public string TargetFileName { get; set; }
        public string OutputPath { get; set; }

        public enum Channel
        {
            Release,
            Beta,
            Any,
        }

        static Channel? _updateChannel = null;
        public static Channel UpdateChannel
        {
            get
            {
                if (_updateChannel == null)
                {
                    var args = Environment.GetCommandLineArgs();
                    _updateChannel = Channel.Any;

                    if (args.Any(x => x == ARGS_RELEASE))
                        _updateChannel = Channel.Release;

                    if (args.Any(x => x == ARGS_BETA))
                        _updateChannel = Channel.Beta;
                }

                return _updateChannel ?? Channel.Any;
            }
        }

        public async Task GetVersion()
        {
            NewVersion ??= CurrentVersion;

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestHeaders.Add("User-Agent", "request");

                var response = await client.GetAsync(RELEASES_URL);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Web request unsuccessfull.");

                var txt = await response.Content.ReadAsStringAsync();
                txt = $"{{\"items\":{txt}}}";

                var json = JsonConvert.DeserializeObject<Response>(txt);

                foreach (var item in json.items)
                {
                    if (UpdateChannel == Channel.Release &&
                        item.prerelease)
                        continue;

                    NewVersion = item.tag_name;
                    return;
                }

                throw new Exception($"Couldn't find any releases in update channel '{UpdateChannel}'.");
            }
        }

        public async Task CheckForUpdates()
        {
            await GetVersion();
            UpdateAvaliable = NewVersion != CurrentVersion;
        }

        public bool UpdateAvaliable { get; private set; } = false;

        public Progress<float> Progress { get; private set; } = new Progress<float>();

        public async Task DownloadUpdate()
        {
            if (NewVersion == null)
                await CheckForUpdates();

            var tempPath = Path.GetTempFileName();

            var url = string.Format(DOWNLOAD_URL, NewVersion, TargetFileName);

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.DefaultRequestHeaders.Add("User-Agent", "request");

                using (var file = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await client.DownloadAsync(url, file, Progress);
                }

                var folderPath = $"{OutputPath}/SL Translation Magizmo";

                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);

                using (var zip = ZipFile.OpenRead(tempPath))
                {
                    foreach (var item in zip.Entries)
                    {
                        const string FolderPath = "SL Translation Magizmo/";

                        var zipPath = item.FullName.Replace('\\', '/');

                        if (zipPath.StartsWith(FolderPath))
                            zipPath = zipPath.Substring(FolderPath.Length, zipPath.Length - FolderPath.Length);

                        var path = $"{OutputPath}/{zipPath}";

                        if (Path.GetFullPath(path) == Path.GetFullPath(Environment.ProcessPath))
                            continue;

                        if (!string.IsNullOrWhiteSpace(zipPath) && 
                            !Path.EndsInDirectorySeparator(zipPath))
                        {
                            var dirPath = Path.GetDirectoryName(path);

                            if (!Directory.Exists(dirPath))
                                Directory.CreateDirectory(dirPath);

                            item.ExtractToFile(path, true);
                        }
                    }
                }

                File.Delete(tempPath);
            }
        }

        [Serializable]
        class Response
        {
            public Item[] items;

            [Serializable]
            public class Item
            {
                public string tag_name;
                public bool prerelease;
            }
        }
    }
}
