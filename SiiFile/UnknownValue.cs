using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.SiiFile {
    /// <summary>
    /// Represents an attribute value that has not been recognized/deserialized.
    /// The purpose of this class is to allow retention of the "unknown" state of
    /// the given value so that it can be written verbatim and not mistekn for a
    /// legit string.
    /// </summary>
    public struct UnknownValue {
        /// <summary>
        /// The raw value as a string.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="value">The raw unkown value as returned from the parser</param>
        public UnknownValue(string value) {
            Value = value;
        }

        public static implicit operator string (UnknownValue value) => value.Value;
    }
}
