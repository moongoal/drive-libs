using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.ScsArchive.Tests {
    [TestClass]
    public class ScsArchiveReaderTests {
        private const ulong HASH_ROOT_FOLDER = 0x9AE16A3B2F90404F;
        private const ulong HASH_DEF_FOLDER = 0x2C6F469EFB31C45A;

        [ClassInitialize]
        public static void InitializeClass (TestContext ctx) {
            if (!File.Exists(ExternalPaths.DefScsFilePath))
                throw new FileNotFoundException("In order to run this test case, a valid *.scs file path must be set in " + nameof(ExternalPaths.DefScsFilePath));
        }

        [TestMethod]
        public void TestReads () {
            using (ScsArchiveReader reader = new ScsArchiveReader(ExternalPaths.DefScsFilePath)) {
                ScsArchiveHeader header = reader.GetHeader().Result;

                Assert.AreEqual("city", header.HashingAlgorithm.ToLower());
                Assert.AreEqual((uint)1, header.Version);
                Assert.AreEqual(header.EntryCount, (uint)reader.ReadArchive().Result.Entries.Count);
            }
        }

        [TestMethod]
        public void TestGetEntries () {
            using (ScsArchiveReader reader = new ScsArchiveReader(ExternalPaths.DefScsFilePath)) {
                IReadOnlyDictionary<ulong, HashFsEntry> entries = reader.GetEntries().Result;

                Assert.IsTrue(entries.ContainsKey(HASH_ROOT_FOLDER));
                Assert.IsTrue(entries.ContainsKey(HASH_DEF_FOLDER));
                Assert.IsFalse(entries.ContainsKey(0x678)); // Well, this is weak.
            }
        }

        [TestMethod]
        public void TestDispose () {
            ScsArchiveReader reader = new ScsArchiveReader(ExternalPaths.DefScsFilePath);

            Assert.IsTrue(reader.Stream.CanRead);

            reader.Dispose();
            Assert.IsFalse(reader.Stream.CanRead);
        }
    }
}
