namespace Drive.SiiFile {
    /// <summary>
    /// Sii object identifier values (tokens & pointers).
    /// </summary>
    public struct Identifier {
        /// <summary>
        /// Identifier's value
        /// </summary>
        public string Value { get; }

        public Identifier(string value) {
            Value = value;
        }

        public static explicit operator Identifier (string ptr) => new Identifier(ptr);
        public static explicit operator string (Identifier ptr) => ptr.Value;
    }
}
