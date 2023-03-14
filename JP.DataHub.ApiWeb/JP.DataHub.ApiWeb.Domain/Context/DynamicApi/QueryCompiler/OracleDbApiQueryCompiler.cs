using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.QueryCompiler
{
    // .NET6
    internal class OraclDbeApiQueryCompiler : IApiQueryCompiler
    {
        private const string TableNameParamName = "TABLE_NAME";
        private const string SelfResourceNameParamName = "Resource(Me)";
        private static readonly Regex ResourceNamePatternName = new Regex(@"Resource\([^#]+?\)");

        private static Lazy<IDynamicApiRepository> _dynamicApiRepository => new Lazy<IDynamicApiRepository>(() => UnityCore.Resolve<IDynamicApiRepository>());
        private IDynamicApiRepository DynamicApiRepository => _dynamicApiRepository.Value;


        public Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors> Compile(IDynamicApiAction action)
        {
            //クエリのNullチェック
            if (action.ApiQuery is null || string.IsNullOrWhiteSpace(action.ApiQuery?.Value))
            {
                return new Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors>(null, null);
            }

            //クエリタイプはcdb以外スルー
            if (action.QueryType?.Value != QueryTypes.NativeDbQuery)
            {
                return new Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors>(null, null);
            }

            var apiQueryString = action.ApiQuery.Value;

            //定義の時点で{TableName:ControllerId}になっているものは不正なクエリとしてエラーにする
            var tablenamePattern = new Regex($"{{{TableNameParamName}:[-_0-9a-zA-Z]+}}");
            if (tablenamePattern.IsMatch(action.ApiQuery.Value))
            {
                return new Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors>(null, ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10431));
            }

            //SELECT句の列として{TABLE_NAME}, Resource(Me),Resource(/API/***)が指定された場合はエラーにする
            //後続でテーブル名に置換されテーブル名をユーザーに返却してしまうためi
            var colnamePattern = new Regex(@"select\s+(?<col>.*?)\s*from", RegexOptions.IgnoreCase);
            if (colnamePattern.Matches(apiQueryString)?.Cast<Match>().Any(x =>
            {
                var colName = x.Groups["col"].Value;
                return colName.Contains($"{{{TableNameParamName}}}") || ResourceNamePatternName.IsMatch(colName);
            }) == true)
            {
                return new Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors>(null, ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10434));
            }

            //Resource(Me)の置換
            apiQueryString = apiQueryString.Replace(SelfResourceNameParamName, $"{{{TableNameParamName}}}");

            //Resource(/API/***)の置換
            //クエリをパースして拡張書式(Resource(***) があったら 今のactionからアクセス権をチェックする
            var resourceNamePattern = @"Resource\((?<url>/API/[^?#]+?)(?<querystring>\?([^#])*?)*?\)";
            var resourceParameters = new Regex(resourceNamePattern).Matches(apiQueryString).Cast<Match>().Distinct();
            foreach (var resourceParameter in resourceParameters)
            {
                //アクセス権チェック処理
                var targetApiUrl = new RequestRelativeUri(resourceParameter.Groups["url"].Value);
                var targetApiQueryString = new GetQuery(resourceParameter.Groups["querystring"].Value);
                var urlWithQueryString = resourceParameter.Groups["url"].Value + resourceParameter.Groups["querystring"].Value;

                // Api取得
                var api = DynamicApiRepository.FindApi(action.MethodType, targetApiUrl, targetApiQueryString);
                if (api is null)
                {
                    return new Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors>(null, ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10432, urlWithQueryString));
                }

                var authResult = Authenticate(action, api);
                if (authResult != null)
                {
                    return new Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors>(null, authResult);
                }
                //↑ がokなら{TableName:ResourceId}に置換する
                apiQueryString = apiQueryString.Replace($"Resource({urlWithQueryString})", $"{{TABLE_NAME:{api.ControllerId.Value}}}");
            }

            return new Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors>(new ApiQuery(apiQueryString), null);
        }

        private static RFC7807ProblemDetailExtendErrors Authenticate(IDynamicApiAction action, IMethod api)
        {
            //アクセス元のAPI(action)とアクセス先のAPI(api)がどちらもSQLアクセス可能になっている必要がある
            if (!(api.IsOtherResourceSqlAccess?.Value == true && action.IsOtherResourceSqlAccess?.Value == true))
            {
                //どっちかがNG
                return ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10433, api.IsOtherResourceSqlAccess?.Value == true ? action.RelativeUri.Value : api.RelativeUri.Value);
            }

            //アクセス先のAPIにアクセス権があるかチェック
            var authResult = api.Authenticate();
            if (authResult.IsSuccessStatusCode) return null;

            var rfc = JsonConvert.DeserializeObject<RFC7807ProblemDetailExtendErrors>(authResult.Content.ReadAsStringAsync().Result);
            return rfc;

        }
    }
}
