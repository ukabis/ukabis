using System;
using JP.DataHub.Com.Json.Schema;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Misc;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false)]
    internal class DataStoreRepositoryParamODataConvertAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new DataStoreRepositoryParamODataConvertHandler();
        }


        public class DataStoreRepositoryParamODataConvertHandler : ICallHandler
        {
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                IMethodReturn result = null;
                try
                {
                    var repository = input.Target as INewDynamicApiDataStoreRepository;
                    if (repository != null)
                    {
                        if (input.Arguments.Count > 0)
                        {
                            for (int i = 0; i < input.Arguments.Count; i++)
                            {
                                var par = input.Arguments.GetParameterInfo(i);
                                if (par.ParameterType == typeof(QueryParam))
                                {
                                    var sqlManager = UnityCore.Resolve<IODataSqlManager>(repository.RepositoryName);
                                    var queryParam = (QueryParam)input.Arguments[i];
                                    if (queryParam.ODataQuery == null && queryParam.QueryString != null && queryParam.QueryString.Dic.Any(x => x.Key.Value.StartsWith("$")))
                                    {
                                        queryParam = ValueObjectUtil.Create<QueryParam>(new ODataQuery(queryParam.QueryString.GetQueryString()), queryParam);
                                    }

                                    if (queryParam.ODataQuery != null && queryParam.QueryType?.Value != QueryTypes.ODataQuery)
                                    {
                                        // ODataActionの場合の変換
                                        ConvertOdataFromOdataAction(input, queryParam, i, sqlManager, repository);
                                    }
                                    else if (queryParam.QueryType?.Value == QueryTypes.ODataQuery)
                                    {
                                        // クエリにODataQueryが設定されている場合の変換
                                        ConvertOdataFromOdataQuery(input, queryParam, i, sqlManager, repository);
                                    }
                                    else if (!string.IsNullOrEmpty(queryParam.ApiQuery?.Value) && queryParam.ApiQuery.Value.StartsWith("$"))
                                    {
                                        // クエリにApiQuery(OData形式)が設定されている場合の変換
                                        ConvertOdataFromApiQuery(input, queryParam, i, sqlManager, repository);
                                    }
                                }
                            }
                        }
                    }
                    result = getNext()(input, getNext);
                }
                catch (Exception)
                {
                    throw;
                }
                return result;
            }

            /// <summary>
            /// ODataをネイティブクエリに変換する。(ODataの透過APIの場合)
            /// </summary>
            private void ConvertOdataFromOdataAction(IMethodInvocation input, QueryParam queryParam, int index, IODataSqlManager sqlManager, INewDynamicApiDataStoreRepository repository)
            {
                int sqlMode = sqlManager.GetSqlMode(queryParam);
                var nativeSql = sqlManager.CreateSqlQuery(queryParam, queryParam.ODataQuery.Value, repository.GetInternalAddWhereString(queryParam, out var additionalParameters), sqlMode, out int? topCount, out int? skipCount, out Dictionary<string, object> parameters);
                MergeParameters(parameters, additionalParameters);

                // ネイティブクエリに変換するため元のDictonaryからODataクエリは削除する
                var replaceQueryString = new Dictionary<QueryStringKey, QueryStringValue>();
                queryParam.QueryString?.Dic?.Where(x => !x.Key.Value.StartsWith("$")).ToList().ForEach(x => replaceQueryString.Add(x.Key, x.Value));

                input.Arguments[index] = ValueObjectUtil.Create<QueryParam>(
                    new QueryStringVO(replaceQueryString),
                    new NativeQuery(nativeSql, parameters, false),
                    (topCount.HasValue ? new SelectCount((int)topCount) : null),
                    (skipCount.HasValue ? new SkipCount((int)skipCount) : null),
                    queryParam);
            }

            /// <summary>
            /// ODataをネイティブクエリに変換する。(APIクエリのクエリタイプがODataの場合)
            /// </summary>
            private void ConvertOdataFromOdataQuery(IMethodInvocation input, QueryParam queryParam, int index, IODataSqlManager sqlManager, INewDynamicApiDataStoreRepository repository)
            {
                // ネイティブクエリに変換するため元のDictonaryからODataクエリは削除する
                var replaceQueryStringDict = new Dictionary<string, string>();
                var replaceKeyValueDict = new Dictionary<string, string>();
                string tmpQuery = queryParam.ApiQuery?.Value;
                if ((queryParam.QueryString != null || queryParam.KeyValue != null) && !string.IsNullOrWhiteSpace(tmpQuery))
                {
                    JSchema uriSchema = queryParam.UriSchema?.Value == null ? null : JSchema.Parse(queryParam.UriSchema.Value);
                    JSchema requestSchema = queryParam.RequestSchema?.Value == null ? null : JSchema.Parse(queryParam.RequestSchema.Value);
                    JSchema responseSchema = queryParam.ResponseSchema?.Value == null ? null : JSchema.Parse(queryParam.ResponseSchema.Value);
                    List<JSchema> jsonSchemaList = new List<JSchema>() { uriSchema, requestSchema, responseSchema };

                    tmpQuery = ReplaceParametersInQuery(tmpQuery, jsonSchemaList, queryParam.KeyValue.Dic.ToDictionary(x => x.Key.Value, y => y.Value.Value), out replaceKeyValueDict);

                    var filteredQueryString = queryParam.QueryString?.Dic?.Where(x => queryParam.KeyValue?.ContainKey(x.Key.Value) == false).ToDictionary(x => x.Key.Value, y => y.Value.Value);
                    tmpQuery = ReplaceParametersInQuery(tmpQuery, jsonSchemaList, filteredQueryString, out replaceQueryStringDict);
                }
                tmpQuery = UrlEncode(tmpQuery);

                int sqlMode = sqlManager.GetSqlMode(queryParam);
                var replaceQueryString = new QueryStringVO(replaceQueryStringDict.ToDictionary(x => new QueryStringKey(x.Key), y => new QueryStringValue(y.Value)));
                var replaceKeyValue = new UrlParameter(replaceKeyValueDict.ToDictionary(x => new UrlParameterKey(x.Key), y => new UrlParameterValue(y.Value)));
                var newQueryParam = ValueObjectUtil.Create<QueryParam>(replaceQueryString, replaceKeyValue, queryParam);
                var nativeSql = sqlManager.CreateSqlQuery(queryParam, tmpQuery, repository.GetInternalAddWhereString(newQueryParam, out var additionalParameters), sqlMode, out int? topCount, out int? skipCount, out Dictionary<string, object> parameters);
                MergeParameters(parameters, additionalParameters);

                input.Arguments[index] = ValueObjectUtil.Create<QueryParam>(
                    replaceQueryString,
                    replaceKeyValue,
                    new NativeQuery(nativeSql, parameters, false),
                    (topCount.HasValue ? new SelectCount((int)topCount) : null),
                    (skipCount.HasValue ? new SkipCount((int)skipCount) : null),
                    queryParam);
            }

            /// <summary>
            /// ODataをネイティブクエリに変換する。(APIクエリのクエリタイプがOData以外だがODataクエリが入力されている場合)
            /// </summary>
            private void ConvertOdataFromApiQuery(IMethodInvocation input, QueryParam queryParam, int index, IODataSqlManager sqlManager, INewDynamicApiDataStoreRepository repository)
            {
                int sqlMode = sqlManager.GetSqlMode(queryParam);

                var tmpQuery = UrlEncode(queryParam.ApiQuery.Value);
                var nativeSql = sqlManager.CreateSqlQuery(queryParam, tmpQuery, repository.GetInternalAddWhereString(queryParam, out var additionalParameters), sqlMode, out int? topCount, out int? skipCount, out Dictionary<string, object> parameters);
                MergeParameters(parameters, additionalParameters);

                input.Arguments[index] = ValueObjectUtil.Create<QueryParam>(
                    new NativeQuery(nativeSql, parameters, false),
                    (topCount.HasValue ? new SelectCount((int)topCount) : null),
                    (skipCount.HasValue ? new SkipCount((int)skipCount) : null),
                    queryParam);
            }

            private void MergeParameters(IDictionary<string, object> parameters, IDictionary<string, object> additionalParameters)
            {
                foreach (var key in additionalParameters.Keys)
                {
                    if (parameters.ContainsKey(key))
                    {
                        // 同じキーがある場合は追加パラメータで上書き
                        parameters[key] = additionalParameters[key];
                    }
                    else
                    {
                        parameters.Add(key, additionalParameters[key]);
                    }
                }
            }

            private string ReplaceParametersInQuery(string query, List<JSchema> jsonSchemaList, IDictionary<string, string> paramters, out Dictionary<string, string> remainedParameters)
            {
                remainedParameters = new Dictionary<string, string>();

                if (paramters == null)
                {
                    return query;
                }

                foreach (var p in paramters)
                {
                    if (query.Contains($"{{{p.Key}}}"))
                    {
                        Type type = JSchemaTypeExtensions.ToType(jsonSchemaList, p.Key);
                        if (type == typeof(int) || type == typeof(double))
                        {
                            query = query.Replace($"{{{p.Key}}}", $"{p.Value}");
                        }
                        else
                        {
                            query = query.Replace($"{{{p.Key}}}", $"'{p.Value}'");
                        }
                    }
                    else
                    {
                        remainedParameters.Add(p.Key, p.Value);
                    }
                }

                return query;
            }

            private string UrlEncode(string query)
            {
                var tmp = UriUtil.SplitODataQuery($"?{query}").Select(x => new { Key = new QueryStringKey(x.Key), Value = new QueryStringValue(x.Value) }).ToDictionary(key => key.Key, value => value.Value);
                return new QueryStringVO(tmp, null).GetQueryString(true);
            }


            public int Order
            {
                get { return 1; }
                set { }
            }
        }

    }
}
