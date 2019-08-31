using System;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class ObjectSerializerTests {
        [TestMethod]
        public async Task TestSerializeObject () {
            string tmpPath = Path.GetTempFileName();

            try {
                using (FileStream fs = new FileStream(tmpPath, FileMode.Create, FileAccess.Write)) {
                    MemoryStream ms = new MemoryStream(Resources.Samples.GameText);
                    SiiFileReader reader = new SiiFileReader();
                    Object obj = await reader.ReadSiiFile(ms);
                    byte[] b = Encoding.UTF8.GetBytes(ObjectSerializer.SerializeObject(obj));
                    fs.Write(b, 0, b.Length);
                }
            } finally {
                if (File.Exists(tmpPath)) {
                    File.Delete(tmpPath);
                }
            }
            
        }
    }
}
