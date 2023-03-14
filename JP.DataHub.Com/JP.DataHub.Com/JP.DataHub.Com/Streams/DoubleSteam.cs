using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JP.DataHub.Com.Streams
{
    public class DoubleSteam : Stream
    {
        private Stream first;
        private Stream second;

        public DoubleSteam(Stream first, Stream second)
        {
            this.first = first;
            this.second = second;
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return true; } }
        public override void Flush() { }
        public override long Length { get { return 0; } }
        public override long Position { get { return 0; } set { } }
        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = first.Read(buffer, offset, count);
            if (result != count && second != null)
            {
                result += second.Read(buffer, result, count - result);
            }
            return result;
        }
        public override long Seek(long offset, SeekOrigin origin) { return 0; }
        public override void SetLength(long value) { }
        public override void Write(byte[] buffer, int offset, int count)
        {
        }
    }
}
