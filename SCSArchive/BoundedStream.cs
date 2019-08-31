using System;
using System.IO;

namespace Drive.ScsArchive {
    /// <summary>
    /// Stream bounded to part of an other stream.
    /// Intended for usage with file streams only and uniquely in read mode.
    /// </summary>
    public class BoundedStream : Stream, IDisposable {
        /// <summary>
        /// The underlying base stream.
        /// </summary>
        public Stream BaseStream { get; }

        /// <summary>
        /// The lower bound of this stream.
        /// </summary>
        public long LowerBound { get; }

        /// <summary>
        /// The upper bound of this stream.
        /// </summary>
        public long UpperBound { get; }

        /// <summary>
        /// True to leave open upon disposal.
        /// </summary>
        private readonly bool _leaveOpen;

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => UpperBound - LowerBound;

        public override long Position {
            get => BaseStream.Position - LowerBound;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Initialize a new instance of this class.
        /// </summary>
        /// <param name="stream">The base stream</param>
        /// <param name="lowerBound">The lower bound</param>
        /// <param name="upperBound">The upper bound</param>
        /// <param name="leaveOpen">True not to close the base stream upon disposal</param>
        public BoundedStream (Stream stream, long lowerBound, long upperBound, bool leaveOpen = false) {
            BaseStream = stream;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            _leaveOpen = leaveOpen;

            BaseStream.Position = lowerBound;
        }

        public new void Dispose () {
            if (!_leaveOpen)
                BaseStream.Dispose();
        }

        public override void Flush () { }

        public override int Read (byte[] buffer, int offset, int count) {
            int realCount = count;

            if (Position + count > Length)
                realCount = (int)(Length - Position);

            return BaseStream.Read(buffer, offset, realCount);
        }

        public override long Seek (long offset, SeekOrigin origin) {
            long absOffset;

            switch (origin) {
                case SeekOrigin.Begin:
                    absOffset = offset + LowerBound;
                    break;

                case SeekOrigin.End:
                    absOffset = UpperBound - offset;
                    break;

                case SeekOrigin.Current:
                    absOffset = BaseStream.Position + offset;
                    break;

                default:
                    throw new IOException(); // Should never happen
            }

            if (absOffset < LowerBound || absOffset > UpperBound)
                throw new IOException("Invalid offset");

            return BaseStream.Seek(absOffset, SeekOrigin.Begin) - LowerBound;
        }

        public override void SetLength (long value) => throw new InvalidOperationException();
        public override void Write (byte[] buffer, int offset, int count) => throw new InvalidOperationException();
    }
}
