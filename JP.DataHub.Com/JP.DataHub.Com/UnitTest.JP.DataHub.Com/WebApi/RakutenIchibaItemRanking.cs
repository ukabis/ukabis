using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;

namespace UnitTest.JP.DataHub.Com.WebApi
{
    [WebApiResource("https://app.rakuten.co.jp/services/api/IchibaItem", typeof(string))]
    public interface IRakutenIchibaItemRanking
    {
        WebApiRequestModel<string> GetRanking();
        WebApiRequestModel<string> GetRanking(string format, string applicationId);
    }

    [WebApiResource("https://app.rakuten.co.jp/services/api/IchibaItem")]
    public class RakutenIchibaItemRanking_Inheritence : Resource, IRakutenIchibaItemRanking
    {
        [WebApi("Ranking/20170628?format=json&applicationId=28de5a25a84b556f1166b173acb7fa94")]
        public WebApiRequestModel<string> GetRanking()
        {
            return MakeApiRequestModel<WebApiRequestModel<string>>();
        }

        [WebApi("Ranking/20170628?format={format}&applicationId={applicationId}")]
        public WebApiRequestModel<string> GetRanking(string format, string applicationId)
        {
            return MakeApiRequestModel<WebApiRequestModel<string>>(new object[] { format, applicationId });
        }
    }

    [WebApiResource("https://app.rakuten.co.jp/services/api/IchibaItem")]
    public class RakutenIchibaItemRanking : Resource, IRakutenIchibaItemRanking
    {
        public RakutenIchibaItemRanking()
            : base()
        {
        }
        public RakutenIchibaItemRanking(string serverUrl)
            : base(serverUrl)
        {
        }
        public RakutenIchibaItemRanking(ServerEnvironment serverEnvironment)
            : base(serverEnvironment)
        {
        }

        [WebApi("Ranking/20170628?format=json&applicationId=28de5a25a84b556f1166b173acb7fa94")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> GetRanking() => null;

        [WebApi("Ranking/20170628?format={format}&applicationId={applicationId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> GetRanking(string format, string applicationId) => null;
    }
}
