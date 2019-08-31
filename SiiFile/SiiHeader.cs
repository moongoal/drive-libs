using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.SiiFile {
    /// <summary>
    /// Binary Sii file header.
    /// </summary>
    public class SiiHeader {
        /// <summary>
        /// The signature of the header.
        /// </summary>
        public UInt32 Signature { get; private set; }

        /// <summary>
        /// The HMAC of the file.
        /// </summary>
        public byte[] Hmac { get; private set; }

        /// <summary>
        /// The encryption IV.
        /// </summary>
        public byte[] InitializationVector { get; private set; }

        /// <summary>
        /// The decompressed size of the unit file.
        /// </summary>
        public UInt32 DecompressedUnitSize { get; private set; }

        private SiiHeader () { }

        /// <summary>
        /// Read the header from a stream. This method advances the stream by
        /// the length of the header but doesn't close the stream.
        /// </summary>
        /// <param name="stream">The stream to read the header from</param>
        /// <returns>Sii file header information</returns>
        public static SiiHeader FromStream (Stream stream) {
            SiiHeader header = new SiiHeader();

            using (BinaryReader br = new BinaryReader(stream, Encoding.UTF8, true)) {
                header.Signature = br.ReadUInt32();
                header.Hmac = br.ReadBytes(32);
                header.InitializationVector = br.ReadBytes(16);
                header.DecompressedUnitSize = br.ReadUInt32();
            }

            return header;
        }
    }
}
