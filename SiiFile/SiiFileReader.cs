using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;

namespace Drive.SiiFile {
    /// <summary>
    /// Sii file reader logic.
    /// </summary>
    public class SiiFileReader {
        /// <summary>
        /// Signature for text-based Sii units.
        /// </summary>
        public const string SII_TEXT_UNIT_SIGNATURE = "SiiNunit";

        /// <summary>
        /// Signature for encrypted Sii units.
        /// </summary>
        public const UInt32 SII_CRYPTO_UNIT_SIGNATURE = 0x43736353; // ScsC

        /// <summary>
        /// Provider for including external Sii files.
        /// </summary>
        private readonly IInclusionProvider _inclusionProvider;

        /// <summary>
        /// Initialize a new instance of this class.
        /// Instantiating an object using this constructor will disable inclusion. An exception will be thrown upon inclusion.
        /// </summary>
        public SiiFileReader () : this(null) { }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="inclusionProvider">The inclusion provider to use</param>
        public SiiFileReader (IInclusionProvider inclusionProvider) {
            _inclusionProvider = inclusionProvider;
        }

        /// <summary>
        /// Read a Sii text file.
        /// </summary>
        /// <param name="stream">Stream containing the sii text data</param>
        /// <returns>The root Sii object representing the unit</returns>
        public async Task<Object> ReadFromText (Stream stream) {
            using (StreamReader sr = new StreamReader(stream)) {
                string content = await sr.ReadToEndAsync();

                if (!content.StartsWith(SII_TEXT_UNIT_SIGNATURE)) // Not a valid text unit
                    throw new InvalidSiiFileException();

                content = SiiTextParser.SanitizeSource(content);
                string[] lines = SiiTextParser.SplitSourceToLines(content).ToArray();
                lines = (await ProcessIncludes(lines)).ToArray();

                return ObjectDeserializer.ParseObject(lines, 0, lines.Length);
            }
        }

        /// <summary>
        /// Process all the <code>@include</code> directives in the file.
        /// </summary>
        /// <param name="lines">Source file lines</param>
        /// <returns>An enumerable object containing the lines of the source code after the inclusions</returns>
        private async Task<IEnumerable<string>> ProcessIncludes (string[] lines) {
            LinkedList<string> result = new LinkedList<string>();

            foreach (string line in lines) {
                if (line.StartsWith("@include")) {
                    string includeSrc = await ProcessSingleInclude(line);
                    includeSrc = SiiTextParser.SanitizeSource(includeSrc);

                    foreach (string includeLine in SiiTextParser.SplitSourceToLines(includeSrc)) {
                        result.AddLast(includeLine);
                    }
                } else result.AddLast(line);
            }

            return result;
        }

        /// <summary>
        /// Process a single include directive.
        /// </summary>
        /// <param name="line">The raw line containing the include directive</param>
        /// <returns>The content to substitute to the directive</returns>
        private async Task<string> ProcessSingleInclude (string line) {
            int pathStart = line.IndexOf('"') + 1;
            string includePath = line.Substring(pathStart, line.Length - pathStart - 1);

            if (_inclusionProvider == null)
                throw new InvalidOperationException("Can't include external files without an inclusion provider");

            using (Stream includeStream = _inclusionProvider.IncludeFile(includePath)) {
                using (StreamReader sr = new StreamReader(includeStream)) {
                    return await sr.ReadToEndAsync();
                }
            }
        }

        /// <summary>
        /// Read a Sii binary file.
        /// </summary>
        /// <param name="stream">Stream containing the sii binary data</param>
        /// <returns>The root Sii object representing the unit</returns>
        public async Task<Object> ReadFromBinary (Stream stream) {
            SiiHeader header = SiiHeader.FromStream(stream);
            using (MemoryStream plainStream = new MemoryStream()) {
                if (header.Signature != SII_CRYPTO_UNIT_SIGNATURE) // Not a valid encrypted unit
                    throw new InvalidSiiFileException();

                await SiiEncryption.Decrypt(stream, plainStream, header.InitializationVector);
                plainStream.Position = 0;
                byte[] plainSig = new byte[SII_TEXT_UNIT_SIGNATURE.Length];
                byte[] txtSig = Encoding.UTF8.GetBytes(SII_TEXT_UNIT_SIGNATURE);
                bool compressed = false;
                
                plainStream.Read(plainSig, 0, plainSig.Length);
                plainStream.Position = 0;

                for (int i = 0; i < txtSig.Length; i++)
                    if (txtSig[i] != plainSig[i]) {
                        compressed = true;
                        break;
                    }

                Stream decompressedStream;

                if (compressed) {
                    decompressedStream = new MemoryStream();

                    await SiiCompression.Decompress(plainStream, decompressedStream);
                    decompressedStream.Position = 0;
                } else {
                    decompressedStream = plainStream;
                }


                return await ReadFromText(decompressedStream);
            }
        }

        /// <summary>
        /// Read a Sii file regardless of format.
        /// </summary>
        /// <param name="stream">Stream containing the sii data</param>
        /// <returns>The root Sii object representing the unit</returns>
        public async Task<Object> ReadSiiFile (Stream stream) {
            using (MemoryStream ms = new MemoryStream()) {
                UInt32 sig;

                await stream.CopyToAsync(ms);
                ms.Position = 0;

                using (BinaryReader br = new BinaryReader(ms, Encoding.UTF8, true)) {
                    sig = br.ReadUInt32();
                }

                ms.Position = 0;

                if (sig == SII_CRYPTO_UNIT_SIGNATURE)
                    return await ReadFromBinary(ms);
                else
                    return await ReadFromText(ms);
            }
        }
    }
}
