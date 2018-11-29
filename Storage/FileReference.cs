using System;
using System.IO;
using System.Threading.Tasks;

namespace Starship.Core.Storage {
    public abstract class FileReference {

        public abstract Task<bool> DeleteAsync();

        public bool Exists { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public string Path { get; set; }

        public string Url { get; set; }

        public string Keywords { get; set; }

        public DateTime LastModified { get; set; }

        public Stream Stream { get; set; }
    }
}