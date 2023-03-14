using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Storage
{
    public interface IStorageClient
    {
        void CopyTo(string objectName, Stream createObject);
        Task CopyToAsync(string objectName, Stream createObject);
        Stream GetStream(string objectName);
        void Delete(string objectName);
        Task DeleteAsync(string objectName);
        IEnumerable<string> List(string prefix);
        bool Exist(string objectName);
        long GetSize(string objectName);
        string GetObjectPath(string objectName);
    }
}
