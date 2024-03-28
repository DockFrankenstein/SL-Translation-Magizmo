using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Data;
using UnityEditor.PackageManager;

namespace Project.AutoUpdate
{
    [Serializable]
    public class AutoUpdater
    {
        const string DOWNLOAD_URL = "https://github.com/DockFrankenstein/SL-Translation-Magizmo/releases/download/{0}/{1}";
        const string RELEASES_URL = "https://api.github.com/repos/DockFrankenstein/SL-Translation-Magizmo/releases";

        public enum Status
        {
            NotPrepared,
            CheckingForUpdates,
            CheckingForUpdatesError,
            UpdateAvaliable,
            DownloadingUpdate,
            DownloadingUpdateError,
            ReadyToFinalizeUpdate,
            UpToDate,
        }

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
                {
                    error = $"Couldn't get web request from {RELEASES_URL}.";
                    UpdaterStatus = Status.CheckingForUpdatesError;
                    yield break;
                }

                var txt = $"{{\"items\":{request.downloadHandler.text}}}";
                var json = JsonUtility.FromJson<Response>(txt);

                if (json.items.Length == 0)
                {
                    error = "Web request contained no items.";
                    UpdaterStatus = Status.CheckingForUpdatesError;
                    yield break;
                }

                NewVersion = json.items.First().tag_name;
            }
        }

        public IEnumerator CheckForUpdates()
        {
            UpdaterStatus = Status.CheckingForUpdates;
            yield return GetVersion();

            if (UpdaterStatus == Status.CheckingForUpdatesError)
                yield break;

            UpdaterStatus = NewVersion == CurrentVersion ?
                Status.UpToDate :
                Status.UpdateAvaliable;
        }

        public Status UpdaterStatus { get; set; } = Status.NotPrepared;
        public string error;

        public IEnumerator DownloadUpdate()
        {
            if (UpdaterStatus == Status.DownloadingUpdate)
                yield break;

            if (NewVersion == null)
                CheckForUpdates();

            var url = string.Format(DOWNLOAD_URL, NewVersion, TargetFileName);

            UpdaterStatus = Status.DownloadingUpdate;

            using (var request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    error = $"Couldn't get web request from {url}.";
                    UpdaterStatus = Status.DownloadingUpdateError;
                    yield break;
                }

                try
                {
                    System.IO.File.WriteAllBytes(ResultPath, request.downloadHandler.data);
                }
                catch (Exception e)
                {
                    error = $"Error while saving new update to disk, {e}";
                    UpdaterStatus = Status.DownloadingUpdateError;
                    yield break;
                }
            }

            UpdaterStatus = Status.ReadyToFinalizeUpdate;
        }

        public void ClearError()
        {
            error = string.Empty;
            UpdaterStatus = UpdaterStatus switch
            {
                Status.CheckingForUpdatesError => Status.NotPrepared,
                Status.DownloadingUpdateError => Status.UpdateAvaliable,
                _ => UpdaterStatus
            };
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