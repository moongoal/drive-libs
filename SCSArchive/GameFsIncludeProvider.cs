using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    /// <summary>
    /// Implementation of <code>IInclusionProvider</code> for the <code>GameFs</code> class.
    /// <see cref="Drive.SiiFile.IInclusionProvider"/>
    /// </summary>
    public sealed class GameFsIncludeProvider : Drive.SiiFile.IInclusionProvider {
        /// <summary>
        /// The game filesystem used to include files.
        /// </summary>
        public GameFs GameFs { get; }

        /// <summary>
        /// The CWD used when including files.
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        /// Include a file.
        /// </summary>
        /// <param name="path">Path of the file to include</param>
        /// <returns>A stream for the included file</returns>
        public Stream IncludeFile (string path) {
            string fullPath;

            if (path[0] != '/')
                fullPath = ((BasePath != null) ? BasePath + "/" : string.Empty) + path;
            else
                fullPath = path.Substring(1);

            return GameFs.OpenFile(fullPath);
        }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="gameFs">Game filesystem instance to bind</param>
        /// <param name="basePath">Base path. Null to use the root folder.</param>
        internal GameFsIncludeProvider (GameFs gameFs, string basePath = null) {
            GameFs = gameFs;
            BasePath = basePath?.TrimEnd('/');
        }
    }
}
