using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Instance;

namespace JP.DataHub.Com.Extensions
{
    public static class FileExtensions
    {
        public static string ToContents(this string fileName, Encoding encoding = null) => ReadFileContents(fileName, encoding);

        public static string ReadFileContents(this string fileName, Encoding encoding = null)
        {
            if (File.Exists(fileName))
            {
                using (var sr = new StreamReader(fileName, encoding ?? Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
            return null;
        }

        public static string ToContents(this Stream stream, Encoding encoding = null) => StreamToContents(stream, encoding);

        public static string StreamToContents(this Stream stream, Encoding encoding = null)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static T StreamToJson<T>(this Stream stream, Encoding encoding = null) where T : new()
        {
            return StreamToContents(stream, encoding).ToJson<T>();
        }

        public static object FileToJson(this string fileName, Type type, Encoding encoding = null)
        {
            if (System.IO.File.Exists(fileName) == false)
            {
                return null;
            }
            object result = fileName.ReadFileContents(encoding).ToJson(type);
            if (result is IInstanceInitializer init)
            {
                init.InstanceInitializer();
            }
            return result;
        }

        public static T FileToJson<T>(this string fileName, Encoding encoding = null) where T : new()
        {
            if (System.IO.File.Exists(fileName) == false)
            {
                return default(T);
            }
            T result = fileName.ReadFileContents(encoding).ToJson<T>();
            if (result is IInstanceInitializer init)
            {
                init.InstanceInitializer();
            }
            return result;
        }

        public static void ToFile(this string str, string fileName, Encoding encoding = null)
        {
            using (var sw = new StreamWriter(fileName, false, encoding ?? Encoding.UTF8))
            {
                sw.Write(str);
            }
        }
    }
}
