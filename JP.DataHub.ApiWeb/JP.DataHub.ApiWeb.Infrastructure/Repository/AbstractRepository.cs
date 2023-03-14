using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    /// <summary>
    /// Abstract Repository.
    /// </summary>
    internal class AbstractRepository
    {
        private readonly Lazy<IPerRequestDataContainer> _requestDataContainer = new Lazy<IPerRequestDataContainer>(
            () =>
            {
                try
                {
                    return UnityCore.Resolve<IPerRequestDataContainer>();
                }
                catch
                {
                    // ignored
                    return UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                }
            });

        protected void MakeResponseInternalInfo(string query, Dictionary<string, object> param)
        {
            //string sql = query;
            //foreach (var key in param.Keys)
            //{
            //    sql = sql.Replace($"@{key}", $"'{param[key]?.ToString()}'");
            //}
            //int cnt = this.PerRequestDataContainer.RepositoryInfo.ContainsKey("X-RepositoryAccessCount") ? int.Parse(PerRequestDataContainer.RepositoryInfo["X-RepositoryAccessCount"]) + 1 : 1;
            //this.PerRequestDataContainer.RepositoryInfo.Add($"X-Repository-{cnt}", GetType().Name);
            //this.PerRequestDataContainer.RepositoryInfo.Add($"X-Query-{cnt}", sql);
            //this.PerRequestDataContainer.RepositoryInfo.Remove("X-RepositoryAccessCount");
            //this.PerRequestDataContainer.RepositoryInfo.Add("X-RepositoryAccessCount", cnt.ToString());
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbstractRepository() { }

        /// <summary>
        /// PerRequestDataContainer
        /// </summary>
        protected IPerRequestDataContainer PerRequestDataContainer => _requestDataContainer.Value;

        /// <summary>
        /// ユーザが認証済みで、プロパティ _UserId/UserName が有効であるかどうか。
        /// </summary>
        public bool IsUserAuthenticated =>
            _requestDataContainer.Value.IsUserAuthenticated;

        /// <summary>
        /// OpenIdを取得します。
        /// </summary>
        public string OpenId => _requestDataContainer.Value.OpenId;

        /// <summary>
        /// ユーザーの名前(First+Middle+Family)を取得します。
        /// このプロパティを利用するには、ユーザが認証状態である必要があります。
        /// </summary>
        [Obsolete("不要っぽいので。必要ならObsolete外してもいいですよ。", true)]
        public string UserName => _requestDataContainer.Value.UserName;


        /// <summary>
        /// ユーザーのAccount名を取得します。
        /// このプロパティを利用するには、ユーザが認証状態である必要があります。
        /// </summary>
        public string UserAccount => _requestDataContainer.Value.UserAccount;

        /// <summary>
        /// ユーザーのベンダーIDを取得します。
        /// </summary>
        public string VendorId => _requestDataContainer.Value.VendorId;

        /// <summary>
        /// ユーザーのシステムIDを取得します。
        /// </summary>
        public string SystemId => _requestDataContainer.Value.SystemId;

        /// <summary>
        /// UTCを取得します。
        /// </summary>
        protected DateTime UtcNow => _requestDataContainer.Value.GetDateTimeUtil().GetUtc(_requestDataContainer.Value.GetDateTimeUtil().LocalNow);
    }
}
