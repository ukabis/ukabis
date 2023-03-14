using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace JP.DataHub.MVC.Storage
{
    public interface IStorage
    {
        Task SetAsync(ProtectedSessionStorage storage, string key, object value);
        Task<StorageResult<TValue>> GetAsync<TValue>(ProtectedSessionStorage storage, string key);
    }
}
