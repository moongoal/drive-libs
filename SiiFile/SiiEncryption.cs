using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Drive.SiiFile {
    /// <summary>
    /// Helper class to handle Sii unit encryption.
    /// </summary>
    public static class SiiEncryption {
        /// <summary>
        /// Sii file encryption key
        /// </summary>
        public static readonly byte[] EncryptionKey = new byte[] {
            0x2a, 0x5f, 0xcb, 0x17, 0x91, 0xd2, 0x2f, 0xb6,
            0x02, 0x45, 0xb3, 0xd8, 0x36, 0x9e, 0xd0, 0xb2,
            0xc2, 0x73, 0x71, 0x56, 0x3f, 0xbf, 0x1f, 0x3c,
            0x9e, 0xdf, 0x6b, 0x11, 0x82, 0x5a, 0x5d, 0x0a
        };

        /// <summary>
        /// Decrypt Sii unit data
        /// </summary>
        /// <param name="inputStream">Stream containing the encrypted unit data</param>
        /// /// <param name="outputStream">Output decrypted stream</param>
        /// <param name="iv">Algorithm initialization vector</param>
        public static async Task Decrypt (Stream inputStream, Stream outputStream, byte[] iv) {
            using (Aes aes = Aes.Create()) {
                aes.IV = iv;
                aes.Key = EncryptionKey;
                aes.Mode = CipherMode.CBC;

                using (ICryptoTransform t = aes.CreateDecryptor()) {
                    using (CryptoStream crys = new CryptoStream(inputStream, t, CryptoStreamMode.Read)) {
                        await crys.CopyToAsync(outputStream);
                    }
                }
            }
        }

        /// <summary>
        /// Encrypt Sii unit data.
        /// </summary>
        /// <param name="inputStream">The input data stream to encrypt</param>
        /// <param name="outputStream">The destination data stream to write the encrypted data to</param>
        /// <param name="iv">The AES initialization vector or null to generate a new one automatically</param>
        /// <returns>The IV used for the encryption process</returns>
        public static async Task<byte[]> Encrypt (Stream inputStream, Stream outputStream, byte[] iv = null) {
            using (Aes aes = Aes.Create()) {
                aes.Key = EncryptionKey;
                aes.Mode = CipherMode.CBC;

                if (iv != null)
                    aes.IV = iv;
                else
                    aes.GenerateIV();

                using (ICryptoTransform t = aes.CreateEncryptor()) {
                    using (CryptoStream crys = new CryptoStream(inputStream, t, CryptoStreamMode.Read)) {
                        await crys.CopyToAsync(outputStream);
                    }
                }

                return aes.IV;
            }
        }
    }
}
