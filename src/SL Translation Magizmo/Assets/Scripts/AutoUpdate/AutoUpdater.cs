using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Project.AutoUpdate
{
    [Serializable]
    public class AutoUpdater
    {
        const string DOWNLOAD_URL = "https://github.com/DockFrankenstein/SL-Translation-Magizmo/releases/download/{0}/{1}";
        const string RELEASES_URL = "https://api.github.com/repos/DockFrankenstein/SL-Translation-Magizmo/releases";

        [field: SerializeField] public string CurrentVersion { get; set; }
        [field: SerializeField] public string NewVersion { get; set; } = null;
        [field: SerializeField] public string TargetFileName { get; set; }
        [field: SerializeField] public string ResultPath { get; set; }

        public IEnumerator GetVersion()
        {
            NewVersion ??= CurrentVersion;

            using (var request = UnityWebRequest.Get(RELEASES_URL))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                    yield break;

                var txt = $"{{\"items\":{request.downloadHandler.text}}}";
                var json = JsonUtility.FromJson<Response>(txt);

                if (json.items.Length == 0) yield break;

                NewVersion = json.items.First().tag_name;
            }
        }

        public void CheckForUpdates()
        {
        }

        public bool IsDownloading { get; set; }

        public IEnumerator DownloadUpdate()
        {
            if (IsDownloading)
                yield break;

            if (NewVersion == null)
                CheckForUpdates();

            var url = string.Format(DOWNLOAD_URL, NewVersion, TargetFileName);

            IsDownloading = true;

            using (var request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    IsDownloading = false;
                    yield break;
                }

                try
                {
                    System.IO.File.WriteAllBytes(ResultPath, request.downloadHandler.data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while saving new update to disk, {e}");
                }
            }

            IsDownloading = false;
        }

        private IEnumerator DownloadUpdateCoroutine(string url, string filePath)
        {
            IsDownloading = true;

            using (var request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    IsDownloading = false;
                    yield break;
                }

                System.IO.File.WriteAllBytes(filePath, request.downloadHandler.data);
            }

            IsDownloading = false;
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