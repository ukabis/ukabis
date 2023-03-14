using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JP.DataHub.Com.Configuration;

namespace JP.DataHub.Com.Extensions
{
    public static class StreamExtensions
    {
        public static Stream ToStream(this string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                return new MemoryStream(Encoding.UTF8.GetBytes(str));
            }
            else
            {
                return new MemoryStream(encoding.GetBytes(str));
            }
        }

        public static string ReadToEnd(this Stream stream)
        {
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            string result = null;
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                result = reader.ReadToEndAsync().Result;
                stream.Seek(0, SeekOrigin.Begin);
            }
            return result;
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            using MemoryStream ms = new();
            if (stream != null)
            {
                stream.CopyTo(ms);
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
            }
            return ms.ToArray();
        }

    }
}
