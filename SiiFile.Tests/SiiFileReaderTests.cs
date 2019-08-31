using System;
using System.IO;
using System.Text;
using System.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class SiiFileReaderTests {
        private class TestInclusionProvider : IInclusionProvider {
            public delegate Stream GetResourceStreamHandler (string fileName);

            private GetResourceStreamHandler _getResStream;

            public Stream IncludeFile (string path) => _getResStream(Path.GetFileNameWithoutExtension(path));

            public TestInclusionProvider(GetResourceStreamHandler getResStream) {
                _getResStream = getResStream;
            }
        }

        private readonly ResourceManager _resManager = new ResourceManager(typeof(Resources.Samples));

        private Stream GetResourceStream (string fileName) {
            string s = (string)_resManager.GetObject(fileName);
            byte[] b = Encoding.UTF8.GetBytes(s);
            return new MemoryStream(b);
        }

        [TestMethod]
        public void TestInclusion () {
            IInclusionProvider provider = new TestInclusionProvider(GetResourceStream);
            SiiFileReader readerWithInclusionProvider = new SiiFileReader(provider);
            SiiFileReader readerWithoutInclusionProvider = new SiiFileReader();

            using (Stream includerStream = GetResourceStream("IncluderUnit")) {
                Assert.ThrowsExceptionAsync<InvalidOperationException>(async delegate {
                    await readerWithInclusionProvider.ReadFromText(includerStream);
                });
            }

            using (Stream includerStream = GetResourceStream("IncluderUnit")) {
                Object obj = readerWithInclusionProvider.ReadSiiFile(includerStream).Result;

                Assert.IsTrue(obj.Attributes.ContainsKey("attr1"));
                Assert.IsTrue(obj.Attributes.ContainsKey("attr2"));
                Assert.IsFalse(obj.Attributes.ContainsKey("attr_non_existing"));
            }
        }

        [TestMethod]
        public void TestSpecializedReadingMethods () {
            SiiFileReader reader = new SiiFileReader();
            Object fromBinary, fromText;

            using (MemoryStream s = new MemoryStream(Resources.Samples.GameEncrypted)) {
                fromBinary = reader.ReadFromBinary(s).Result;
            }

            using (Stream s = new MemoryStream(Resources.Samples.GameText)) {
                fromText = reader.ReadFromText(s).Result;
            }

            Assert.AreEqual(fromBinary.Children.Count, fromText.Children.Count);
        }

        [TestMethod]
        public void TestReadSiiFile () {
            SiiFileReader reader = new SiiFileReader();
            Object fromBinary, fromText;

            using (MemoryStream s = new MemoryStream(Resources.Samples.GameEncrypted)) {
                fromBinary = reader.ReadSiiFile(s).Result;
            }

            using (Stream s = new MemoryStream(Resources.Samples.GameText)) {
                fromText = reader.ReadSiiFile(s).Result;
            }

            Assert.AreEqual(fromBinary.Children.Count, fromText.Children.Count);
        }
    }
}
