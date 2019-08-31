using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    /// <summary>
    /// This class is meant to simplify opening files spread across the different *.scs (and in future *.zip) files.
    /// This is mostly useful in the context of <code>@include</code> directives within *.sii files that include files from a possibly different archive.
    /// </summary>
    public class GameFs {
        /// <summary>
        /// Ordered archive base names.
        /// </summary>
        private static readonly string[] _archiveFileNames = new string[] { // Not sure this is important; might be removed in favour of globbing in the future
            "effect",
            "base",
            "core",
            "def",
            "locale",
            "base_cfg"
        };

        /// <summary>
        /// Mapping between path and ScsArchive.
        /// </summary>
        private readonly Dictionary<string, ScsArchive> _archives = new Dictionary<string, ScsArchive>();

        /// <summary>
        /// Game install path.
        /// </summary>
        private readonly string _installPath;

        /// <summary>
        /// Mod folder path.
        /// </summary>
        private readonly string _modPath;

        /// <summary>
        /// Enumerable containing all the loaded archives.
        /// </summary>
        public IEnumerable<ScsArchive> Archives => _archives.Values;

        /// <summary>
        /// Enumerable containing all the loaded archive paths.
        /// </summary>
        public IEnumerable<string> ArchivePaths => _archives.Keys;

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="gameInstallPath">The game installation path</param>
        /// <param name="modInstallPath">The game mod path</param>
        public GameFs (string gameInstallPath, string modInstallPath) {
            _installPath = gameInstallPath;
            _modPath = modInstallPath;
        }

        /// <summary>
        /// Initialize the class.
        /// This method must be manually called after instantiation.
        /// </summary>
        /// <returns></returns>
        public async Task Initialize () { // TODO: Add support for mod archives
            // Base game
            foreach (string fName in _archiveFileNames) {
                string arcPath = Path.Combine(_installPath, fName + ".scs");

                if (File.Exists(arcPath))
                    _archives.Add(arcPath, await LoadArchive(arcPath));
            }

            // DLCs
            foreach (string fPath in Directory.EnumerateFiles(_installPath)) {
                string fName = Path.GetFileName(fPath);

                if (fName.StartsWith("dlc_", StringComparison.InvariantCultureIgnoreCase) && fName.EndsWith(".scs", StringComparison.InvariantCultureIgnoreCase)) {
                    _archives.Add(fPath, await LoadArchive(fPath));
                }
            }
        }

        /// <summary>
        /// Open a file.
        /// </summary>
        /// <param name="path">The path of the file to open</param>
        /// <returns>A stream serving the opened file</returns>
        public Stream OpenFile (string path) {
            ulong hash = HashFsFileName.GetHashForFullPath(path);

            foreach (ScsArchive arc in Archives) {
                if (arc.HasEntry(hash))
                    return new HashFsEntryReader(arc.GetEntry(hash)).GetStream();
            }

            throw new FileNotFoundException(path);
        }

        /// <summary>
        /// Load and return an *.scs archive.
        /// </summary>
        /// <param name="path">The path of the archive</param>
        /// <returns>The loaded archive</returns>
        private async Task<ScsArchive> LoadArchive (string path) {
            return await new ScsArchiveReader(path).ReadArchive();
        }

        /// <summary>
        /// List the contents of a folder.
        /// </summary>
        /// <param name="path">The path of the folder to list</param>
        /// <returns>An enumerable of full paths representing the contents of the folder</returns>
        public async Task<IEnumerable<string>> ListFolder (string path) {
            IEnumerable<string> contents = new LinkedList<string>();
            ulong pathHash = HashFsFileName.GetHashForFullPath(path);

            foreach (ScsArchive arc in _archives.Values) {
                if (arc.HasEntry(pathHash)) {
                    contents = contents.Union(
                        (await new HashFsEntryReader(arc.GetEntry(pathHash)).ListFiles(path))
                        .Select((HashFsFileName x) => x.Name));
                }
            }

            return contents;
        }

        /// <summary>
        /// Get an include provider for this instance.
        /// </summary>
        /// <param name="basePath">The path to use as CWD during the inclusion process</param>
        /// <returns>A new include provider object</returns>
        public GameFsIncludeProvider GetIncludeProvider (string basePath = null) {
            return new GameFsIncludeProvider(this, basePath);
        }
    }
}
