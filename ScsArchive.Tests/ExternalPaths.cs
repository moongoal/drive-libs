using System.IO;

namespace Drive.ScsArchive.Tests {
    static class ExternalPaths {
        public static readonly string GameInstallPath = @"D:\SteamLibrary\steamapps\common\Euro Truck Simulator 2";
        public static readonly string DefScsFilePath = Path.Combine(GameInstallPath, "def.scs");
    }
}
