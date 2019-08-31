using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.ScsArchive.Tests {
    [TestClass]
    public class GameFsTests {
        private const ulong HASH_ROOT_FOLDER = 0x9AE16A3B2F90404F;
        private const ulong HASH_DEF_FOLDER = 0x2C6F469EFB31C45A;

        [ClassInitialize]
        public static void InitializeClass(TestContext ctx) {
            if (!Directory.Exists(ExternalPaths.GameInstallPath))
                throw new DirectoryNotFoundException("In order to run this test case, a valid game install folder path must be set in " + nameof(ExternalPaths.GameInstallPath));
        }

        [TestMethod]
        public void TestGetInclusionProvider () {
            GameFs gameFs = new GameFs(ExternalPaths.GameInstallPath, null /* support for mods not present yet */);
            Drive.SiiFile.IInclusionProvider inclusionProvider = gameFs.GetIncludeProvider();

            Assert.IsNotNull(inclusionProvider);
            Assert.IsInstanceOfType(inclusionProvider, typeof(GameFsIncludeProvider));
        }

        [TestMethod]
        public void TestListFolder () {
            GameFs gameFs = new GameFs(ExternalPaths.GameInstallPath, null /* support for mods not present yet */);
            Task.WaitAll(gameFs.Initialize());
            IEnumerable<string> names = gameFs.ListFolder("").Result;

            foreach (string n in new string[] { "map", "model", "prefab", "model2", "def", "video" })
                Assert.IsTrue(names.Contains(n));
        }

        [TestMethod]
        public void TestOpenFile () {
            GameFs gameFs = new GameFs(ExternalPaths.GameInstallPath, null /* support for mods not present yet */);
            Task.WaitAll(gameFs.Initialize());
            IEnumerable<string> names = gameFs.ListFolder("").Result;

            using(Stream s = gameFs.OpenFile("autoexec.cfg")) { }

            Assert.ThrowsException<FileNotFoundException>(delegate {
                using (Stream s2 = gameFs.OpenFile("not.existing.file.y48wvhwvcuhiunciu")) { }
            });
        }
    }
}
