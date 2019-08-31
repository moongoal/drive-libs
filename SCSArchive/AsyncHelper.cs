using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive.ScsArchive {
    /// <summary>
    /// Helper class for async code.
    /// </summary>
    static class AsyncHelper {
        /// <summary>
        /// Fill the buffer from 0 to its length.
        /// </summary>
        /// <param name="buff">The buffer to be filled</param>
        public static async Task FillBuffer (Stream stream, byte[] buff) {
            int byteRead = 0;

            while (byteRead != buff.Length) {
                int chunkSz = await stream.ReadAsync(buff, byteRead, buff.Length - byteRead);

                if (chunkSz == 0)
                    throw new IOException();

                byteRead += chunkSz;
            }
        }
    }
}
