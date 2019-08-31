using System;
using System.Numerics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Drive.SiiFile {
    /// <summary>
    /// Helper class to parse attribute values.
    /// </summary>
    public static class AttributeDeserializer {
        private static readonly Regex StringRxp = new Regex("^\".*\"$");
        private static readonly Regex IntegerRxp = new Regex(@"^-?(0x[a-fA-F\d]+|\d+)$");
        private static readonly Regex DecimalRxp = new Regex(@"^((&[\da-fA-F]+)|(\d*.\d+))$");
        private static readonly Regex VectorRxp = new Regex(@"^\([\da-fA-F,;\s&x-]+\)$");
        private static readonly Regex PlacementRxp = new Regex(@"^\([\da-fA-F,;\s&x-]+\)\s*\([\da-fA-F,;\s&x-]+\)$");
        private static readonly Regex BoolRxp = new Regex(@"^(true|false)$");
        private static readonly Regex PtrRxp = new Regex(@"^[.a-zA-Z_][.a-zA-Z0-9_]*$");
        private static readonly Regex ArrayRxp = new Regex(@"\[(\d+)?\]$");

        /// <summary>
        /// Convert an attribute name/(raw)value pair into an Attribute object.
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">String representation of the value</param>
        /// <returns>An attribute containing the parsed value</returns>
        public static Attribute ParseAttribute (string name, string value) {
            Func<string, object> deserializer;

            if (StringRxp.IsMatch(value))
                deserializer = ParseStringAttribute;
            else if (IntegerRxp.IsMatch(value))
                deserializer = ParseIntegerAttribute;
            else if (DecimalRxp.IsMatch(value))
                deserializer = ParseDecimalAttribute;
            else if (BoolRxp.IsMatch(value))
                deserializer = ParseBooleanAttribute;
            else if (PtrRxp.IsMatch(value))
                deserializer = ParsePointerAttribute;
            else if (VectorRxp.IsMatch(value))
                deserializer = ParseVectorAttribute;
            else if (PlacementRxp.IsMatch(value))
                deserializer = ParsePlacementAttribute;
            else
                deserializer = ParseUnknownAttribute;

            object deserializedValue = deserializer(value);
            string attributeName = name;

            if (IsArrayItem(name)) {
                attributeName = name.Substring(0, name.IndexOf('['));

                return new ArrayItemAttribute(
                    attributeName,
                    deserializedValue,
                    ArrayIndex(name)
                    );
            }

            return new Attribute(
                attributeName,
                deserializedValue
                );
        }

        /// <summary>
        /// Return true if the attribute's name states an array item
        /// </summary>
        /// <param name="name">Name of the attribute to examine</param>
        /// <returns><code>True</code> if the attribute is an item of an array</returns>
        private static bool IsArrayItem (string name) => ArrayRxp.IsMatch(name);

        /// <summary>
        /// Extract the index of an array item attribute
        /// </summary>
        /// <param name="name">Name of the attribute to examine</param>
        /// <returns>The index of the attribute in its array or <code>null</code> if none is present</returns>
        private static uint? ArrayIndex (string name) {
            Match match = ArrayRxp.Match(name);

            if (uint.TryParse(match?.Groups[1].Value, out uint idx)) {
                return idx;
            }

            return null;
        }

        /// <summary>
        /// Parse an integer attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed value as a <code>BigInteger</code></returns>
        private static object ParseIntegerAttribute (string value) {
            bool isHex = value.ToLower().StartsWith("0x");

            return BigInteger.Parse(
                isHex ? value.Substring(2) : value,
                isHex ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.AllowLeadingSign
                );
        }

        /// <summary>
        /// Parse a floating point attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed value as a <code>float</code></returns>
        private static object ParseDecimalAttribute (string value) {
            try {
                return (float)(BigInteger)ParseIntegerAttribute(value);
            } catch {
                if (float.TryParse(value, out float res))
                    return res;

                return BitConverter.ToSingle(
                    new byte[] {
                    Convert.ToByte(value.Substring(7, 2), 16),
                    Convert.ToByte(value.Substring(5, 2), 16),
                    Convert.ToByte(value.Substring(3, 2), 16),
                    Convert.ToByte(value.Substring(1, 2), 16)
                    }, 0
                );
            }
        }

        /// <summary>
        /// Parse a string attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed value as a <code>string</code></returns>
        private static object ParseStringAttribute (string value) {
            return value.Substring(1, value.Length - 2);
        }

        /// <summary>
        /// Parse an unknown attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed value as an <code>UnknownValue</code> object</returns>
        /// <see cref="Drive.SiiFile.UnknownValue"/>
        private static object ParseUnknownAttribute (string value) {
            return new UnknownValue(value);
        }

        /// <summary>
        /// Parse a boolean attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed value as a <code>bool</code></returns>
        private static object ParseBooleanAttribute (string value) {
            return bool.Parse(value);
        }

        /// <summary>
        /// Parse a pointer or token attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed value as an <code>Identifier</code> object</returns>
        /// <see cref="Drive.SiiFile.Identifier"/>
        private static object ParsePointerAttribute (string value) {
            return new Identifier(value);
        }

        /// <summary>
        /// Parse a vector attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed value as either a <code>BigInteger</code> or <code>float</code> array</returns>
        private static object ParseVectorAttribute (string value) => ParseVectorAttribute(value, false);

        /// <summary>
        /// Parse a vector attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <param name="forceDecimal">True to force treating the value as a floating point vector</param>
        /// <returns>The parsed value as either a <code>BigInteger</code> or <code>float</code> array</returns>
        private static object ParseVectorAttribute (string value, bool forceDecimal) {
            string[] svec = value.Substring(1, value.Length - 2).Split(',', ';', ')', '(').Select(x => x.Trim()).Where(x => !(x.Equals('(') || x.Equals(')') || string.IsNullOrWhiteSpace(x))).ToArray();
            bool intVector = true;

            foreach (string vecElem in svec) {
                intVector &= IntegerRxp.IsMatch(vecElem);
            }

            return intVector && !forceDecimal ? (object)ParseIntegerVector(svec) : (object)ParseFloatVector(svec);
        }

        /// <summary>
        /// Parse an integer vector attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed vector as a <code>BigInteger</code> array</returns>
        private static BigInteger[] ParseIntegerVector (string[] values) {
            BigInteger[] vec = new BigInteger[values.Length];

            for (int i = 0; i < values.Length; i++)
                vec[i] = (BigInteger)ParseIntegerAttribute(values[i]);

            return vec;
        }

        /// <summary>
        /// Parse a floating point vector attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed vector as a <code>float</code> array</returns>
        private static float[] ParseFloatVector (string[] values) {
            float[] vec = new float[values.Length];

            for (int i = 0; i < values.Length; i++)
                vec[i] = (float)ParseDecimalAttribute(values[i]);

            return vec;
        }

        /// <summary>
        /// Parse a placement attribute
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed value as a <code>Placement</code> array</returns>
        /// <see cref="Drive.SiiFile.Placement"/>
        private static object ParsePlacementAttribute (string value) {
            int qstart = value.IndexOf(" (");
            string spos = value.Substring(0, qstart);
            string srot = value.Substring(qstart + 1);

            return new Placement(
                (float[])ParseVectorAttribute(spos, forceDecimal: true),
                (float[])ParseVectorAttribute(srot, forceDecimal: true)
            );
        }
    }
}
