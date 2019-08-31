using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    /// <summary>
    /// Scs archive reader.
    /// </summary>
    public sealed class ScsArchiveReader : IDisposable {
        /// <summary>
        /// File header.
        /// </summary
        private readonly ScsArchiveHeader _header = new ScsArchiveHeader();

        /// <summary>
        /// File signature.
        /// </summary>
        private readonly byte[] SCS_ARCHIVE_MAGIC = new byte[] { 0x53, 0x43, 0x53, 0x23 };

        /// <summary>
        /// _header size.
        /// </summary>
        private const int SCS_HEADER_SIZE = 32; // Just guessing here :)

        /// <summary>
        /// Archive path.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// True if initialized.
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// File reader.
        /// </summary>
        private BufferedStream _streamReader;

        /// <summary>
        /// File reader.
        /// </summary>
        public BufferedStream Stream {
            get => _streamReader ?? (_streamReader = new BufferedStream(new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read)));
            set => _streamReader = value;
        }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="path">Path to the archive file</param>
        public ScsArchiveReader (string path) {
            _path = path;
        }

        public void Dispose () {
            _streamReader?.Dispose();
        }

        /// <summary>
        /// Read the Scs archive header.
        /// </summary>
        private async Task ReadHeader () {
            byte[] head = new byte[SCS_HEADER_SIZE];

            await AsyncHelper.FillBuffer(Stream, head);

            using (MemoryStream msHead = new MemoryStream(head)) {
                using (BinaryReader brHead = new BinaryReader(msHead)) {
                    if (!ValidateSignature(brHead.ReadBytes(SCS_ARCHIVE_MAGIC.Length)))
                        throw new InvalidScsArchiveException(_path);

                    _header.Version = brHead.ReadUInt32(); // Is this really the version number?
                    _header.HashingAlgorithm = Encoding.UTF8.GetString(brHead.ReadBytes(4));
                    _header.EntryCount = brHead.ReadUInt32();
                    _header.CentralDirectoryAddress = brHead.ReadUInt32();
                }
            }
        }

        /// <summary>
        /// Get the file header.
        /// </summary>
        /// <returns>The Scs file header</returns>
        public async Task<ScsArchiveHeader> GetHeader() {
            await Initialize();

            return _header;
        }

        /// <summary>
        /// Validate the file signature.
        /// </summary>
        /// <param name="data">File signature</param>
        /// <returns>True if the signature is valid</returns>
        private bool ValidateSignature (byte[] data) {
            for (int i = 0; i < SCS_ARCHIVE_MAGIC.Length; i++)
                if (data[i] != SCS_ARCHIVE_MAGIC[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Initialize the reader.
        /// This method must be called after instantiation, before usage of the object.
        /// </summary>
        /// <returns></returns>
        private async Task Initialize () {
            if (!_initialized) {
                await ReadHeader();
                _initialized = true;
            }
        }

        /// <summary>
        /// Read and return the Scs archive.
        /// </summary>
        /// <returns>An ScsArchive instance</returns>
        public async Task<ScsArchive> ReadArchive () => new ScsArchive(await GetEntries());

        /// <summary>
        /// Read and return the entries from the HashFs central directory.
        /// </summary>
        /// <returns>A mapping between hash and HashFs entry</returns>
        public async Task<IReadOnlyDictionary<ulong, HashFsEntry>> GetEntries () {
            await Initialize();

            const int DIR_ENTRY_SIZE = 32;
            byte[] buff = new byte[DIR_ENTRY_SIZE * _header.EntryCount];
            Dictionary<ulong, HashFsEntry> entries = new Dictionary<ulong, HashFsEntry>();

            Stream.Position = _header.CentralDirectoryAddress;
            await AsyncHelper.FillBuffer(Stream, buff);

            using (MemoryStream ms = new MemoryStream(buff)) {
                using (BinaryReader br = new BinaryReader(ms)) {
                    for (uint i = 0; i < _header.EntryCount; i++) {
                        (ulong hash, HashFsEntry entry) = ReadNextEntry(br);

                        entries.Add(hash, entry);
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Read the next HashFs entry from the central directory.
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <returns>A tuple containing the hash of the entry and the allocated entry object</returns>
        private (ulong hash, HashFsEntry entry) ReadNextEntry (BinaryReader br) {
            HashFsEntry dirEntry = new HashFsEntry();
            ulong hash = br.ReadUInt64();

            dirEntry.Offset = br.ReadUInt32();
            br.ReadUInt32(); // Rsvd
            dirEntry.Type = (HashFsEntryType)(br.ReadUInt32() % 4);
            dirEntry.Crc32 = br.ReadUInt32();
            dirEntry.UncompressedSize = br.ReadUInt32();
            dirEntry.CompressedSize = br.ReadUInt32();
            dirEntry.ArchivePath = _path;

            return (hash, dirEntry);
        }
    }
}
