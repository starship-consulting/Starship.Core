using System;
using System.Collections.Generic;
using System.IO;

namespace Starship.Core.Utility {
    public static class FileHelper {
        public static List<string> GetFilesRecursively(string directory, string extension = "") {
            var results = new List<string>();

            foreach (var file in Directory.GetFiles(directory)) {
                if (string.IsNullOrEmpty(extension) || Path.GetExtension(file) == extension) {
                    results.Add(file.Replace(@"\", "/"));
                }
            }

            foreach (var file in Directory.GetDirectories(directory)) {
                results.AddRange(GetFilesRecursively(file, extension));
            }

            return results;
        }
    }
}