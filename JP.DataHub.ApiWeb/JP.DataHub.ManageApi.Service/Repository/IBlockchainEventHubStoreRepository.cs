using JP.DataHub.ManageApi.Service.DymamicApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Repository
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
        bool Register(string id, JToken data, RepositoryInfo.RepositoryType repositoryType, string versionKey = null);

        /// <summary>
        /// EventHubにリソースの削除を通知します。
        /// </summary>
        /// <param name="id">登録データのid</param>
        /// <param name="repositoryType"></param>
        /// <param name="versionKey"></param>
        /// <returns>通知結果</returns>
        bool Delete(string id, RepositoryInfo.RepositoryType repositoryType, string versionKey = null);
    }
}
