using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SLTM.Installer
{
    public static partial class HttpClientExtensions
    {
        public static async Task DownloadAsync(this HttpClient client, string requestUrl, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                var contentLength = response.Content.Headers.ContentLength;
                using (var download = await response.Content.ReadAsStreamAsync())
                {
                    // no progress... no contentLength... very sad
                    if (progress is null || !contentLength.HasValue)
                    {
                        await download.CopyToAsync(destination);
                        return;
                    }
                    // Such progress and contentLength much reporting Wow!
                    var progressWrapper = new Progress<long>(totalBytes => progress.Report(GetProgressPercentage(totalBytes, contentLength.Value)));
                    await download.CopyToAsync(destination, 81920, progressWrapper, cancellationToken);
                }
            }

            float GetProgressPercentage(float totalBytes, float currentBytes) => (totalBytes / currentBytes) * 100f;
        }
    }
}