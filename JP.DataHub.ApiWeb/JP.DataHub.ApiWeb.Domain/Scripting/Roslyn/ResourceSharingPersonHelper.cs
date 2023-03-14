using System.Net;
using System.Net.Http;
using AutoMapper;
using Newtonsoft.Json;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;
using JP.DataHub.ApiWeb.Domain.Service;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// 個人リソースシェアリングの情報を参照・登録するためのヘルパークラスです。
    /// </summary>
    [RoslynScriptHelp]
    public class ResourceSharingPersonHelper
    {
        private IPerRequestDataContainer container = UnityCore.Resolve<IPerRequestDataContainer>();
        private IResourceSharingPersonService service = UnityCore.Resolve<IResourceSharingPersonService>();

        #region AutoMapper
        /// <summary>
        /// AutoMapperの設定
        /// </summary>
        private static Lazy<IMapper> _Mapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ResourceSharingPersonRuleModel, ApiModel>()
                    .ForMember(d => d.ResourceSharingPersonRuleId, opt => opt.MapFrom(s => Guid.Parse(s.ResourceSharingPersonRuleId)))
                    .ForMember(d => d.ResourceSharingRuleName, opt => opt.MapFrom(s => s.ResourceSharingRuleName))
                    .ForMember(d => d.SharingFromUserId, opt => opt.MapFrom(s => s.SharingFromUserId))
                    .ForMember(d => d.SharingFromMailAddress, opt => opt.MapFrom(s => s.SharingFromMailAddress))
                    .ForMember(d => d.SharingToUserId, opt => opt.MapFrom(s => s.SharingToUserId))
                    .ForMember(d => d.SharingToMailAddress, opt => opt.MapFrom(s => s.SharingToMailAddress));
            });

            return config.CreateMapper();
        });

        /// <summary>AutoMapper</summary>
        private static IMapper Mapper => _Mapper.Value;
        #endregion


        /// <summary>
        /// 自分からの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">
        /// 共有対象のリソースパス
        /// </param>
        /// <returns>
        /// 個人リソースシェアリングルールの一覧
        /// </returns>
        public HttpResponseMessage GetFromList(string path)
        {
            try
            {
                var list = service.GetFromList(path, container.OpenId);
                return CreateResponse(list);
            }
            catch (Exception ex)
            {
                return CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// 自分への個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">
        /// 共有対象のリソースパス
        /// </param>
        /// <returns>
        /// 個人リソースシェアリングルールの一覧
        /// </returns>
        public HttpResponseMessage GetToList(string path)
        {
            try
            {
                var list = service.GetToList(path, container.OpenId);
                return CreateResponse(list);
            }
            catch (Exception ex)
            {
                return CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// 個人リソースシェアリングルールを登録します。
        /// </summary>
        /// <param name="contents">
        /// POSTされたBody
        /// </param>
        /// <param name="path">
        /// 共有対象のリソースパス
        /// </param>
        /// <returns>
        /// 登録結果
        /// </returns>
        public HttpResponseMessage Register(string contents, string path)
        {
            try
            {
                // BodyをDeserialize
                var requestModel = JsonConvert.DeserializeObject<ApiModel>(contents);

                // ADB2Cのテナントからユーザー情報を取得
                var oidUserService = UnityCore.Resolve<IOpenIdUserApplicationService>();
                var task = Task.Run(() => oidUserService.Get(new UserId(requestModel.SharingToMailAddress)));
                var user = task.Result;
                if (user == null)
                {
                    return CreateResponse(HttpStatusCode.BadRequest, $"指定されたメールアドレスのユーザーは存在しません。: {requestModel.SharingToMailAddress}");
                }

                var statusCode = HttpStatusCode.OK;
                ResourceSharingPersonRuleModel registered;

                if (string.IsNullOrEmpty(requestModel.ResourceSharingPersonRuleId))
                {
                    // 新規登録
                    var model = new ResourceSharingPersonRuleModel()
                    {
                        ResourceSharingPersonRuleId = Guid.NewGuid().ToString(),
                        ResourceSharingRuleName = requestModel.ResourceSharingRuleName,
                        ResourcePath = path,
                        SharingFromUserId = container.OpenId,
                        SharingFromMailAddress = container.Claims["emails"],
                        SharingToUserId = user.ObjectId.Value,
                        SharingToMailAddress = requestModel.SharingToMailAddress,
                        SharingToVendorId = null,
                        SharingToSystemId = null,
                        Query = null,
                        Script = null,
                        IsEnable = true
                    };

                    registered = service.Register(model);
                    statusCode = HttpStatusCode.Created;
                }
                else
                {
                    // 既存データ取得
                    var model = service.Get(requestModel.ResourceSharingPersonRuleId.ToString());
                    if (model == null)
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }

                    // 更新
                    model.ResourceSharingRuleName = requestModel.ResourceSharingRuleName;
                    model.SharingFromUserId = container.OpenId;
                    model.SharingFromMailAddress = container.Claims["emails"];
                    model.SharingToUserId = user.ObjectId.Value;
                    model.SharingToMailAddress = requestModel.SharingToMailAddress;
                    registered = service.Update(model);
                }

                // レスポンス作成
                var result = Mapper.Map<ApiModel>(registered);
                var content = new StringContent(JsonConvert.SerializeObject(result));
                return new HttpResponseMessage(statusCode) { Content = content };
            }
            catch (JsonException ex)
            {
                return CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (AlreadyExistsException ex)
            {
                return CreateResponse(HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                return CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// 指定されたIDの個人リソースシェアリングルールを削除します。
        /// </summary>
        /// <param name="keyValues">
        /// URLパラメータ
        /// </param>
        /// <returns>
        /// 削除結果
        /// </returns>
        public HttpResponseMessage Delete(Dictionary<string, string> keyValues)
        {
            // パラメータ存在チェック
            if (!keyValues.ContainsKey("ResourceSharingPersonRuleId"))
                return CreateResponse(HttpStatusCode.BadRequest, "ResourceSharingPersonRuleIdを指定してください。");

            string id = keyValues["ResourceSharingPersonRuleId"];

            try
            {
                // 形式チェック
                Guid.Parse(id);
            }
            catch (Exception ex)
            {
                return CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            try
            {
                service.Delete(id);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (NotFoundException)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// 検索結果のレスポンスを作成します。
        /// </summary>
        /// <param name="list">検索結果</param>
        /// <returns>検索結果のレスポンス</returns>
        private HttpResponseMessage CreateResponse(IEnumerable<ResourceSharingPersonRuleModel> list)
        {
            if (list.Any())
            {
                var result = Mapper.Map<IEnumerable<ApiModel>>(list);
                var content = new StringContent(JsonConvert.SerializeObject(result));
                var response = new HttpResponseMessage() { Content = content };
                response.Content.Headers.ContentType.MediaType = "application/json";
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// メッセージレスポンスを作成します。
        /// </summary>
        /// <param name="code">HttpStatusCode</param>
        /// <param name="message">メッセージ</param>
        /// <returns>メッセージレスポンス</returns>
        private HttpResponseMessage CreateResponse(HttpStatusCode code, string message)
        {
            var content = new StringContent(JsonConvert.SerializeObject(message));
            return new HttpResponseMessage(code) { Content = content };
        }

        /// <summary>
        /// 個人リソースシェアリングAPIモデル
        /// </summary>
        private class ApiModel
        {
            /// <summary>
            /// 個人リソースシェアリングルールID
            /// </summary>
            public string ResourceSharingPersonRuleId { get; set; }

            /// <summary>
            /// ルール名
            /// </summary>
            public string ResourceSharingRuleName { get; set; }

            /// <summary>
            /// 共有元ユーザーID
            /// </summary>
            public string SharingFromUserId { get; set; }

            /// <summary>
            /// 共有元メールアドレス
            /// </summary>
            public string SharingFromMailAddress { get; set; }

            /// <summary>
            /// 共有先ユーザーID
            /// </summary>
            public string SharingToUserId { get; set; }

            /// <summary>
            /// 共有先メールアドレス
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string SharingToMailAddress { get; set; }
        }

        /// <summary>
        /// 返却メッセージ
        /// </summary>
        private class ResponseMessage
        {
            /// <summary>
            /// インスタンスを初期化します。
            /// </summary>
            /// <param name="messge">メッセージ</param>
            public ResponseMessage(string messge)
            {
                Message = messge;
            }

            /// <summary>メッセージ</summary>
            public string Message { get; set; }
        }
    }
}
