using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.ScsArchive.Tests {
    [TestClass]
    public class HashFsEntryReaderTests {
        private const ulong HASH_ROOT_FOLDER = 0x9AE16A3B2F90404F;

        [TestMethod]
        public void TestEntryReadingMethods () {
            using (ScsArchiveReader reader = new ScsArchiveReader(ExternalPaths.DefScsFilePath)) {
                IReadOnlyDictionary<ulong, HashFsEntry> entries = reader.GetEntries().Result;
                HashFsEntry rootEntry = entries[HASH_ROOT_FOLDER];
                HashFsEntryReader rootEntryReader = new HashFsEntryReader(rootEntry);
                IEnumerable<HashFsFileName> names = rootEntryReader.ListFiles().Result;

                using (Stream s = rootEntryReader.GetStream()) {
                    byte[] streamData = new byte[rootEntry.UncompressedSize];

                    while (s.Position < streamData.Length)
                        s.Read(streamData, (int)s.Position, streamData.Length);

                    byte[] contentData = rootEntryReader.GetContent().Result;

                    CollectionAssert.AreEqual(streamData, contentData);

                    string contentDataString = Encoding.UTF8.GetString(contentData);
                    string contentString = rootEntryReader.GetContent(Encoding.UTF8).Result;

                    Assert.AreEqual(contentDataString, contentString);

                    foreach (HashFsFileName n in names) {
                        string realName = (n.IsDirectory ? "*" : "") + n.Name;

                        Assert.IsTrue(contentString.Contains(realName));
                    }
                }
            }
        }

        [TestMethod]
        public void TestGetDirectoryFileNames () {
            using (ScsArchiveReader reader = new ScsArchiveReader(ExternalPaths.DefScsFilePath)) {
                IReadOnlyDictionary<ulong, HashFsEntry> entries = reader.GetEntries().Result;
                HashFsEntry rootEntry = entries[HASH_ROOT_FOLDER];
                HashFsEntryReader rootEntryReader = new HashFsEntryReader(rootEntry);
                IEnumerable<HashFsFileName> names = rootEntryReader.ListFiles().Result;

                Assert.AreEqual(1, (from HashFsFileName n in names where n.Name.Equals("def") select n).Count());
                Assert.AreEqual(0, (from HashFsFileName n in names where n.Name.Equals("egfoaepbwgp") select n).Count());
            }
        }
    }
}
