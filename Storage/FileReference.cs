using System;
using System.IO;
using Newtonsoft.Json;

namespace Starship.Core.Storage {
    public class FileReference {

        public FileReference() {
        }
        
        public FileReference(string path, bool isFolder = false) {
            Path = path;
            IsFolder = isFolder;
        }
        
        public long Length { get; set; }
        
        public string ContentType { get; set; }
        
        public string Path { get; set; }

        public string Keywords { get; set; }

        public DateTime LastModified { get; set; }
        
        public bool IsFolder { get; set; }

        [JsonIgnore]
        public Stream Stream { get; set; }
    }
}