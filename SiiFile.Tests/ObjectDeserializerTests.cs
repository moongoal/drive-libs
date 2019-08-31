using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class ObjectDeserializerTests {
        [TestMethod]
        public void TestParseObject () {
            Object obj = ParseTextObject(Resources.Samples.Object);

            Assert.AreEqual(obj.ClassName, "class");
            Assert.AreEqual(obj.Name, "instance");
            Assert.AreEqual(obj.Children.Count(), 0);
            Assert.AreEqual(obj.Attributes.Count(), 2);

            Attribute a1 = obj.Attributes["attribute"];
            Attribute a2 = obj.Attributes["other_attribute"];

            Assert.AreEqual(a1.Value, (BigInteger)15);
            Assert.AreEqual(a2.Value, "Sdang");
        }

        [TestMethod]
        public void TestParseObjectWithStaticArray () {
            Object obj = ParseTextObject(Resources.Samples.ObjectWithStaticArray);
            object[] array = (object[])obj.Attributes.First().Value.Value;

            Assert.AreEqual((string)array[0], "donna");
            Assert.AreEqual((string)array[1], "cavallo");
            Assert.AreEqual((string)array[2], "re");
        }

        [TestMethod]
        public void TestParseObjectWithDynamicArray () {
            Object obj = ParseTextObject(Resources.Samples.ObjectWithDynamicArray);
            LinkedList<object> array = (LinkedList<object>)obj.Attributes.First().Value.Value;

            Assert.AreEqual((string)array.ElementAt(0), "donna");
            Assert.AreEqual((string)array.ElementAt(1), "cavallo");
            Assert.AreEqual((string)array.ElementAt(2), "re");
        }

        private Object ParseTextObject (string objText) {
            string[] objLines = objText.Split('\n').Select(x => x.Trim('\r')).ToArray();

            return ObjectDeserializer.ParseObject(objLines, 0, objLines.Length);
        }
    }
}
