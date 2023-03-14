using AutoMapper;
using JP.DataHub.Com.Extensions;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Core.DataContainer;
using JP.DataHub.Com.Unity;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Log;

namespace JP.DataHub.ManageApi.Service.DymamicApi
{
    internal class JsonFormatCustomValidator : IJsonValidator
    {
        private static bool IsEnableCheckForeignKey { get => _isEnableCheckForeignKey.Value; }
        private static Lazy<bool> _isEnableCheckForeignKey = new Lazy<bool>(() => UnityCore.Resolve<bool>("EnableCheckForeignKey"));
        private static List<string> CheckForeignKeyAllowList { get => _checkForeignKeyAllowList.Value; }
        private static Lazy<List<string>> _checkForeignKeyAllowList = new Lazy<List<string>>(() => UnityCore.Resolve<string>("CheckForeignKeyAllowList")?.Split(',').ToList());

        /// <summary>
        /// FKの問い合わせ結果のタスクキャッシュ
        /// 並列で処理されたときに 時間差で同一のURLに対して リクエスト飛んだ時にwaitしたいので task型にしている
        /// </summary>
        private ConcurrentDictionary<string, ValueTask<int>> TaskCache = new ConcurrentDictionary<string, ValueTask<int>>();

        private readonly Lazy<bool> _UseForeignKeyCache =
            new Lazy<bool>(() => UnityCore.Resolve<bool>("UseForeignKeyCache"));
        private bool UseForeignKeyCache => _UseForeignKeyCache.Value;

        private IPerRequestDataContainer DataContainer;
        private HttpContext LocalHttpContext;

        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
        }).CreateMapper();

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
            if (string.IsNullOrEmpty(@group.Value)) return;
            if (IsEnableCheckForeignKey != true)
            {
                if (CheckForeignKeyAllowList == null || CheckForeignKeyAllowList.Where(x => @group.Value.StartsWith(x)).Any() == false) return;
            }

            string url = @group.Value.Replace("{value}", value.ToString());
            int idx = url.IndexOf('?');
            string query = idx > 0 ? url.Substring(idx) : null;
            string path = idx > 0 ? url.Substring(0, idx) : url;
            string cacheKey = $"{path}{query}";

            int status;
            ValueTask<int> task;
            if (UseForeignKeyCache)
            {
                if (TaskCache.TryGetValue(cacheKey, out task))
                {
                    status = task.Result;
                }
                else
                {
                    status = 200;
                }
            }
            else
            {
                status = GetDataFromApi(path, query);
            }

            //IsSuccessStatusCode
            if ((HttpStatusCode)status != HttpStatusCode.OK)
            {
                string errorText = string.Format("There are no results from {0}", url);
                context.RaiseError(errorText);
                var log = new JPDataHubLogger(this.GetType());
                log.Error(errorText);
                log.Error(Environment.StackTrace);
            }
        }

        private int GetDataFromApi(string path, string query)
        {
            return (int)HttpStatusCode.OK;
        }

        public IReadOnlyCollection<string> GetSchemaFormatList()
        {
            throw new NotImplementedException();
        }
    }
}
