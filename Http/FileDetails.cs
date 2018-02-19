namespace Starship.Core.Http {

    public class FileDetails {

        public byte[] Read() {
            return System.IO.File.ReadAllBytes(Location);
        }

        public string Name { get; set; }

        public long Size { get; set; }

        public string ContentType { get; set; }

        public string Location { get; set; }
    }
}