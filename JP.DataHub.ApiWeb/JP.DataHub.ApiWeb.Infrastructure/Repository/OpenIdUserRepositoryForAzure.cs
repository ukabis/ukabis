using System.Net;
using System.Text;
using System.Web;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.OpenIdUser;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Data.GraphApi;
using JP.DataHub.ApiWeb.Infrastructure.Models.AzureAdb2c;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    /// <summary>
    /// OpenIdユーザー管理のためのリポジトリです。
    /// </summary>
    class OpenIdUserRepositoryForAzure : IOpenIdUserRepository
    {
        private static readonly string s_apiEndpoint;
        private static readonly string s_applicationEndpoint;
        private static string s_systemIdPropertyName;

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, OpenIdUser>().ConstructUsing(s => CreateInstance(s));
            });

            return config.CreateMapper();
        });
        private static IMapper s_mapper => s_lazyMapper.Value;

        private HttpClient _client;


        /// <summary>
        /// クラスを初期化します。
        /// </summary>
        static OpenIdUserRepositoryForAzure()
        {
            var config = UnityCore.Resolve<IConfiguration>();
            var tenant = config.GetValue<string>("ida:GraphApiTenant") ?? config.GetValue<string>("OpenId:Tenant");
            s_apiEndpoint = string.Format(config.GetValue<string>("ida:GraphApiEndpoint"), tenant, "users", config.GetValue<string>("ida:GraphApiVersion"));
            s_applicationEndpoint = string.Format(config.GetValue<string>("ida:GraphApiEndpoint"), tenant, "applications", config.GetValue<string>("ida:GraphApiVersion"));
        }


        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="httpClient">OpenIdプロバイダーに接続するHttpClient</param>
        public OpenIdUserRepositoryForAzure(HttpClient httpClient)
        {
            _client = httpClient;
        }

        /// <summary>
        /// シリアライザを作成します。
        /// </summary>
        /// <returns>JsonSerializer</returns>
        private async Task<JsonSerializer> CreateSerializer()
        {
            // SystemIdのプロパティ名が未設定の場合は取得する
            if (string.IsNullOrEmpty(s_systemIdPropertyName))
            {
                // ODataのqueryを作成
                var query = "&$filter=startswith(displayName, 'b2c-extensions-app')";

                string objectId = null;

                // Graph API呼び出し
                using (var response = await _client.GetAsync(s_applicationEndpoint + query))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // 結果を読み取り
                        var jobj = JObject.Parse(await response.Content.ReadAsStringAsync());

                        // objectIdを取得
                        objectId = (string)jobj.SelectToken("value[*].objectId");
                    }
                    else
                    {
                        throw await CreateOperationException(response);
                    }
                }

                // プロパティ名取得URL作成
                var builder = new UriBuilder(s_applicationEndpoint);
                builder.Path += "/" + objectId + "/extensionProperties";

                // Graph API呼び出し
                using (var response = await _client.GetAsync(builder.Uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // 結果を読み取り
                        var jobj = JObject.Parse(await response.Content.ReadAsStringAsync());

                        // プロパティ名を取得
                        var tokens = jobj.SelectTokens("value[*].name");
                        var token = tokens.Where(t => t.Value<string>().EndsWith("_SystemId")).FirstOrDefault();
                        s_systemIdPropertyName = token?.Value<string>();
                    }
                    else
                    {
                        throw await CreateOperationException(response);
                    }
                }
            }

            // シリアライザを作成
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new CustomUserAttributeRenameContractResolver(s_systemIdPropertyName);

            return serializer;
        }

        /// <summary>
        /// 指定されたオブジェクトIDのユーザーを削除します。
        /// </summary>
        /// <param name="objectId">OpenIdプロバイダー中のオブジェクトID</param>
        /// <returns>Task</returns>
        public async Task Delete(ObjectId objectId)
        {
            // Uriを作成
            var builder = new UriBuilder(s_apiEndpoint);
            builder.Path += "/" + objectId.Value;

            // Graph API呼び出し
            using (var response = await _client.DeleteAsync(builder.Uri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await CreateOperationException(response);
                }
            }
        }

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        public async Task<OpenIdUser> Get(UserId userId)
        {
            string mailAddress = HttpUtility.UrlEncode(userId.Value);
            // ODataのfilterを作成
            string filter = $"&$filter=signInNames/any(n: n/value eq '{mailAddress}') or otherMails/any(m: m eq '{mailAddress}') and creationType eq 'LocalAccount'";

            // Graph API呼び出し
            using (var response = await _client.GetAsync(s_apiEndpoint + filter))
            {
                if (response.IsSuccessStatusCode)
                {
                    // 結果を読み取り
                    var jobj = JObject.Parse(await response.Content.ReadAsStringAsync());

                    // シリアライザを作成
                    var serializer = await CreateSerializer();

                    // ユーザーの一覧を取得
                    var user = jobj["value"].ToObject<IEnumerable<User>>(serializer).FirstOrDefault();
                    // 結果を返却
                    return s_mapper.Map<OpenIdUser>(user);
                }
                else
                {
                    throw await CreateOperationException(response);
                }
            }
        }

        /// <summary>
        /// 指定された条件で登録されたユーザーの一覧を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>ユーザーの一覧</returns>
        public async Task<IEnumerable<OpenIdUser>> GetList(SystemId systemId)
        {
            // シリアライザを作成
            var serializer = await CreateSerializer();

            // ODataのqueryを作成
            var query = $"&$filter={s_systemIdPropertyName} eq '{systemId.Value}'";

            // Graph API呼び出し
            using (var response = await _client.GetAsync(s_apiEndpoint + query))
            {
                if (response.IsSuccessStatusCode)
                {
                    // 結果を読み取り
                    var jobj = JObject.Parse(await response.Content.ReadAsStringAsync());

                    // ユーザーの一覧を取得
                    var users = jobj["value"].ToObject<IEnumerable<User>>(serializer);
                    // 結果を返却
                    return s_mapper.Map<IEnumerable<OpenIdUser>>(users).OrderBy(u => u.UserId.Value);
                }
                else
                {
                    throw await CreateOperationException(response);
                }
            }
        }

        /// <summary>
        /// 指定されたユーザー情報でユーザーを登録します。
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>登録結果</returns>
        public async Task<OpenIdUser> Register(OpenIdUser user)
        {
            // リクエストを作成
            var request = new HttpRequestMessage(HttpMethod.Post, s_apiEndpoint);
            // リクエスト送信
            return await SendRequest(request, user);
        }

        /// <summary>
        /// 指定されたユーザー情報でユーザーを更新します。
        /// </summary>
        /// <param name="objectId">OpenIdプロバイダー中のオブジェクトID</param>
        /// <param name="user">ユーザー情報</param>
        /// <returns>Task</returns>
        public async Task Update(ObjectId objectId, OpenIdUser user)
        {
            // Uriを作成
            var builder = new UriBuilder(s_apiEndpoint);
            builder.Path += "/" + objectId.Value;

            // リクエストを作成
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), builder.Uri);
            // リクエスト送信
            await SendRequest(request, user);
        }

        /// <summary>
        /// 指定されたHttpリクエストでユーザー情報を登録・更新します。
        /// </summary>
        /// <param name="request">Httpリクエスト</param>
        /// <param name="user">ユーザー情報</param>
        /// <returns>登録結果</returns>
        private async Task<OpenIdUser> SendRequest(HttpRequestMessage request, OpenIdUser user)
        {
            // シリアライザを作成
            var serializer = await CreateSerializer();

            // 登録データ作成
            var registerUser = new RegisterUser
            {
                signInNames = new[] { new SignInName { value = user.UserId.Value } },
                displayName = user.UserName,
                passwordProfile = new Password { password = user.Password },
                systemId = user.SystemId.Value
            };

            // シリアライザの設定を作成
            var serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = serializer.ContractResolver
            };
            // 設定を指定してシリアライズ
            string json = JsonConvert.SerializeObject(registerUser, serializerSettings);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // Graph API呼び出し
            using (var response = await _client.SendAsync(request))
            {
                if (response.IsSuccessStatusCode)
                {
                    // 結果を読み取り
                    JObject jobj = null;
                    try
                    {
                        jobj = JObject.Parse(await response.Content.ReadAsStringAsync());
                    }
                    catch
                    {
                        // do nothing
                    }

                    if (jobj != null)
                    {
                        // ユーザーを取得
                        var registerdUser = jobj.ToObject<User>(serializer);
                        // 結果を返却
                        return s_mapper.Map<OpenIdUser>(registerdUser);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    throw await CreateOperationException(response);
                }
            }
        }

        /// <summary>
        /// レスポンスに応じてOpenIdUserOperationExceptionを作成します。
        /// </summary>
        /// <param name="response">レスポンス</param>
        /// <returns>OpenIdUserOperationException</returns>
        private async Task<OpenIdUserOperationException> CreateOperationException(HttpResponseMessage response)
        {
            string message;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorObj = JObject.Parse(await response.Content.ReadAsStringAsync());
                message = (string)errorObj.SelectToken("['odata.error'].message.value");
            }
            else
            {
                message = await response.Content.ReadAsStringAsync();
            }

            return new OpenIdUserOperationException(message);
        }


        private static OpenIdUser CreateInstance(User s)
        {
            var userId = s.signInNames.Length > 0 ? s.signInNames.First().value : s.otherMails.FirstOrDefault();
            return new OpenIdUser(s.objectId, userId, s.systemId, null, s.displayName, s.createdDateTime);
        }
    }
}
