using System;
using System.Linq;
using System.Collections.Generic;

namespace Drive.SiiFile {
    /// <summary>
    /// Represent a Sii unit object.
    /// </summary>
    public class Object {
        private readonly Dictionary<string, Attribute> _attributes;

        /// <summary>
        /// Object name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Object class
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// Child objects
        /// </summary>
        public IDictionary<string, Object> Children { get; }

        /// <summary>
        /// Object own attributes
        /// </summary>
        public IDictionary<string, Attribute> Attributes => _attributes;

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="className">The class of the object</param>
        /// <param name="children">A dictionary containing the objects children of the one being constructed</param>
        /// <param name="attributes">An attribute collection containing the attributes of the object being constructed</param>
        public Object (string name, string className, IDictionary<string, Object> children = null, IDictionary<string, Attribute> attributes = null) {
            Name = name;
            ClassName = className;

            if (attributes != null)
                _attributes = new Dictionary<string, Attribute>(attributes);
            else
                _attributes = new Dictionary<string, Attribute>();

            if (children != null)
                Children = new Dictionary<string, Object>(children);
            else
                Children = new Dictionary<string, Object>();
        }

        /// <summary>
        /// Get an enumerable containing all the child objects of the given class.
        /// </summary>
        /// <param name="name">Name of the class to search for</param>
        /// <returns>An enumerable with all the found children</returns>
        public IEnumerable<Object> GetChildrenByClassName (string name) => from Object chld in Children.Values where chld.ClassName.Equals(name) select chld;
    }
}
