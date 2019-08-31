using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.SiiFile {
    public class ArrayItemAttribute : Attribute {
        /// <summary>
        /// Index within array.
        /// </summary>
        public uint? Index { get; }

        /// <summary>
        /// True if this attribute represents an item in a dynamic array.
        /// </summary>
        public bool IsItemOfDynamicArray => Index == null;

        public ArrayItemAttribute (string name, object value, uint? index = null) : base(name, value) {
            Index = index;
        }
    }
}
