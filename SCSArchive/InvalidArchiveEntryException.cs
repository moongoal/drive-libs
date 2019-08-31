using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    [Serializable]
    public class InvalidArchiveEntryException : Exception {
        public InvalidArchiveEntryException () { }
        public InvalidArchiveEntryException (string message) : base(message) { }
        public InvalidArchiveEntryException (string message, Exception inner) : base(message, inner) { }
        protected InvalidArchiveEntryException (
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
