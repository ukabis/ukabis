using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Instance;

namespace JP.DataHub.Com.Configuration
{
    public static class ConfigExtensions
    {
        public static T FileToJson<T>(this string fileName)
        {
            if (System.IO.File.Exists(fileName) == false)
            {
                return default(T);
            }
            var result = fileName.ReadFileContents().ToJson<T>();
            if (result is IInstanceInitializer init)
            {
                init.InstanceInitializer();
            }
            return result;
        }
    }
}
