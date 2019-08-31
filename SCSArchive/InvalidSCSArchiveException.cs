using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    [Serializable]
    public class InvalidScsArchiveException : Exception {
        public InvalidScsArchiveException () { }
        public InvalidScsArchiveException (string message) : base(message) { }
        public InvalidScsArchiveException (string message, Exception inner) : base(message, inner) { }
        protected InvalidScsArchiveException (
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
