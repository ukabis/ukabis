using System;
using System.IO;

namespace JP.DataHub.Com.IO
{
    public class LoggingStream : Stream, IDisposable
    {
        private readonly Stream Stream;
        private readonly Stream SaveStream;
        private readonly Func<long, Stream, long> CallBackFunc;
        private long Size = 0;
        private bool IsSendEvent = false;
        private readonly bool IsSaveStream;
        /// <summary>
        /// Loggingを行うためのStream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="callback">Close時に実行されるFunc</param>
        /// <param name="isSaveStream">読み込んだStreamを保存するか</param>
        public LoggingStream(Stream stream, Func<long, Stream, long> callback = null, bool isSaveStream = true)
        {
            Stream = stream;
            IsSaveStream = isSaveStream;
            SaveStream = new MemoryStream();
            CallBackFunc = callback;
        }
        public override bool CanRead { get { return (Stream != null) ? Stream.CanRead : false; } }
        public override bool CanSeek { get { return (Stream != null) ? Stream.CanSeek : false; } }
        public override bool CanWrite { get { return (Stream != null) ? Stream.CanWrite : false; } }
        public override void Flush() { Stream.Flush(); }
        public override long Length { get { return (Stream != null) ? Stream.Length : 0; } }
        public override long Position { get { return Stream.Position; } set { Stream.Position = value; } }
        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = Stream.Read(buffer, offset, count);
            if (IsSaveStream)
            {
                SaveStream.Write(buffer, offset, result);
            }
            Size += result;
            return result;
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return Stream.Seek(offset, origin);
        }
        public override void SetLength(long value) { Stream.SetLength(value); }
        public override void Write(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
        }

        private void ExcuteFunc()
        {
            try
            {
                if (IsSendEvent) return;
                IsSendEvent = true;
                if (CallBackFunc == null) return;
                SaveStream.Seek(0, SeekOrigin.Begin);
                CallBackFunc(Size, SaveStream);
            }
            finally
            {
                Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsSendEvent)
            {
                ExcuteFunc();
            }
            if (Stream != null)
            {
                Stream.Dispose();
            }
            if (SaveStream != null)
            {
                SaveStream.Dispose();
            }
        }

        ~LoggingStream()
        {
            Dispose(false);
        }
    }
}
