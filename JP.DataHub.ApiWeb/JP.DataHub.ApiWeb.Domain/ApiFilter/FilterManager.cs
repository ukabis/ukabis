using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;
using Microsoft.Extensions.Options;
using JP.DataHub.Aop;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Domain.ApiFilter
{
    // .NET6
    public class FilterManager : IFilterManager
    {
        protected IApiHelper ApiHelper { get; set; }
        protected ITermsHelper TermsHelper { get; set; }
        protected ICacheHelperFactory CacheHelperFactory { get; set; }
        private List<ApiFilterConfig> list;

        public FilterManager(IApiHelper apiHelper, ITermsHelper termsHelper, ICacheHelperFactory cacheHelperFactory)
        {
            ApiHelper = apiHelper;
            TermsHelper = termsHelper;
            CacheHelperFactory = cacheHelperFactory;
            list = UnityCore.Resolve<IOptions<List<ApiFilterConfig>>>()?.Value;
        }

        public List<IApiFilter> GetApiFilter(IApiFilterActionParam param)
        {
            param.ApiHelper = ApiHelper;
            param.TermsHelper = TermsHelper;
            // Filter一覧の絞り込みと並び替え
            return list?.Where(
                x =>
                {
                    var isMatchApiFilterActionParam =
                        (string.IsNullOrEmpty(x.ResourceUrl) || x.ResourceUrl == "*" || x.ResourceUrl == param.ResourceUrl || Regex.IsMatch(param.ResourceUrl, x.ResourceUrl)) &&
                        (string.IsNullOrEmpty(x.ApiUrl) || x.ApiUrl == "*" || x.ApiUrl == param.ApiUrl || Regex.IsMatch(param.ApiUrl, x.ApiUrl)) &&
                        (string.IsNullOrEmpty(x.Action) || x.Action == "*" || x.Action == param.Action || Regex.IsMatch(param.Action, x.Action)) &&
                        (string.IsNullOrEmpty(x.HttpMethod) || x.HttpMethod == "*" || x.HttpMethod.ToUpper() == param.HttpMethodType.ToUpper() || Regex.IsMatch(param.HttpMethodType, x.HttpMethod)) &&
                        (string.IsNullOrEmpty(x.VendorId) || x.VendorId == "*" || x.VendorId == param.VendorId || Regex.IsMatch(param.VendorId, x.VendorId)) &&
                        (string.IsNullOrEmpty(x.SystemId) || x.SystemId == "*" || x.SystemId == param.SystemId || Regex.IsMatch(param.SystemId, x.SystemId));

                    var isMatchApiFilterActionParamEx =
                        (string.IsNullOrEmpty(x.RequestVendorId) || x.RequestVendorId == "*" || x.RequestVendorId == param.RequestVendorId || Regex.IsMatch(param.RequestVendorId ?? "null", x.RequestVendorId)) &&
                        (string.IsNullOrEmpty(x.RequestSystemId) || x.RequestSystemId == "*" || x.RequestSystemId == param.RequestSystemId || Regex.IsMatch(param.RequestSystemId ?? "null", x.RequestSystemId));
                    return isMatchApiFilterActionParam && isMatchApiFilterActionParamEx;

                    return isMatchApiFilterActionParam;
                }).OrderBy(x => x.Level).ThenBy(x => x.Seq).ToList()
                ?.Select(x => new { filter = x.Load(ApiHelper, CacheHelperFactory?.Create(x.Assembly), TermsHelper, x.Param, new JPDataHubLogger(x.Type)), level = x.Level }).Where(x => x.filter != null).ToList()
                ?.GroupBy(x => x.level)?.FirstOrDefault()?.Select(x => x.filter)?.ToList();
                // GroupByをしている理由は、仕様として優先度の高い者（Levelが小さい）だけを返す仕様。もし同一Levelが複数存在する場合はSeqの順番に処理することとする
            }
        }
    }
