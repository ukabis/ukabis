using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    internal interface IBlockchainEventHubStoreRepository
    {
        /// <summary>
        /// EventHubにリソースの登録を通知します。
        /// </summary>
        /// <param name="id">登録データのid</param>
        /// <param name="data">登録データ</param>
        /// <param name="repositoryType"></param>
        /// <param name="versionKey"></param>
        /// <returns>通知結果</returns>
        bool Register(string id, JToken data, RepositoryType repositoryType, string versionKey = null);

        /// <summary>
        /// EventHubにリソースの削除を通知します。
        /// </summary>
        /// <param name="id">登録データのid</param>
        /// <param name="repositoryType"></param>
        /// <param name="versionKey"></param>
        /// <returns>通知結果</returns>
        bool Delete(string id, RepositoryType repositoryType, string versionKey = null);
    }
}