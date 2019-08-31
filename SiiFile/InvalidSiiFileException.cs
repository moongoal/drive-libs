using System;
using System.Runtime.Serialization;

namespace Drive.SiiFile {
    /// <summary>
    /// Thrown when trying to load information from a file which is not a Sii unit file.
    /// </summary>
    [Serializable]
    internal class InvalidSiiFileException : Exception {
        public InvalidSiiFileException () {
        }

        public InvalidSiiFileException (string message) : base(message) {
        }

        public InvalidSiiFileException (string message, Exception innerException) : base(message, innerException) {
        }

        protected InvalidSiiFileException (SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}