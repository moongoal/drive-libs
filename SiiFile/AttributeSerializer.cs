using System;
using System.Text;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Drive.SiiFile {
    /// <summary>
    /// Facility to write the attributes to strings.
    /// </summary>
    public static class AttributeSerializer {

        /// <summary>
        /// Serialize an attribute to a string.
        /// </summary>
        /// <param name="attr">The attribute to serialize</param>
        /// <returns>A string containing the serialized attribute</returns>
        public static string SerializeAttribute (Attribute attr) => SerializeRawAttribute(attr.Name, attr.Value);

        /// <summary>
        /// Serialize an attribute from its raw components.
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns>A string containing the serialized attribute</returns>
        private static string SerializeRawAttribute (string name, object value) => $"{name}: {SerializeAttributeValue(value)}";

        /// <summary>
        /// Serialize the length part of a static array.
        /// </summary>
        /// <param name="attr">The array attribute</param>
        /// <returns>A string containing the serialized array length</returns>
        public static string SerializeArrayLength (Attribute attr) => SerializeRawAttribute(attr.Name, (BigInteger)((object[])attr.Value).Length);

        /// <summary>
        /// Serialize one item of an array.
        /// </summary>
        /// <param name="attr">The array attribute</param>
        /// <param name="index">The index of the item to serialize</param>
        /// <param name="isDynamic">True if the array is dynamic</param>
        /// <returns>A string containing the serialized array item</returns>
        public static string SerializeArrayItem (Attribute attr, int index, bool isDynamic) {
            string arrayName = attr.Name + "[" + (isDynamic ? "" : index.ToString()) + "]";

            return SerializeRawAttribute(arrayName, ((object[])attr.Value)[index]);
        }

        /// <summary>
        /// Serialize an attribute's raw value.
        /// </summary>
        /// <param name="value">The attribute's value to serialize</param>
        /// <returns>A string containing the serialized value</returns>
        public static string SerializeAttributeValue (object value) {
            switch (value) {
                case string strValue:
                    return $"\"{strValue}\"";

                case Identifier ptrValue:
                    return ptrValue.Value;

                case UnknownValue unkValue:
                    return unkValue.Value;

                case bool boolValue:
                    return boolValue.ToString().ToLower();

                case BigInteger intValue:
                    return intValue.ToString();

                case float decValue:
                    return SerializeDecimalValue(decValue);

                case float[] decVector:
                    return SerializeDecimalVectorValue(decVector);

                case BigInteger[] intVector:
                    return SerializeIntegerVectorValue(intVector);

                case Placement placeValue:
                    return SerializePlacementValue(placeValue);

                default:
                    throw new ArgumentException("No logic available to convert type " + value.GetType());
            }
        }

        /// <summary>
        /// Serialize a decimal value.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns>A string containing the serialized value</returns>
        private static string SerializeDecimalValue (float value) {
            float tvalue = (float)Math.Truncate(value);

            if (value == tvalue)
                return ((int)tvalue).ToString();
            else {
                byte[] valueBytes = BitConverter.GetBytes(value);
                StringBuilder builder = new StringBuilder("&");

                Array.Reverse(valueBytes);

                foreach (byte b in valueBytes)
                    builder.Append(b.ToString("x").PadLeft(2, '0'));

                return builder.ToString();
            }
        }

        /// <summary>
        /// Serialize a decimal vector value.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns>A string containing the serialized value</returns>
        private static string SerializeDecimalVectorValue (float[] value) {
            StringBuilder b = new StringBuilder("(");

            for (int i = 0; i < value.Length - 1; i++)
                b.Append(string.Format("{0}, ", SerializeDecimalValue(value[i])));

            b.Append(string.Format("{0})", SerializeDecimalValue(value[value.Length - 1])));

            return b.ToString();
        }

        /// <summary>
        /// Serialize a integer vector value.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns>A string containing the serialized value</returns>
        private static string SerializeIntegerVectorValue (BigInteger[] value) {
            StringBuilder b = new StringBuilder("(");

            for (int i = 0; i < value.Length - 1; i++)
                b.Append(string.Format("{0}, ", value[i].ToString()));

            b.Append(string.Format("{0})", value[value.Length - 1].ToString()));

            return b.ToString();
        }

        /// <summary>
        /// Serialize a placement value.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns>A string containing the serialized value</returns>
        private static string SerializePlacementValue (Placement value) {
            string pos = SerializeDecimalVectorValue(value.Location);
            string rot = SerializeDecimalVectorValue(value.Rotation);
            StringBuilder rotBuilder = new StringBuilder(rot);

            rotBuilder[rot.IndexOf(',')] = ';'; // Clear separation of W

            return $"{pos} {rotBuilder.ToString()}";
        }
    }
}
