using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Drive.SiiFile;
using System.Threading.Tasks;

namespace Drive.ScsArchive.Tests {
    [TestClass]
    public class GameFsIncludeProviderTests {
        [TestMethod]
        public void TestInclusion () {
            GameFs gameFs = new GameFs(ExternalPaths.GameInstallPath, null);
            Task.WaitAll(gameFs.Initialize());
            SiiFileReader reader = new SiiFileReader(gameFs.GetIncludeProvider("def"));

            Task.WaitAll(reader.ReadSiiFile(gameFs.OpenFile("def/country.sii")));
        }
    }
}
