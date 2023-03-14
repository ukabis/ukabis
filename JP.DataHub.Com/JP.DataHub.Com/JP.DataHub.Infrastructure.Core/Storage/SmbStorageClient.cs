using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Storage
{
    public class SmbStorageClient : IStorageClient
    {
        private string RootPath;
        private const int BUFFER_SIZE = 1024 * 4;

        public SmbStorageClient(string rootPath)
        {
            RootPath = rootPath;
        }

        public void CopyTo(string objectName, Stream source)
        {
            var filename = Path.Combine(RootPath, objectName);
            var dir = Path.GetDirectoryName(filename);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            using (var dst = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var buffer = new byte[BUFFER_SIZE];
                for (; ; )
                {
                    int readbyte = source?.ReadAsync(buffer, 0, BUFFER_SIZE).Result ?? 0;
                    if (readbyte == 0)
                    {
                        break;
                    }
                    dst.Write(buffer, 0, readbyte);
                }
            }
        }

        public Task CopyToAsync(string objectName, Stream createObject)
        {
            throw new NotImplementedException();
        }

        public Stream GetStream(string objectName)
        {
            var filename = Path.Combine(RootPath, objectName);
            if (File.Exists(filename) == false)
            {
                return null;
            }
            return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public void Delete(string objectName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string objectName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> List(string prefix)
        {
            yield break;
        }

        public bool Exist(string objectName) => File.Exists(Path.Combine(RootPath, objectName));

        public long GetSize(string objectName)
        {
            var filename = Path.Combine(RootPath, objectName);
            return Exist(filename) == false ? 0 : (long)new FileInfo(filename)?.Length;
        }

        public string GetObjectPath(string objectName)
        {
            throw new NotImplementedException();
        }
    }
}
