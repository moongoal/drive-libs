using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.ScsArchive.Tests {
    [TestClass]
    public class HashFsFileNameTests {
        [TestMethod]
        public void TestCtor () {
            HashFsFileName fileName;

            fileName = new HashFsFileName("file");
            Assert.IsFalse(fileName.IsDirectory);
            Assert.AreEqual("file", fileName.Name);

            fileName = new HashFsFileName("*dir");
            Assert.IsTrue(fileName.IsDirectory);
            Assert.AreEqual("dir", fileName.Name);

            fileName = new HashFsFileName("file", "/def");
            Assert.IsFalse(fileName.IsDirectory);
            Assert.AreEqual("def/file", fileName.Name);

            fileName = new HashFsFileName("*dir", "/def/");
            Assert.IsTrue(fileName.IsDirectory);
            Assert.AreEqual("def/dir", fileName.Name);
        }

        [TestMethod]
        public void TestHashGeneration () {
            string rootFolder = "";
            string defFolder = "def";
            ulong rootHash = 0x9AE16A3B2F90404F;
            ulong defHash = 0x2C6F469EFB31C45A;

            Assert.AreEqual(rootHash, HashFsFileName.GetHashForFullPath(rootFolder));
            Assert.AreEqual(defHash, HashFsFileName.GetHashForFullPath(defFolder));

        }
    }
}
