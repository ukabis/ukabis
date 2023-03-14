using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Aop
{
    // .NET6
    public interface IAopCacheHelper
    {
        void Add(string key, object value);
        void Add(string key, object value, TimeSpan expiration);
        T Get<T>(string key);
        T GetOrAdd<T>(string key, Func<T> misshitAction);
        T GetOrAdd<T>(string key, TimeSpan expiration, Func<T> misshitAction);
        void Remove(string key);
    }
}
