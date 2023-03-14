using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JP.DataHub.Com.Extensions
{
    public static class PathExtensions
    {
        public static string ToFileName(this string str) => System.IO.Path.GetFileName(str);
        public static string ToPathName(this string str) => System.IO.Path.GetDirectoryName(str);
        public static string ToExecutingAssemblyPathOnFileName(this string str) => Assembly.GetExecutingAssembly()?.Location?.ToPathName() == null ? str : System.IO.Path.Combine(Assembly.GetExecutingAssembly().Location.ToPathName(), str);
        //public static string ToLocalUserAppDataPathOnFileName(this string str) => str == null ? null : System.IO.Path.Combine(System.Windows.Forms.Application.LocalUserAppDataPath, str);
    }
}
