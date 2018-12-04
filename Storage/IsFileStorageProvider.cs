using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Starship.Core.Storage {
    public interface IsFileStorageProvider {

        void Rename(string partition, string path, string name);

        void CreateDirectories(string partition, string path);

        Task<FileReference> UploadAsync(string partition, Stream stream, string path);

        Task<FileReference> UploadAsync(string partition, byte[] data, string path);

        Task<IEnumerable<FileReference>> ListFilesAsync(string partition, string path);

        Task<bool> DeleteAsync(string partition, string path);

        Task<FileReference> GetFileAsync(string partition, string path);
    }
}