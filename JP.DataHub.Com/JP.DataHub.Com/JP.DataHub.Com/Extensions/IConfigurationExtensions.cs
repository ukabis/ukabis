using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace JP.DataHub.Com.Extensions
{
    public static class IConfigurationExtensions
    {
        public static IList<T> GetValue<T>(this IConfigurationSection section)
        {
            var result = new List<T>();
            foreach (var item in section.GetChildren())
            {
                result.Add(item.Value.To<T>());
            }
            return result;
        }
    }
}
