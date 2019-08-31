using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    /// <summary>
    /// Header of a Scs archive.
    /// </summary>
    public class ScsArchiveHeader {
        /// <summary>
        /// Scs archive version.
        /// </summary>
        public UInt32 Version { get; internal set; }

        /// <summary>
        /// Hashing algorithm used by the HashFs implementation.
        /// </summary>
        public string HashingAlgorithm { get; internal set; }

        /// <summary>
        /// Number of entries in the central directory.
        /// </summary>
        public UInt32 EntryCount { get; internal set; }

        /// <summary>
        /// Offset from the beginning of file where the central directory is located.
        /// </summary>
        public UInt32 CentralDirectoryAddress { get; internal set; }
    }
}
