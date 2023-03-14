using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    /// <summary>
    /// リソース変更通知用EventHubリポジトリのインターフェースです。
    /// </summary>
    internal interface IResourceChangeEventHubStoreRepository
    {
        /// <summary>
        /// EventHubにリソースの登録を通知します。
        /// </summary>
        /// <param name="action">DynamicApiのアクション</param>
        /// <param name="data">登録データ</param>
        /// <returns>通知結果</returns>
        bool Register(IDynamicApiAction action, JToken data);

        /// <summary>
        /// EventHubにリソースの更新を通知します。
        /// </summary>
        /// <param name="action">DynamicApiのアクション</param>
        /// <param name="data">登録データ</param>
        /// <param name="input">入力データ</param>
        /// <returns>通知結果</returns>
        bool Update(IDynamicApiAction action, JToken data, JToken input);

        /// <summary>
        /// EventHubにリソースの削除を通知します。
        /// </summary>
        /// <param name="action">DynamicApiのアクション</param>
        /// <returns>通知結果</returns>
        bool Delete(IDynamicApiAction action);
    }
}
