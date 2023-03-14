using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache
{
    public interface ICacheManager
    {
        bool IsEnable { get; set; }
        bool IsEnableFire { get; set; }

        void FireId(string keyName, string val);
        Task FireIdAsync(string keyName, string val);

        void FireEntity(string entityName);
        void FireEntity(params string[] entities);
        Task FireEntityAsync(string entityName);
        Task FireEntityAsync(IEnumerable<string> entityName);

        void FireKey(string keyName);
        void FireKey(params string[] keyName);
        Task FireKeyAsync(string keyName);
        Task FireKeyAsync(IEnumerable<string> keyName);

        void FireId(ICache cache,string keyName, string val);
        Task FireIdAsync(ICache cache, string keyName, string val);

        void FireEntity(ICache cache, string entityName);
        void FireEntity(ICache cache, params string[] entities);
        Task FireEntityAsync(ICache cache, string entityName);
        Task FireEntityAsync(ICache cache, IEnumerable<string> entityName);

        void FireKey(ICache cache, string keyName);
        void FireKey(ICache cache, params string[] keyName);
        Task FireKeyAsync(ICache cache, string keyName);
        Task FireKeyAsync(ICache cache, IEnumerable<string> keyName);
    }
}
