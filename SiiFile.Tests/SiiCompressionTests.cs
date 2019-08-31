using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class SiiCompressionTests {
        [TestMethod]
        public async Task TestDecompress () {
            byte[] expectedOutput = Resources.Samples.GameText;
            byte[] inputContent = Resources.Samples.GameCompressed;
            MemoryStream inputStream = new MemoryStream(inputContent);
            MemoryStream outputStream = new MemoryStream();

            await SiiCompression.Decompress(inputStream, outputStream);

            outputStream.Position = 0;
            byte[] output = new byte[outputStream.Length];
            outputStream.Read(output, 0, output.Length);

            CollectionAssert.AreEqual(output, expectedOutput);
        }

        [TestMethod]
        public async Task TestCompress () {
            /*
             * Since compressed size is not set in stone, we do a roundtrip to test compression
             * and assume Decompress() being reliable because it's tested on external data.
             */
            byte[] inputContent = Resources.Samples.GameText;
            byte[] expectedOutput = inputContent;
            MemoryStream inputStream = new MemoryStream(inputContent);
            MemoryStream outputStream = new MemoryStream();
            MemoryStream decompressedStream = new MemoryStream();

            await SiiCompression.Compress(inputStream, outputStream);

            outputStream.Position = 0;
            await SiiCompression.Decompress(outputStream, decompressedStream);

            decompressedStream.Position = 0;
            byte[] output = new byte[decompressedStream.Length];
            decompressedStream.Read(output, 0, output.Length);

            CollectionAssert.AreEqual(output, expectedOutput);
        }
    }
}
