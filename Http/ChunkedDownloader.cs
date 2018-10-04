using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Starship.Core.Http {
    public class ChunkedDownloader : IDownloader {
        
        public ChunkedDownloader() {
            MaxChunks = 5;
        }

        public FileDetails Download(HttpResponseMessage response, string url, string path) {
            var parallelDownloadSuported = response.Headers.AcceptRanges.Contains("bytes");
            var contentLength = response.Content.Headers.ContentLength ?? 0;

            var tasks = new List<Task>();
            var partSize = (long)System.Math.Ceiling(contentLength / (double)MaxChunks);

            File.Create(path).Dispose();

            for (var i = 0; i < MaxChunks; i++) {
                var start = i * partSize + System.Math.Min(1, i);
                var end = System.Math.Min((i + 1) * partSize, contentLength);

                tasks.Add(Task.Run(() => DownloadPart(url, path, start, end)));
            }
            
            Task.WhenAll(tasks).Wait();

            //var content = File.ReadAllBytes(path);
            //File.Delete(path);

            var info = new FileInfo(path);

            return new FileDetails {
                Name = path,
                Size = info.Length,
                Location = path
            };
        }

        private async Task DownloadPart(string url, string saveAs, long start, long end) {
            using (var httpClient = new HttpClient()) {

                httpClient.Timeout = TimeSpan.FromMinutes(10);

                using (var fileStream = new FileStream(saveAs, FileMode.Open, FileAccess.Write, FileShare.ReadWrite)) {
                    var message = new HttpRequestMessage(HttpMethod.Get, url);
                    message.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0");
                    message.Headers.Add("Range", string.Format("bytes={0}-{1}", start, end));

                    fileStream.Position = start;

                    try {
                        var response = await httpClient.SendAsync(message);
                        await response.Content.CopyToAsync(fileStream);
                    }
                    catch (AggregateException ex) {
                        throw ex.InnerException.InnerException;
                    }
                }
            }
        }

        public int MaxChunks { get; set; }
    }
}