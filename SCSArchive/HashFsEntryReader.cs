using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    /// <summary>
    /// HashFS entry reader.
    /// This class provides access to the data of a HashFS entry.
    /// </summary>
    public class HashFsEntryReader {
        /// <summary>
        /// The HashFS entry to read.
        /// </summary>
        public HashFsEntry Entry { get; }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="entry">The HashFS entry to read</param>
        public HashFsEntryReader (HashFsEntry entry) {
            Entry = entry;
        }

        /// <summary>
        /// Get the list of filenames for a directory.
        /// </summary>
        /// <param name="baseDir">Path to use as base for the listed files.</param>
        /// <returns>Enumerable of HashFs file names</returns>
        public async Task<IEnumerable<HashFsFileName>> ListFiles (string baseDir = null) {
            if (!Entry.IsDirectory)
                throw new InvalidArchiveEntryException("Not a directory entry");

            List<HashFsFileName> fileNames = new List<HashFsFileName>(); // Directory Entry names

            using (Stream EntryStream = GetStream()) {
                byte[] buff = new byte[Entry.UncompressedSize];

                await AsyncHelper.FillBuffer(EntryStream, buff);

                using (MemoryStream ms = new MemoryStream(buff)) {
                    string curFileName;

                    using (StreamReader sr = new StreamReader(ms)) {
                        while ((curFileName = await sr.ReadLineAsync()) != null) {
                            fileNames.Add(new HashFsFileName(curFileName, baseDir));
                        }
                    }
                }
            }

            return fileNames;
        }

        /// <summary>
        /// Get a stream for the entry.
        /// </summary>
        /// <returns>A readable stream serving the entry data</returns>
        public Stream GetStream () {
            Stream fs = new BoundedStream(
                new FileStream(Entry.ArchivePath, FileMode.Open, FileAccess.Read, FileShare.Read),
                Entry.Offset,
                Entry.Offset + (Entry.IsCompressed ? Entry.CompressedSize : Entry.UncompressedSize)
            );

            if (Entry.IsCompressed)
                fs = new GZipStream(fs, CompressionMode.Decompress);

            return fs;
        }

        /// <summary>
        /// Get the content for an entry.
        /// </summary>
        /// <returns>The content of the entry as a byte array</returns>
        public async Task<byte[]> GetContent () {
            Stream entryStream = GetStream();
            byte[] buff = new byte[Entry.UncompressedSize];

            await AsyncHelper.FillBuffer(entryStream, buff);

            return buff;
        }

        /// <summary>
        /// Get the content for an entry
        /// </summary>
        /// <param name="encoding">Encoding to use</param>
        /// <returns>The content of the entry as a string</returns>
        public async Task<string> GetContent (Encoding encoding) {
            if (encoding == null)
                encoding = Encoding.Default;

            byte[] contents = await GetContent();

            return encoding.GetString(contents);
        }
    }
}
