using System;
using System.IO;
using System.Text;

namespace Starship.Core.Extensions {
    public static class ByteExtensions {
        public static String ConvertToString(this byte[] bytes) {
            if (bytes == null) {
                return string.Empty;
            }

            using (var stream = new MemoryStream(bytes)) {
                using (var reader = new StreamReader(stream, Encoding.UTF8)) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
