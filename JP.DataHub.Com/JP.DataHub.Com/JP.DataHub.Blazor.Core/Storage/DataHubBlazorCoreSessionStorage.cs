using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using JP.DataHub.MVC.Storage;

namespace JP.DataHub.Blazor.Core.Storage
{
    /// <summary>
    /// ブラウザーのタブ単位のストレージ
    /// </summary>
    public class DataHubBlazorCoreSessionStorage : IStorage
    {

        public DataHubBlazorCoreSessionStorage()
        {
        }

        public async Task SetAsync(ProtectedSessionStorage storage, string key, object value)
        {
            await storage.SetAsync(key, value);
        }

        public async Task<StorageResult<TValue>> GetAsync<TValue>(ProtectedSessionStorage storage, string key)
        {
            try
            {
                var result = await storage.GetAsync<TValue>(key);
                return new StorageResult<TValue>(result.Success, result.Value);
            }
            // ブラウザ側にkey&Valueが残ってしまう場合があるため、例外発生時は対象のキーを削除する
            catch (Exception e)
            {
                try
                {
                    await storage.DeleteAsync(key);
                    var result = await storage.GetAsync<TValue>(key);
                    return new StorageResult<TValue>(result.Success, result.Value);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
