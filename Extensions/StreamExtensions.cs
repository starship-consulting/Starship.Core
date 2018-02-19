using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Starship.Core.Extensions {
    public static class StreamExtensions {

        public static async Task<byte[]> ToBytesAsync(this Stream input) {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream()) {
                int read;
                while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static void ToFile(this Stream stream, String fileLocation) {
            using (var fileStream = new FileStream(fileLocation, FileMode.OpenOrCreate, FileAccess.Write)) {
                using (var writer = new BinaryWriter(fileStream)) {
                    foreach (int chunk in Chunks(stream.Length)) {
                        byte[] buffer = new byte[chunk];

                        stream.Read(buffer, 0, chunk);
                        writer.Write(buffer);
                    }
                }
            }
        }

        public static String ConvertToString(this MemoryStream stream) {
            return Encoding.Default.GetString((stream.ToArray()));
        }

        public static String GetString(this MemoryStream stream) {
            return Encoding.Default.GetString((stream.ToArray()));
        }

        public static long BoundedCopyTo(this Stream source, Stream destination, long length) {
            var buffer = new byte[1024];
            long totalBytesRead = 0;
            int currentBytesRead;
            while (true) {
                var bytesToRead = 1024;
                if (totalBytesRead + bytesToRead > length)
                    bytesToRead = (int)(length - totalBytesRead);

                currentBytesRead = source.Read(buffer, 0, bytesToRead);
                if (currentBytesRead <= 0)
                    break;

                totalBytesRead += currentBytesRead;
                destination.Write(buffer, 0, currentBytesRead);

                if (totalBytesRead == length)
                    break;
            }

            return totalBytesRead;
        }

        public static void Write(this Stream source, byte[] buffer) {
            source.Write(buffer, 0, buffer.Length);
        }

        public static void Write(this Stream source, string s, Encoding encoding) {
            source.Write(encoding.GetBytes(s));
        }

        private static IEnumerable Chunks(long length) {
            if (length > Chunk) {
                yield return Chunk;

                foreach (int i in Chunks(length - Chunk))
                    yield return i;
            }
            else {
                yield return (int)length;
            }
        }

        private const int Chunk = 4096;
    }
}
