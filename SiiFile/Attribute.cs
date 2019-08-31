namespace Drive.SiiFile {
    /// <summary>
    /// Represents a Sii object attribute.
    /// </summary>
    public class Attribute {
        /// <summary>
        /// Name of attribute
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value of attribute
        /// </summary>
        public object Value { get; internal set; }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="name">Name of attribute</param>
        /// <param name="value">Value of attribute</param>
        internal Attribute (string name, object value) {
            Name = name;
            Value = value;
        }

        public override bool Equals (object obj) {
            Attribute objAttr = obj as Attribute;

            if (objAttr != null)
                Name.Equals(objAttr.Name);

            return false;
        }

        public override int GetHashCode () => Name.GetHashCode();
    }
}
