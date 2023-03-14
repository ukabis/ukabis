using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Misc;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    internal static class AllApiEntityExtension
    {
        internal static bool IsMatchCommon<T>(T api, HttpMethodType httpMethodType, string urlRelative, string query, string[] splitRelative)
        where T : IAllApiEntityIdentifier
        {
            // 相対URL（アクセスしてきたもの）がなかったり、DB保存しているAPI情報のURLが存在しない場合は、APIとして成立しない、アクセスしたURLがおかしいため、それらは該当なしとする
            if (urlRelative == null || api.method_name == null || api.controller_relative_url == null)
            {
                return false;
            }
            // HTTPメソッドタイプがあっているか？
            if (api.method_type.ToUpper() != httpMethodType.Value.ToString().ToUpper())
            {
                return false;
            }

            // メソッド名がマッチするか？
            Dictionary<string, string> keyValue = UriUtil.IsMatchUrlWithoutQueryString(splitRelative, api.controller_relative_url, api.method_name);
            if (keyValue == null)
            {
                // 別名としてのメソッド名がマッチするか？
                keyValue = UriUtil.IsMatchUrlWithoutQueryString(splitRelative, api.controller_relative_url, api.alias_method_name);
                if (keyValue == null)
                {
                    return false;
                }
            }

            //クエリー部分もチェック(getewayはチェック不要)
            if (api.action_type_cd != ActionType.Gateway.ToCode() && api.is_nomatch_querystring == false)
            {
                var tmpQuery = query ?? "";
                List<string> queryKey1 = new List<string>();
                if (tmpQuery.Contains("?"))
                {
                    queryKey1 = UriUtil.GetParameterKeyList(tmpQuery).Distinct().ToList();
                }
                else
                {
                    queryKey1 = UriUtil.GetParameterKeyListToQueryParameter(tmpQuery);
                }

                // QueryStringをパースする
                // QueryStringで必須なものが全て指定されているか？（必須に指定されているものが無ければこれはNG）
                // QueryStringで必須でないものが指定されている場合はよしとする
                // URL定義に含まれないパラメータが指定されている場合はNGとする
                var uriDefinition = UriUtil.ParseQueryStringDefinition(api.controller_relative_url + "/" + api.method_name);
                foreach (var key in queryKey1)
                {
                    var hit = uriDefinition.FirstOrDefault(x => x.Key == key);
                    if (hit != null)
                    {
                        hit.TmpFlag = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (uriDefinition.Where(x => x.TmpFlag == false && x.IsRequired == true).Count() > 0)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsMatch(this AllApiEntity api, HttpMethodType httpMethodType, string urlRelative, string query, string[] splitRelative)
            => IsMatchCommon<AllApiEntity>(api, httpMethodType, urlRelative, query, splitRelative);

        internal static bool IsMatch(this AllApiEntityIdentifier api, HttpMethodType httpMethodType, string urlRelative, string query, string[] splitRelative)
        {
            return IsMatchCommon<AllApiEntityIdentifier>(
                api,
                httpMethodType,
                urlRelative,
                query,
                splitRelative
            );
        }
    }
}
