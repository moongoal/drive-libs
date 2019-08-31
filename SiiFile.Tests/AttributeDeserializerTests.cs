using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Expectation = System.Tuple<string, object>;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class AttributeDeserializerTests {
        [TestMethod]
        public void TestParseAttributeScalars () {
            const string stringValue = "test string";
            const string stringValue2 = "test \\\"string\\\" here";
            BigInteger intValue = 11;
            float floatValue = 0.625f;
            const string ptrValue = "my.super.named.token.ohyeah";
            const string ptrValue2 = ".super.local.token.ohyeah";

            IEnumerable<Expectation> expects = new List<Expectation> {
                new Expectation($"\"{stringValue}\"", stringValue),
                new Expectation($"\"{stringValue2}\"", stringValue2),
                new Expectation(intValue.ToString(), intValue),
                new Expectation("0x" + intValue.ToString("x"), intValue),
                new Expectation((-intValue).ToString(), -intValue),
                new Expectation("&3f200000", floatValue),
                new Expectation(floatValue.ToString(), floatValue),
                new Expectation("true", true),
                new Expectation("false", false),
                new Expectation(ptrValue, new Identifier(ptrValue)),
                new Expectation(ptrValue2, new Identifier(ptrValue2))
            };

            foreach (Expectation ex in expects)
                ParseAttributeScalarTestPart(ex.Item1, ex.Item2);
        }

        private static void ParseAttributeScalarTestPart (string inputValue, object expectedOutputValue) {
            Assert.AreEqual(AttributeDeserializer.ParseAttribute("", inputValue).Value, expectedOutputValue);
        }

        [TestMethod]
        public void TestParseAttributeVectors () {
            IEnumerable<Expectation> expects = new List<Expectation> {
                new Expectation("(1, 2,  3)", new BigInteger[] { 1, 2, 3 }),
                new Expectation("(1, -2,  0x3)", new BigInteger[] { 1, -2, 3 }),
                new Expectation("(1, 2, &3f200000)", new float[] {1f, 2f, 0.625f}),
            };

            foreach (Expectation ex in expects)
                ParseAttributeVectorTestPart(ex.Item1, ex.Item2);
        }

        private static void ParseAttributeVectorTestPart (string inputValue, object expectedOutputValue) {
            Attribute attr = AttributeDeserializer.ParseAttribute("", inputValue);

            CollectionAssert.AreEqual((ICollection)attr.Value, (ICollection)expectedOutputValue);
        }

        [TestMethod]
        public void TestParseAttributePlacement () {
            Attribute attr = AttributeDeserializer.ParseAttribute("", "(0, 1, 0) (12; 6, 6, 6)");
            Attribute attr2 = AttributeDeserializer.ParseAttribute("", "(0, 1, 0) (0x0c; 6, 6, 6)");
            Attribute attr3 = AttributeDeserializer.ParseAttribute("", "(0, -1, 0) (0x0c; 6, 6, 6)");
            float[] placLoc = new float[] { 0, 1, 0 };
            float[] placLoc2 = new float[] { 0, -1, 0 };
            float[] placRot = new float[] { 12, 6, 6, 6 };

            Assert.AreEqual(
                new SiiFile.Placement(placLoc, placRot),
                (SiiFile.Placement)attr.Value
                );
            Assert.AreEqual(
                new SiiFile.Placement(placLoc, placRot),
                (SiiFile.Placement)attr2.Value
                );
            Assert.AreEqual(
                new SiiFile.Placement(placLoc2, placRot),
                (SiiFile.Placement)attr3.Value
                );
        }
    }
}
