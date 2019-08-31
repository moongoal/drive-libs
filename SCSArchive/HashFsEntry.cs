using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    /// <summary>
    /// Entry within the HashFs central directory.
    /// </summary>
    public class HashFsEntry {
        /// <summary>
        /// Offset from the beginning of the file.
        /// </summary>
        public uint Offset { get; internal set; }

        /// <summary>
        /// Size of the compressed entry.
        /// </summary>
        public uint CompressedSize { get; internal set; }

        /// <summary>
        /// Size of the entry after decompression.
        /// </summary>
        public uint UncompressedSize { get; internal set; }

        /// <summary>
        /// Path of the Scs archive.
        /// </summary>
        public string ArchivePath { get; internal set; }

        /// <summary>
        /// Entry type.
        /// </summary>
        public HashFsEntryType Type { get; internal set; }

        /// <summary>
        /// CRC32 of the entry.
        /// </summary>
        public uint Crc32 { get; internal set; }

        /// <summary>
        /// True if the entry is a directory.
        /// </summary>
        public bool IsDirectory => Type == HashFsEntryType.CompressedDirectory || Type == HashFsEntryType.UncompressedDirectory;

        /// <summary>
        /// True if the entry is compressed.
        /// </summary>
        public bool IsCompressed => Type == HashFsEntryType.CompressedDirectory || Type == HashFsEntryType.CompressedFile;

        internal HashFsEntry () { }
    }
}
