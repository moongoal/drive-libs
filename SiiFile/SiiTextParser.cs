using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.SiiFile {
    /// <summary>
    /// Sii text parser.
    /// </summary>
    public static class SiiTextParser {
        /// <summary>
        /// Return a copy of source where all comments have been removed.
        /// </summary>
        /// <param name="source">The source code string</param>
        /// <returns>A sanitized version of the <para>source</para> string.</returns>
        public static string SanitizeSource (string source) {
            int lineNo = 1;
            bool inString = false;
            StringBuilder output = new StringBuilder(source.Length);

            for (int i = 0; i < source.Length; i++) {
                try {
                    switch (source[i]) {
                        case '#':
                            if (!inString)
                                i = source.IndexOf('\n', i + 1);
                            break;

                        case '/':
                            if (!inString) {
                                if (source[i + 1] == '/')
                                    i = source.IndexOf('\n', i + 2);
                                else if (source[i + 1] == '*')
                                    i = source.IndexOf("*/", i + 2) + 2;
                            }
                            break;

                        case '"':
                            inString = !inString;
                            break;
                    }
                } catch (ArgumentOutOfRangeException) {
                    throw new SiiParseException(lineNo);
                }

                if (source[i] == '\n')
                    lineNo += 1;

                output.Append(source[i]);
            }

            return string.Join(
                "\n",
                output.ToString().Split('\n').Select(x => x.Trim()));
        }

        /// <summary>
        /// Parse an attribute line
        /// </summary>
        /// <param name="content">The raw attribute line</param>
        /// <returns>The attribute name and raw value</returns>
        public static (string name, string value) ParseAttribute (string content) {
            string[] vals = content.Split(new char[] { ':' }, 2);

            return (vals[0].Trim(), vals[1].Trim());
        }

        /// <summary>
        /// Return the next object end index (ending boundary).
        /// </summary>
        /// <param name="lines">Sii source code lines</param>
        /// <param name="objectStart">Starting line of the object</param>
        /// <returns>The index of the last line of the object</returns>
        public static int GetObjectEndLineIndex (string[] lines, int objectStart) {
            int nestingLevel = 0;

            for (int i = objectStart; i < lines.Length; i++) {
                if (IsObjectBeginning(lines, i, ref i))
                    nestingLevel += 1; // Entering object
                else if (lines[i].Equals("}")) {
                    nestingLevel -= 1; // Exiting object

                    if (nestingLevel == 0) { // Top-level object end reached
                        return i;
                    }
                }
            }

            throw new UnexpectedEndOfObjectException(lines[0].Split(':')[1].Trim(' ', '{'));
        }

        /// <summary>
        /// Tell whether the given position in an array of source lines marks the beginning of an object.
        /// Note the array has to be at least <code>offset + 1</code> elements long.
        /// </summary>
        /// <param name="lines">The array of Sii file lines</param>
        /// <param name="offset">The number of lines to skip in the array before performing the check</param>
        /// <param name="objBeginning">If a new object beginning is found, sets the new value for the line cursor at the opening brace's line</param>
        /// <returns>True if an object is beginning at the specified location.</returns>
        public static bool IsObjectBeginning (string[] lines, int offset, ref int objBeginning) {
            if (lines[offset].EndsWith("{")) {
                objBeginning = offset;
            } else if (offset < lines.Length - 1 && lines[offset + 1].Equals("{")) {
                objBeginning = offset + 1;
            } else return false;

            return true;
        }

        /// <summary>
        /// Split the Sii source code into its lines.
        /// </summary>
        /// <param name="source">The raw Sii file source code</param>
        /// <returns>An enumerable object of text lines representing the input source</returns>
        public static IEnumerable<string> SplitSourceToLines (string source) => (from string line in source.Split('\n')
                                                                                 where !line.Equals(string.Empty)
                                                                                 select line);
    }
}
