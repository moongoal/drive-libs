using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.ScsArchive.Tests {
    [TestClass]
    public class HashFsEntryTests {
        [TestMethod]
        public void TestProperties () {
            HashFsEntry entry;

            entry = new HashFsEntry { Type = HashFsEntryType.CompressedDirectory };
            Assert.IsTrue(entry.IsDirectory);
            Assert.IsTrue(entry.IsCompressed);

            entry = new HashFsEntry { Type = HashFsEntryType.UncompressedDirectory };
            Assert.IsTrue(entry.IsDirectory);
            Assert.IsFalse(entry.IsCompressed);

            entry = new HashFsEntry { Type = HashFsEntryType.CompressedFile };
            Assert.IsFalse(entry.IsDirectory);
            Assert.IsTrue(entry.IsCompressed);

            entry = new HashFsEntry { Type = HashFsEntryType.UncompressedFile };
            Assert.IsFalse(entry.IsDirectory);
            Assert.IsFalse(entry.IsCompressed);
        }
    }
}
