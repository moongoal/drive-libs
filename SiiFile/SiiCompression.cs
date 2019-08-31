using System;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Drive.SiiFile {
    /// <summary>
    /// Helper class to handle Sii unit compression.
    /// </summary>
    public static class SiiCompression {
        /// <summary>
        /// Decompress the content of Sii unit data.
        /// </summary>
        /// <param name="inputStream">Compressed information</param>
        /// <param name="outputStream">Decompressed output stream</param>
        public static async Task Decompress (Stream inputStream, Stream outputStream) {
            using (GZipStream gz = new GZipStream(inputStream, CompressionMode.Decompress)) {
                await gz.CopyToAsync(outputStream);
            }
        }

        /// <summary>
        /// Compress the content of Sii unit data.
        /// </summary>
        /// <param name="inputStream">Decompressed information</param>
        /// <param name="outputStream">Compressed output stream</param>
        public static async Task Compress (Stream inputStream, Stream outputStream) {
            using (GZipStream gz = new GZipStream(outputStream, CompressionMode.Compress, true)) {
                await inputStream.CopyToAsync(gz);
            }
        }
    }
}
