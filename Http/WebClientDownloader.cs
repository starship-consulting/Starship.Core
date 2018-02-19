using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Starship.Core.Http {
    public class WebClientDownloader : IDownloader {

        public FileDetails Download(HttpResponseMessage response, string url, string path) {
            using (var client = new WebClient()) {
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0");
                client.DownloadFile(url, path);

                var info = new FileInfo(path);

                return new FileDetails {
                    Name = path,
                    Size = info.Length,
                    Location = path
                };
            }
        }
    }
}