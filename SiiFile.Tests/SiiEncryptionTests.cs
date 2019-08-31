using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class SiiEncryptionTests {
        const int headerLength = 56;

        [TestMethod]
        public async Task TestDecrypt () {
            byte[] expectedOutput = Resources.Samples.GameCompressed;
            byte[] inputContent = Resources.Samples.GameEncrypted;
            byte[] iv = Resources.Samples.GameIV;
            
            Stream inputStream = new MemoryStream(inputContent, headerLength, inputContent.Length - headerLength);
            MemoryStream outputStream = new MemoryStream();

            await SiiEncryption.Decrypt(inputStream, outputStream, iv);
            byte[] output = new byte[outputStream.Length];
            outputStream.Position = 0;
            outputStream.Read(output, 0, output.Length);

            CollectionAssert.AreEqual(output, expectedOutput);
        }

        [TestMethod]
        public async Task TestEncrypt () {
            byte[] inputContent = Resources.Samples.GameCompressed;
            byte[] expectedOutput = new byte[Resources.Samples.GameEncrypted.Length - headerLength];
            byte[] iv = Resources.Samples.GameIV;
            MemoryStream outputStream = new MemoryStream();
            MemoryStream inputStream = new MemoryStream(inputContent);

            Array.Copy(Resources.Samples.GameEncrypted, headerLength, expectedOutput, 0, expectedOutput.Length);
            await SiiEncryption.Encrypt(inputStream, outputStream, iv);

            outputStream.Position = 0;
            byte[] output = new byte[outputStream.Length];
            outputStream.Read(output, 0, output.Length);

            CollectionAssert.AreEqual(output, expectedOutput);
        }
    }
}
