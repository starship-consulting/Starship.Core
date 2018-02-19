using System;

namespace Starship.Core.Events.Standard {
    public class FileModified {

        public FileModified() {
        }

        public FileModified(string name, byte[] data) {
            Name = name;
            Data = data;
        }

        public string Name { get; set; }

        public byte[] Data { get; set; }
    }
}