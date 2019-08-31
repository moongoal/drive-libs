using System.IO;

namespace Drive.SiiFile {
    /// <summary>
    /// Inclusion facility to support <code>@include</code> directives within
    /// Sii files.
    /// </summary>
    public interface IInclusionProvider {
        /// <summary>
        /// Open a stream that will be used to include the given file into another Sii file.
        /// </summary>
        /// <param name="path">The path of the file to include</param>
        /// <returns>A stream serving the file to include</returns>
        Stream IncludeFile (string path);
    }
}
