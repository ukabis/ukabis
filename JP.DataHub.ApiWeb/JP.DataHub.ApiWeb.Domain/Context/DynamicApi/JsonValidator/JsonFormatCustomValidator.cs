using System;
using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.Extensions;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator
{
    public class JsonFormatCustomValidator : IJsonValidator
    {
        private static bool IsEnableCheckForeignKey { get => s_lazyIsEnableCheckForeignKey.Value; }
        private static Lazy<bool> s_lazyIsEnableCheckForeignKey = new Lazy<bool>(() => UnityCore.Resolve<IConfiguration>().GetValue<bool>("AppConfig:EnableCheckForeignKey", true));

        private static List<string> CheckForeignKeyAllowList { get => s_lazyCheckForeignKeyAllowList.Value; }
        private static Lazy<List<string>> s_lazyCheckForeignKeyAllowList = new Lazy<List<string>>(() => UnityCore.Resolve<IConfiguration>().GetSection("AppConfig:CheckForeignKeyAllowList").Get<string[]>()?.ToList());

        /// <summary>
        /// FKの問い合わせ結果のタスクキャッシュ
        /// 並列で処理されたときに 時間差で同一のURLに対して リクエスト飛んだ時にwaitしたいので task型にしている
        /// </summary>
        private ConcurrentDictionary<string, ValueTask<int>> _taskCache = new ConcurrentDictionary<string, ValueTask<int>>();

        private bool UseForeignKeyCache => s_useForeignKeyCache.Value;
        private readonly Lazy<bool> s_useForeignKeyCache = new Lazy<bool>(() => UnityCore.Resolve<bool>("UseForeignKeyCache"));

        private Lazy<IHttpContextAccessor> _lazyHttpContextAccessor = new Lazy<IHttpContextAccessor>(() => UnityCore.Resolve<IHttpContextAccessor>());
        private IHttpContextAccessor _httpContextAccessor => _lazyHttpContextAccessor.Value;

        public void Validate(JToken value, JsonValidatorContext context)
        {
            if (value.Type == JTokenType.String)
            {
                var parseFormat = JsonPropertyFormatParser.ParseFormat(context.Schema.Format);
                // JsonSchema規定のチェック処理(date, ipなど)は、format指定が１要素のみのときは行わない。JsonSchema規定のチェック処理とOtherのチェック処理が重複し、エラーメッセージが２つ出てしまうため。
                if (parseFormat.Count > 1)
                {
                    parseFormat
                        .Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.Other)
                        .Where(x => GetSchemaFormatList().Contains(x.FormatTypeName))
                        .ToList().ForEach(x => CheckJSchemaDefinedFormat(value, x.FormatTypeName, context));
                }

                // ForeignKeyチェック処理
                parseFormat.Where(x => x.FormatType == JsonPropertyFormatParser.JsonFormatType.ForeignKey).ToList().ForEach(x => CheckForeignKey(value, x.Match.Groups[x.KeyName1], context));

            }
            else if (value.Type == JTokenType.Array)
            {
                foreach (var v in value)
                {
                    Validate(v, context);
                }
            }
        }

        /**
         * 独自定義以外のValidation（JsonSchema規定のValidation）を実行する
         */
        private void CheckJSchemaDefinedFormat(JToken value, string formatTypeName, JsonValidatorContext context)
        {
            // str経由でdeepCopy
            var tempJSchemaStr = context.Schema.ToString();
            var jSchema = JSchema.Parse(tempJSchemaStr);
            // format差し替え
            jSchema.Format = formatTypeName;
            // JsonSchema規定のValidationを実施
            try
            {
                value.Validate(jSchema);
            }
            catch (JSchemaValidationException ex)
            {
                if (ex.ValidationError.ErrorType == ErrorType.Format)
                {
                    context.RaiseError(ex.Message);
                }
                // Format以外のValidation(maxLength, minLengthなど)は、JToken.IsValid(JSchema)の標準Validationで行う
            }
        }

        public bool CanValidate(JSchema schema) => JsonPropertyFormatParser.ParseFormat(schema.Format).Count > 0;

        private void CheckForeignKey(JToken value, Group group, JsonValidatorContext context)
        {
            if (string.IsNullOrEmpty(group.Value))
            {
                return;
            }

            if (IsEnableCheckForeignKey != true)
            {
                if (CheckForeignKeyAllowList == null || CheckForeignKeyAllowList.Where(x => group.Value.StartsWith(x)).Any() == false)
                {
                    return;
                }
            }

            var url = group.Value.Replace("{value}", value.ToString());
            var idx = url.IndexOf('?');
            var query = idx > 0 ? url[idx..] : null;
            var path = idx > 0 ? url[..idx] : url;
            var cacheKey = $"{path}{query}";

            int status;
            ValueTask<int> task;
            if (UseForeignKeyCache)
            {
                if (_taskCache.TryGetValue(cacheKey, out task))
                {
                    status = task.Result;
                }
                else
                {
                    var dataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                    var localHttpContext = _httpContextAccessor.HttpContext;
                    //Cacheのdicに格納するのがTaskなので ここで一時的に変数に入れる(即Resultするとtaskが取れなくなる)
                    task = new ValueTask<int>(TaskExtension.RunWithContext(() => GetDataFromApi(path, query), localHttpContext, dataContainer));
                    _taskCache.TryAdd(cacheKey, task);
                    status = task.Result;
                }
            }
            else
            {
                status = GetDataFromApi(path, query);
            }

            if ((HttpStatusCode)status != HttpStatusCode.OK)
            {
                var errorText = $"There are no results from {url}";
                context.RaiseError(errorText);
                var log = new JPDataHubLogger(this.GetType());
                log.Error(errorText);
                log.Error(Environment.StackTrace);
            }
        }

        private int GetDataFromApi(string path, string query)
        {
            var header = _httpContextAccessor?.HttpContext?.Request?.Headers?.ToHttpHeaderValueObject();
            var api = UnityCore.Resolve<IDynamicApiApplicationService>();
            var response = api.Request(new HttpMethodType(HttpMethodType.MethodTypeEnum.GET), path.ToRequestRelativeUri(), new Contents(Stream.Null), query.ToQueryString(), header, new MediaType(MediaTypeConst.ApplicationJson), new Accept(MediaTypeConst.ApplicationJson), new ContentRange(""), new ContentType(MediaTypeConst.ApplicationJson), new ContentLength(0));
            return (int)response.StatusCode;
        }

        public IReadOnlyCollection<string> GetSchemaFormatList()
        {
            throw new NotImplementedException();
        }
    }
}