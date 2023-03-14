using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Unity;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Authentication;
using JP.DataHub.Service.Core;
using JP.DataHub.Com.Authentication;
using JP.DataHub.Blazor.Core.Storage;

namespace JP.DataHub.Blazor.Core.Pages
{
    public class BlazorBasePage : ComponentBase, IPageOrController
    {

        [Inject]
        protected NavigationManager NavManager { get; set; }
        [Inject]
        protected IHttpContextAccessor HttpContextAccessor { get; set; }

        protected ISession Session { get => HttpContextAccessor.HttpContext.Session; }

        protected IUnityContainer Container { get; set; }

        protected IDynamicApiClient CommonDynamicApiClient { get; set; }

        [Inject]
        protected ProtectedSessionStorage SessionStorage { get; set; }


        [Inject]
        protected ITokenAcquisition TokenAcquisition { get; set; }

        [Inject]
        protected ICommonCrudService CrudService { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        protected Microsoft.Identity.Web.ITokenAcquisition TokenAcquisitionService { get; set; }

        protected string oid { get => GetOid(); }
        protected bool IsAuthenticated { get => GetIsAuthenticated(); }
        private object oid_lock = new object();
        private string _oid = null;
        private bool? _isAuthorized = null;
        private string oidtoken = null;

        private DataHubBlazorCoreSessionStorage _dataHubBlazorCoreSessionStorage = new();

        public BlazorBasePage()
        {
            Container = UnityCore.UnityContainer;
            CommonDynamicApiClient = Container.Resolve<IDynamicApiClient>(CommonDynamicApiConst.Key);
        }

        protected bool RequireAuthenticationIfYet()
        {
            var authState = AuthenticationStateProvider.GetAuthenticationStateAsync().Result;
            var user = authState.User;

            if (user.Identity.IsAuthenticated != true)
            {
                var redirectUri = $"/{NavManager.ToBaseRelativePath(NavManager.Uri)}";
                NavManager.NavigateTo($"MicrosoftIdentity/Account/SignIn?redirectUri={redirectUri}", forceLoad: true);
                return true;
            }
            else
            {
                var context = UnityCore.Resolve<IOAuthContext>();
                context.TokenAcquisition = TokenAcquisition;
            }

            if (IsAuthenticated == false)
            {
                NavManager.NavigateTo("UnAuthorized", false);
                return true;
            }

            return false;
        }

        private string GetOid()
        {
            if (_oid != null)
            {
                return _oid;
            }
            lock (oid_lock)
            {
                var authState = AuthenticationStateProvider.GetAuthenticationStateAsync().Result;
                _oid = authState.User.GetNameIdentifier();
                return _oid;
            }
        }

        private bool GetIsAuthenticated()
        {
            if (_isAuthorized != null)
            {
                return _isAuthorized.Value;
            }
            lock (oid_lock)
            {
                var authState = AuthenticationStateProvider.GetAuthenticationStateAsync().Result;
                _isAuthorized = authState.User.Identity.IsAuthenticated;
                return _isAuthorized.Value;
            }
        }

        protected async Task SetAuthorizationToken(bool isAuthorized)
        {
            var key = $"{oid}_{Const.APIPARAM_OPENID_TOKEN}";
            var result = await _dataHubBlazorCoreSessionStorage.GetAsync<string>(SessionStorage, key);
            var token = result.Value;

            if (isAuthorized == true && new Jwt(token).IsExpired())
            {
                var settings = UnityCore.Resolve<JP.DataHub.Com.Settings.OidcSettings>();
                token = await TokenAcquisitionService.GetAccessTokenForUserAsync(new string[] { });
                await _dataHubBlazorCoreSessionStorage.SetAsync(SessionStorage, key, "Bearer " + token);
            }
        }

        protected async Task<string> GetOpenIdTokenNoBearerBySessionStorageAsync()
        {
            return (await GetOpenIdTokenBySessionStorageAsync()).Replace("Bearer ", "");
        }

        protected async Task<string> GetOpenIdTokenBySessionStorageAsync()
        {
            if (oidtoken == null)
            {
                oidtoken = (await SessionStorage.GetAsync<string>($"{oid}_{Const.APIPARAM_OPENID_TOKEN}")).Value;
            }
            return oidtoken;
        }

        protected async Task<Dictionary<string, object>> GetOpenAuthParamAsync()
        {
            var result = new Dictionary<string, object>();
            result.Add(Const.APIPARAM_OPENID_TOKEN, await GetOpenIdTokenBySessionStorageAsync());
            return result;
        }

        protected IDynamicApiClientSelector GetApiCallParam<T>(string key = null, string val = null) where T : IDynamicApiClientSelector
        {
            IDynamicApiClientSelector result = null;
            if (typeof(T) == typeof(ILoginUser))
            {
                result = new LoginUserDynamicApiClientSelector();
            }
            else if (typeof(T) == typeof(ICommon))
            {
                result = new CommonDynamicApiClientSelector();
            }
            if (result != null && key != null)
            {
                result.Param.Add(key, val);
            }
            return result;
        }
    }
}
