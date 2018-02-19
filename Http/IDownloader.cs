using System.Net.Http;

namespace Starship.Core.Http {
    public interface IDownloader {
        FileDetails Download(HttpResponseMessage response, string url, string rootPath);
    }
}
