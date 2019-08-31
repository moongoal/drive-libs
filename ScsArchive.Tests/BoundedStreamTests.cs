using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.ScsArchive.Tests {
    [TestClass]
    public class BoundedStreamTests {
        private static readonly byte[] _buff = new byte[500];

        [ClassInitialize]
        public static void InitializeClass (TestContext ctx) {
            for (int i = 0; i < _buff.Length; i++)
                _buff[i] = (byte)(i % 256);
        }

        [TestMethod]
        public void TestBoundProperties () {
            using (MemoryStream baseStream = new MemoryStream(_buff)) {
                using (BoundedStream boundedStream = new BoundedStream(baseStream, 10, 100)) {
                    Assert.AreEqual(10, boundedStream.LowerBound);
                    Assert.AreEqual(100, boundedStream.UpperBound);
                }
            }
        }

        [TestMethod]
        public void TestPosition () {
            using (MemoryStream baseStream = new MemoryStream(_buff)) {
                using (BoundedStream boundedStream = new BoundedStream(baseStream, 10, 100)) {
                    Assert.AreEqual(0, boundedStream.Position);

                    boundedStream.ReadByte();
                    Assert.AreEqual(1, boundedStream.Position);

                    boundedStream.Position = 0;
                    Assert.AreEqual(0, boundedStream.Position);

                    Assert.ThrowsException<IOException>(() => boundedStream.Position = -1);
                    Assert.AreEqual(0, boundedStream.Position);

                    Assert.ThrowsException<IOException>(() => boundedStream.Position = boundedStream.UpperBound - boundedStream.LowerBound + 1);
                    Assert.AreEqual(0, boundedStream.Position);

                    Assert.ThrowsException<IOException>(() => boundedStream.Seek(-1, SeekOrigin.Begin));
                    Assert.AreEqual(0, boundedStream.Position);

                    Assert.ThrowsException<IOException>(() => boundedStream.Seek(-1, SeekOrigin.End));
                    Assert.AreEqual(0, boundedStream.Position);

                    Assert.ThrowsException<IOException>(() => boundedStream.Seek(-1, SeekOrigin.Current));
                    Assert.AreEqual(0, boundedStream.Position);

                    boundedStream.Seek(1, SeekOrigin.Begin);
                    Assert.AreEqual(1, boundedStream.Position);
                }
            }
        }

        [TestMethod]
        public void TestRead () {
            using (MemoryStream baseStream = new MemoryStream(_buff)) {
                using (BoundedStream boundedStream = new BoundedStream(baseStream, 10, 100)) {
                    Assert.AreEqual((boundedStream.Position + boundedStream.LowerBound) % 256, boundedStream.ReadByte() % 256);
                    Assert.AreEqual((boundedStream.Position + boundedStream.LowerBound) % 256, boundedStream.ReadByte() % 256);

                    // Partial reads to upper bound
                    boundedStream.Seek(1, SeekOrigin.End);
                    byte[] buff = new byte[2] { 0, 200 };
                    byte[] expectedBuff = new byte[2] { 99, 200 };

                    boundedStream.Read(buff, 0, buff.Length);
                    CollectionAssert.AreEqual(expectedBuff, buff);
                }
            }
        }
    }
}
