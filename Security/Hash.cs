using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Starship.Core.Extensions;

namespace Starship.Core.Security {
    /// <summary>
    /// (From NEN)  Provides commonly used utilities for computing cryptographic hashes.
    /// </summary>
    public static class Hash {

        /// <summary>
        /// Hashes the contents of a string using the crypto algorithm returned by <paramref name="algorithm"/>.
        /// Returns a hex-encoded string containing the hash value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="algorithm">A function returning a HashAlgorithm.</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashStream<T>(Func<T> algorithm, Stream input) where T : HashAlgorithm {
            using (var hash = algorithm()) {
                using (var cryptoStream = new CryptoStream(Stream.Null, hash, CryptoStreamMode.Write)) {
                    input.BoundedCopyTo(cryptoStream, long.MaxValue);
                    cryptoStream.FlushFinalBlock();
                    return string.Join("", hash.Hash.Select(b => b.ToString("X2")).ToArray());
                }
            }
        }

        /// <summary>
        /// Returns the SHA1 hash of the given string, encoded using the given encoding.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="encoding">The encoding used to produce a byte stream for hashing.</param>
        /// <returns>A string containing a textual representation of the SHA1 hash.</returns>
        public static string Sha1(string input, Encoding encoding) {
            var ms = new MemoryStream(encoding.GetBytes(input));
            return Sha1(ms);
        }

        /// <summary>
        /// Returns the SHA256 hash of the given string, encoded using the given encoding.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="encoding">The encoding used to produce a byte stream for hashing.</param>
        /// <returns>A string containing a textual representation of the SHA256 hash.</returns>
        public static string Sha256(string input, Encoding encoding) {
            var ms = new MemoryStream(encoding.GetBytes(input));
            return Sha256(ms);
        }

        /// <summary>
        /// Returns the SHA1 hash of the contents of a stream.
        /// </summary>
        /// <param name="input">The stream to hash.</param>
        /// <returns>A string containing a textual representation of the SHA1 hash.</returns>
        public static string Sha1(Stream input) {
            return HashStream(SHA1.Create, input);
        }

        /// <summary>
        /// Returns the SHA256 hash of the contents of a stream.
        /// </summary>
        /// <param name="input">The stream to hash.</param>
        /// <returns>A string containing a textual representation of the SHA256 hash.</returns>
        public static string Sha256(Stream input) {
            return HashStream(SHA256.Create, input);
        }

        /// <summary>
        /// Returns the SHA1 hash of the file given by path.
        /// </summary>
        /// <param name="path">The path of the file to hash.</param>
        /// <returns>A string containing a textual representation of the SHA1 hash.</returns>
        public static string Sha1File(string path) {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                return Sha1(fs);
            }
        }

        /// <summary>
        /// Returns the HMAC-SHA1 message digest if the stream given by input,
        /// using the given secret key.
        /// </summary>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="input">The input stream for which the digest will be computed.</param>
        /// <returns>A hex string containing the HMAC-SHA1 digest value.</returns>
        public static string HmacSha1(byte[] secretKey, Stream input) {
            return HashStream(() => new HMACSHA1 {Key = secretKey}, input);
        }

        public static string HmacSha1(string secretKey, string message, Encoding encoding) {
            return HmacSha1(encoding.GetBytes(secretKey), new MemoryStream(encoding.GetBytes(message)));
        }

        public static string HmacSha1(string secretKey, string message) {
            return HmacSha1(secretKey, message, Encoding.UTF8);
        }

        /// <summary>
        /// Returns the HMAC-SHA256 message digest if the stream given by input,
        /// using the given secret key.
        /// </summary>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="input">The input stream for which the digest will be computed.</param>
        /// <returns>A hex string containing the HMAC-SHA256 digest value.</returns>
        public static string HmacSha256(byte[] secretKey, Stream input) {
            return HashStream(() => new HMACSHA256 {Key = secretKey}, input);
        }

        public static string HmacSha256(string secretKey, string message, Encoding encoding) {
            return HmacSha256(encoding.GetBytes(secretKey), new MemoryStream(encoding.GetBytes(message)));
        }

        public static string HmacSha256(string secretKey, string message) {
            return HmacSha256(secretKey, message, Encoding.UTF8);
        }

        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static string EncryptStringAES(string plainText, string sharedSecret, string salt) {

            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null; // Encrypted string to return
            RijndaelManaged aesAlg = null; // RijndaelManaged object used to encrypt the data.

            try {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, salt.ToBytes());

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream()) {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }

                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string DecryptStringAES(string cipherText, string sharedSecret, string salt) {

            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, salt.ToBytes());

                // Create the streams used for decryption
                byte[] bytes = Convert.FromBase64String(cipherText);

                using (MemoryStream msDecrypt = new MemoryStream(bytes)) {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally {
                aesAlg?.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s) {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length) {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length) {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}