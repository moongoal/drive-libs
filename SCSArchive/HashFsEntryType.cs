using System;

namespace Drive.ScsArchive {
    /// <summary>
    /// HashFs entry type.
    /// </summary>
    public enum HashFsEntryType : UInt32 {
        UncompressedFile = 0,
        UncompressedDirectory = 1,
        CompressedFile = 2,
        CompressedDirectory = 3
    }
}
