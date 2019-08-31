using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static CityHash.CityHash;

namespace Drive.ScsArchive {
    /// <summary>
    /// File name within the HashFs.
    /// </summary>
    public class HashFsFileName {
        /// <summary>
        /// True if the file name represents a directory.
        /// </summary>
        public bool IsDirectory { get; }

        /// <summary>
        /// Name of the file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="rawName">Raw name as returned by the HashFs reader</param>
        /// <param name="parentFolder">Path of the folder containing this file</param>
        public HashFsFileName (string rawName, string parentFolder = null) {
            IsDirectory = rawName.StartsWith("*");
            Name = (
                (string.IsNullOrEmpty(parentFolder) ? "" : parentFolder.Trim('/') + "/")
                + (IsDirectory ? rawName.Substring(1) : rawName)
                );
        }

        /// <summary>
        /// Return the hash for a full path.
        /// </summary>
        /// <param name="path">File path to hash</param>
        /// <returns>A hash usable to locate a file entry within the HashFs central directory</returns>
        public static ulong GetHashForFullPath (string path) => CityHash64(path, Encoding.UTF8);

        public static implicit operator string (HashFsFileName fName) => fName.Name;
    }
}
