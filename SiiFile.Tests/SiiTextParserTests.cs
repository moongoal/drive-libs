using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class SiiTextParserTests {
        [TestMethod]
        public void TestIsObjectBeginning () {
            string[] lines1 = {
                "class : instance {",
                "}"
            };
            string[] lines2 = {
                "class : instance",
                "{",
                "}"
            };
            string[] lines3 = {
                "class : instance",
                "}"
            };
            int i = -1;

            Assert.IsTrue(SiiTextParser.IsObjectBeginning(lines1, 0, ref i));
            Assert.AreEqual(i, 0);

            Assert.IsTrue(SiiTextParser.IsObjectBeginning(lines2, 0, ref i));
            Assert.AreEqual(i, 1);

            i = -1;
            Assert.IsFalse(SiiTextParser.IsObjectBeginning(lines3, 0, ref i));
            Assert.AreEqual(i, -1);
        }

        [TestMethod]
        public void TestGetObjectEndLineIndex () {
            string[] lines = {
                "class : instance {",
                "something: 5",
                "something_else: a.b.c",
                "}",
                "..."
            };

            Assert.AreEqual(SiiTextParser.GetObjectEndLineIndex(lines, 0), 3);
        }

        [TestMethod]
        public void TestParseAttribute () {
            (string name, string value) attr = SiiTextParser.ParseAttribute("MyAttr: 4");

            Assert.AreEqual(attr.name, "MyAttr");
            Assert.AreEqual(attr.value, "4");
        }

        [TestMethod]
        public void TestSanitizeSource () {
            string source = Resources.Samples.UnitWithComments.Replace("\r", "");
            string expectedResult = Resources.Samples.UnitWithoutComments.Replace("\r", "");

            Assert.AreEqual(SiiTextParser.SanitizeSource(source), expectedResult);
        }
    }
}
