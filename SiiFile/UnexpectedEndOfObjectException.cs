using System;

namespace Drive.SiiFile {
    /// <summary>
    /// Thrown whenever an object's end is unexpectedly reached.
    /// </summary>
    public class UnexpectedEndOfObjectException : Exception {
        /// <summary>
        /// The name of the faulty object.
        /// An empty string if none was provided.
        /// </summary>
        public string ObjectName { get; }

        /// <summary>
        /// Initialize a new instance of this class without an object's name.
        /// </summary>
        public UnexpectedEndOfObjectException() { ObjectName = string.Empty; }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="objectName">The name of the faulty object</param>
        public UnexpectedEndOfObjectException(string objectName) { ObjectName = objectName; }
    }
}
