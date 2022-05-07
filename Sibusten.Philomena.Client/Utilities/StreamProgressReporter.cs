using System;
using System.IO;

namespace Sibusten.Philomena.Client.Utilities
{
    public struct StreamProgressInfo
    {
        public long BytesRead { get; set; }
        public long? BytesTotal { get; set; }
    }

    public class StreamProgressReporter : Stream
    {
        private Stream _sourceStream;
        private long _currentPosition = 0;
        private long? _reportedLength;
        private IProgress<StreamProgressInfo>? _progress;

        public StreamProgressReporter(Stream sourceStream, IProgress<StreamProgressInfo>? progress, long? length = null)
        {
            _sourceStream = sourceStream;
            _progress = progress;
            _reportedLength = length;
        }

        public override bool CanRead => _sourceStream.CanRead;

        public override bool CanSeek => _sourceStream.CanSeek;

        public override bool CanWrite => _sourceStream.CanWrite;

        public override long Length => _sourceStream.Length;

        public override long Position { get => _sourceStream.Position; set => _sourceStream.Position = value; }

        public override void Flush()
        {
            _sourceStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _sourceStream.Read(buffer, offset, count);
            _currentPosition += bytesRead;

            _progress?.Report(new StreamProgressInfo()
            {
                BytesRead = _currentPosition,
                BytesTotal = CanSeek ? Length : _reportedLength,  // Use the stream length if seeking is possible, otherwise fall back to a provided length
            });

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _currentPosition = _sourceStream.Seek(offset, origin);

            _progress?.Report(new StreamProgressInfo()
            {
                BytesRead = _currentPosition,
                BytesTotal = Length,
            });

            return _currentPosition;
        }

        public override void SetLength(long value)
        {
            _sourceStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _sourceStream.Write(buffer, offset, count);
        }
    }
}
