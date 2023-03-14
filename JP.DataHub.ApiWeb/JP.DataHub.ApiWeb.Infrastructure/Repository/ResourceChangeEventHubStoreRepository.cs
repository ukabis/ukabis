using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Data.EventHub;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    /// <summary>
    /// リソース変更通知用EventHubリポジトリ
    /// </summary>
    internal class ResourceChangeEventHubStoreRepository : IResourceChangeEventHubStoreRepository
    {
#if Oracle
        private readonly IEventHubStreamingService _eventHub;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="eventHub">イベントハブ通知処理</param>
        public ResourceChangeEventHubStoreRepository(IEventHubStreamingService eventHub)
        {
            _eventHub = eventHub;
        }
#else
        private readonly IJPDataHubEventHub _eventHub;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="eventHub">イベントハブ通知処理</param>
        public ResourceChangeEventHubStoreRepository(IJPDataHubEventHub eventHub)
        {
            _eventHub = eventHub;
        }
#endif

        /// <summary>
        /// EventHubにリソースの登録を通知します。
        /// </summary>
        /// <param name="action">DynamicApiのアクション</param>
        /// <param name="data">登録データ</param>
        /// <returns>通知結果</returns>
        public bool Register(IDynamicApiAction action, JToken data)
            => SendMessage(action, data);

        /// <summary>
        /// EventHubにリソースの更新を通知します。
        /// </summary>
        /// <param name="action">DynamicApiのアクション</param>
        /// <param name="data">登録データ</param>
        /// <param name="input">入力データ</param>
        /// <returns>通知結果</returns>
        public bool Update(IDynamicApiAction action, JToken data, JToken input)
            => SendMessage(action, data, input);

        /// <summary>
        /// EventHubにリソースの削除を通知します。
        /// </summary>
        /// <param name="action">DynamicApiのアクション</param>
        /// <returns>通知結果</returns>
        public bool Delete(IDynamicApiAction action)
        {
            // クエリーが設定されている場合は処理対象外
            if (!string.IsNullOrEmpty(action.ApiQuery?.Value))
                throw new InvalidOperationException("API has Query.");

            return SendMessage(action, null);
        }

        /// <summary>
        /// EventHubに通知します。
        /// </summary>
        /// <param name="action">DynamicApiのアクション</param>
        /// <param name="data">通知する情報</param>
        /// <param name="input">入力データ</param>
        /// <returns>通知結果</returns>
        private bool SendMessage(IDynamicApiAction action, JToken data, JToken input = null)
        {
            var connectionString = UnityCore.Resolve<IConfiguration>().GetValue<string>("ConnectionStrings:ResourceChangeEventHub");
            if (connectionString == null)
            {
                return false;
            }

            // 通知データ作成
            var sendData = new JObject
            {
                ["Data"] = data,
                ["RequestInfo"] = JObject.FromObject(new RequestInfo(action))
            };

            if (action.Query != null && action.Query.Dic.Count > 0)
                sendData["Query"] = JObject.FromObject(action.Query.Dic.ToDictionary(q => q.Key.Value, q => q.Value.Value));
            if (input != null) sendData["Input"] = input;

            _eventHub.ConnectionString = connectionString;
            var prtition = EventHubPartitionKey.CreateRegisterPartition(action.PartitionKey, action.IsVendor, action.VendorId, action.SystemId, data);
            return _eventHub.SendMessageAsync(sendData, prtition?.Value ?? "").Result;
        }

        /// <summary>
        /// EventHubに通知するリクエストの情報
        /// </summary>
        private class RequestInfo
        {
            public string ActionType { get; set; }
            public string VendorId { get; set; }
            public string SystemId { get; set; }
            public string OpenId { get; set; }
            public string ControllerId { get; set; }
            public string ApiId { get; set; }
            public string RepositoryKey { get; set; }

            /// <summary>
            /// インスタンスを初期化します。
            /// </summary>
            /// <param name="action">DynamicApiのアクション</param>
            public RequestInfo(IDynamicApiAction action)
            {
                ActionType = action.ActionType.Code;
                VendorId = action.VendorId.Value;
                SystemId = action.SystemId.Value;
                OpenId = action.OpenId.Value;
                ControllerId = action.ControllerId.Value;
                ApiId = action.ApiId.Value;
                RepositoryKey = action.RepositoryKey.Value;
            }
        }
    }
}
