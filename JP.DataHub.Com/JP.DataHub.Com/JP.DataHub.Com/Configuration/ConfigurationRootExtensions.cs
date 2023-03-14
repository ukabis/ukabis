using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Configuration
{
    public static class ConfigurationRootExtensions
    {
        public static string FindFile(this IConfigurationRoot root)
        {
            bool optional = true;
            for (int i = 0; i <= 1; i++)
            {
                foreach (var c in root.Providers)
                {
                    var source = c.GetPropertyValue("Source");
                    var sourceOptional = source.GetPropertyValue<bool>("Optional");
                    if (sourceOptional == optional)
                    {
                        var path = source.GetPropertyValue("FileProvider").GetPropertyValue<string>("Root");
                        return Path.Combine(path, source.GetPropertyValue<string>("Path"));
                    }
                }
                optional = !optional;
            }
            return null;
        }
    }
}
