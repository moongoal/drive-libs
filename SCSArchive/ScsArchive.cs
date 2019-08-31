using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    /// <summary>
    /// Scs archive.
    /// All paths are *case sensitive*.
    /// </summary>
    public class ScsArchive {
        /// <summary>
        /// Central directory entries mapping from hash to entry.
        /// </summary>
        public IReadOnlyDictionary<ulong, HashFsEntry> Entries { get; }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="entries">Mapping hash to entry this object will use</param>
        internal ScsArchive(IReadOnlyDictionary<ulong, HashFsEntry> entries) {
            Entries = entries;
        }

        /// <summary>
        /// Tells whether an entry is present.
        /// </summary>
        /// <param name="path">Full path to check for existence</param>
        /// <returns>True if the entry is present</returns>
        public bool HasEntry (string path) => HasEntry(HashFsFileName.GetHashForFullPath(path));

        /// <summary>
        /// Tells whether an entry is present.
        /// </summary>
        /// <param name="hash">Hash of the full path to check for existence</param>
        /// <returns>True if the entry is present</returns>
        public bool HasEntry (ulong hash) => Entries.ContainsKey(hash);

        /// <summary>
        /// Get an entry.
        /// </summary>
        /// <param name="path">Full path of the entry</param>
        /// <returns>The retrieved entry</returns>
        public HashFsEntry GetEntry (string path) => GetEntry(HashFsFileName.GetHashForFullPath(path));

        /// <summary>
        /// Get an entry.
        /// </summary>
        /// <param name="hash">Hash of the full path of the entry</param>
        /// <returns>The retrieved entry</returns>
        public HashFsEntry GetEntry (ulong hash) => Entries[hash];
    }
}
