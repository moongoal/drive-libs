using System;

namespace Drive.SiiFile {
    /// <summary>
    /// Thrown when an exception occurs upon parsing.
    /// </summary>
    public class SiiParseException : Exception {
        /// <summary>
        /// The line number where the exception was thrown or <code>null</code> if no line was given.
        /// </summary>
        int? LineNumber { get; }

        /// <summary>
        /// Initialize a new instance of this class with no line number information.
        /// </summary>
        public SiiParseException () { LineNumber = null; }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="lineNo">The line number within the Sii file where the exception happened</param>
        public SiiParseException (int lineNo) { LineNumber = lineNo; }
    }
}
