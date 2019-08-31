using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class PlacementTests {
        [TestMethod]
        public void TestConstructor () {
            Placement p = new Placement(new float[] { 1, 2, 3 }, new float[] { 4, 5, 6, 7 });

            CollectionAssert.AreEqual(p.Location, new float[] { 1, 2, 3 });
            CollectionAssert.AreEqual(p.Rotation, new float[] { 4, 5, 6, 7 });
        }

        [TestMethod]
        public void TestEquals () {
            Placement p1 = new Placement(new float[] { 1, 2, 3 }, new float[] { 4, 5, 6, 7 });
            Placement p2 = new Placement(new float[] { 1, 2, 3 }, new float[] { 4, 5, 6, 7 });
            Placement p3 = new Placement(new float[] { 0, 2, 3 }, new float[] { 4, 5, 6, 7 });
            Placement p4 = new Placement(new float[] { 1, 2, 3 }, new float[] { 0, 5, 6, 7 });

            Assert.IsTrue(p1.Equals(p2));
            Assert.IsFalse(p1.Equals(p3));
            Assert.IsFalse(p1.Equals(p4));
        }
    }
}
