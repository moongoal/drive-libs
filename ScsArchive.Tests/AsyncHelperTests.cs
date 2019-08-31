using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.ScsArchive.Tests {
    [TestClass]
    public class AsyncHelperTests {
        [TestMethod]
        public void TestFillBuffer () {
            Random r = new Random();
            byte[] inputBuf = new byte[] { 1, 2, 3, 4, 5 };
            byte[] inputBuf2 = new byte[1024 * 1024];

            r.NextBytes(inputBuf2);

            TestFillBufferWith(inputBuf);
            TestFillBufferWith(inputBuf2);
        }

        private void TestFillBufferWith(byte[] inputBuf) {
            byte[] outputBuf = new byte[inputBuf.Length];

            using (Stream s = new MemoryStream(inputBuf)) {
                Task.WaitAll(AsyncHelper.FillBuffer(s, outputBuf));
                CollectionAssert.AreEqual(inputBuf, outputBuf);
            }
        }
    }
}
