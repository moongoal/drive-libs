using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class ObjectTests {
        [TestMethod]
        public void TestCtor () {
            const string objName = "obj";
            const string objClass = "myClass";
            Object obj = new Object(objName, objClass, null, null);

            Assert.AreEqual(objName, obj.Name);
            Assert.AreEqual(objClass, obj.ClassName);
            Assert.IsNotNull(obj.Children);
            Assert.IsNotNull(obj.Attributes);
        }

        [TestMethod]
        public void TestGetChildrenByClassName () {
            Dictionary<string, Object> children = new Dictionary<string, Object> {
                { "a", new Object("a", "cls") },
                { "b", new Object("b", "cls") },
                { "c", new Object("c", "cls2") }
            };
            Object obj = new Object("myObj", "myCls", children);

            Assert.AreEqual(2, obj.GetChildrenByClassName("cls").Count());
            Assert.AreEqual(1, obj.GetChildrenByClassName("cls2").Count());
            Assert.AreEqual(0, obj.GetChildrenByClassName("cls3").Count());
        }
    }
}
