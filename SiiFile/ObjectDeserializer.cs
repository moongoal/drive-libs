using System;
using System.Numerics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Drive.SiiFile {
    /// <summary>
    /// Helper class to parse objects
    /// </summary>
    public static class ObjectDeserializer {
        /// <summary>
        /// Parse a Sii unit object.
        /// </summary>
        /// <param name="lines">Sii file source code lines</param>
        /// <param name="start">Object initial line</param>
        /// <param name="end">Object last line</param>
        /// <returns>The parsed object</returns>
        public static Object ParseObject (string[] lines, int start, int end) {
            IDictionary<string, Attribute> attrs = new Dictionary<string, Attribute>();
            IDictionary<string, Object> children = new Dictionary<string, Object>();
            int objectMetaIdx = lines[start].Equals("{") ? start - 1 : start;
            string[] meta = lines[objectMetaIdx].Split(':');
            string objName = meta.Length > 1 ? meta[1].Trim(' ', '{') : meta[0].Trim(' ', '{');
            string clsName = meta.Length > 1 ? meta[0].Trim() : "";

            for (
                int i = start + (lines[start + 1].Equals("{") ? 2 : 1);
                i < end;
                i++
            ) {
                string line = lines[i];
                bool isObject = SiiTextParser.IsObjectBeginning(lines, i, ref i);

                if (isObject) {
                    int objectStart = i;
                    int objectEnd = SiiTextParser.GetObjectEndLineIndex(lines, objectStart);
                    Object child = ParseObject(lines, objectStart, objectEnd);
                    children.Add(child.Name, child);

                    i += objectEnd - objectStart - 1;
                } else {
                    if (!line.Equals("}")) {
                        Attribute attr;

                        (string attrName, string attrValue) = SiiTextParser.ParseAttribute(line);
                        attr = AttributeDeserializer.ParseAttribute(attrName, attrValue);

                        if (attr is ArrayItemAttribute) {
                            AddArrayItem((ArrayItemAttribute)attr, attrs);
                        } else
                            attrs.Add(attr.Name, attr);
                    }
                }
            }

            return new Object(objName, clsName, children, attrs);
        }

        /// <summary>
        /// Add an attribute representing an array item to its corresponding array (represented by another attribute) found in a collection.
        /// </summary>
        /// <param name="attr">The array item's attribute</param>
        /// <param name="attributes">A collection of attributes containing the array</param>
        private static void AddArrayItem (ArrayItemAttribute attr, IDictionary<string, Attribute> attributes) {
            if (!attributes.TryGetValue(attr.Name, out Attribute attrArray)) { // Must be a not yet initialized dynamic array
                LinkedList<object> array = new LinkedList<object>();

                array.AddLast(attr.Value);
                attributes.Add(attr.Name, new Attribute(attr.Name, array));

                return;
            }

            examineArray: switch (attrArray.Value) {
                case object[] staticArray:
                    staticArray[(int)attr.Index] = attr.Value;
                    break;

                /*
                 * Warning: some indexed arrays are dynamic and this code won't preserve
                 * the correct order of items unless they are sorted.
                 */
                case LinkedList<object> dynamicArray:
                    dynamicArray.AddLast(attr.Value);
                    break;

                case BigInteger arrayLength:
                    attrArray.Value = new object[(int)arrayLength];
                    goto examineArray;
            }
        }
    }
}
