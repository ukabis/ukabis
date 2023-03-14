using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    /// <summary>
    /// グループAOPの対象リソースをリクエストするクラス。
    /// </summary>
    /// <remarks>
    /// グループAOPは個人依存のリソースを対象とする。
    /// 本クラスはグループ適用を確認するためのリクエストのみしか扱わないため、
    /// リソース固有のリクエストを行う場合は他のAPIと同様に別途クラスを作成すること。
    /// </remarks>
    public class GroupTargetResource : Resource
    {
        public GroupTargetResource()
            : base()
        {
        }

        public GroupTargetResource(string serverUrl)
            : base(serverUrl)
        {
        }

        public GroupTargetResource(IServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }

        [WebApi("GetList?groupId={groupId}")]
        public WebApiRequestModel<List<GroupTargetResponseModel>> GetList(string groupId) => MakeApiRequestModel<WebApiRequestModel<List<GroupTargetResponseModel>>>(new object[] { groupId });

        [WebApiPost]
        public WebApiRequestModel AdaptResourceSchema() => MakeApiRequestModel<WebApiRequestModel>();
    }
}
