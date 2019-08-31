using System;
using System.Numerics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Expectation = System.Tuple<string, object>;

namespace Drive.SiiFile.Tests {
    [TestClass]
    public class AttributeSerializerTests {
        [TestMethod]
        public void TestSerializeAttribute () {
            const string stringValue = "test string";
            BigInteger intValue = 10;
            const string ptrValue = "my.super.named.token.ohyeah";

            IEnumerable<Expectation> expects = new List<Expectation> {
                new Expectation($"\"{stringValue}\"", stringValue),
                new Expectation(intValue.ToString(), intValue),
                new Expectation("&3f200000", 0.625f),
                new Expectation("true", true),
                new Expectation(ptrValue, new Identifier(ptrValue)),
                new Expectation($"({intValue}, {intValue}, {intValue})", new BigInteger[] { intValue, intValue, intValue }),
                new Expectation(
                    $"({intValue}, {intValue}, {intValue}) (&3f200000; &3f200000, &3f200000, &3f200000)",
                    new Placement(new float[] { (float)intValue, (float)intValue, (float)intValue }, new float[] { 0.625f, 0.625f, 0.625f, 0.625f }))
            };

            foreach (Expectation ex in expects)
                Assert.AreEqual(AttributeSerializer.SerializeAttributeValue(ex.Item2), ex.Item1);
        }
    }
}
