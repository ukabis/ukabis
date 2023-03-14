using JP.DataHub.AdminWeb.Core.Authentication;
using JP.DataHub.AdminWeb.Service.Interface;
using JP.DataHub.Blazor.Core.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Web;
using Microsoft.JSInterop;
using Radzen;

namespace JP.DataHub.AdminWeb.Core.Component.Pages
{
    public abstract class AdminWebBasePage : BlazorBasePage
    {
        [Inject]
        protected ICommonAdminService CommonService { get; set; }

        [Inject]
        protected Radzen.DialogService DialogService { get; set; }

        [Inject]
        protected Radzen.NotificationService NotificationService { get; set; }

        [Inject]
        protected IJSRuntime JS { get; set; }

        protected virtual bool AuthenticationRequired { get; } = true;

        protected override async Task OnInitializedAsync()
        {
            if (AuthenticationRequired && RequireAuthenticationIfYet())
            {
                return;
            }

            // 認証しているか？
            bool fail = false;
            try
            {
                var isAuthorized = this.IsAuthenticated;
                await SetAuthorizationToken(isAuthorized);
                if (string.IsNullOrEmpty(await GetOpenIdTokenBySessionStorageAsync()) == true)
                {
                    fail = true;
                }
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                NavManager.NavigateTo("/MicrosoftIdentity/Account/SignOut", true);
                return;
            }
            catch
            {
                fail = true;
            }
            if (fail == true)
            {
                NavManager.NavigateTo("/", false);
                return;
            }

            await SetAuthorizationToken(this.IsAuthenticated);



            await OnAfterInitializeAsync();
        }

        /// <summary>
        /// 共通処理(認証チェック等)後に呼び出されるメソッド。
        /// 各ページはこのメソッドをoverrideして初期化を行う。
        /// </summary>
        /// <returns></returns>
        protected virtual async Task OnAfterInitializeAsync()
        {
        }

        protected bool IsSystemAdministrator()
        {
            var authState = AuthenticationStateProvider.GetAuthenticationStateAsync().Result;
            var user = authState.User;
            if (bool.TryParse(user.FindFirst(AdminClaimTypes.IsSystemAdministrator)?.Value, out var isSystemAdministrator))
            {
                return isSystemAdministrator;
            }
            return false;
        }

        protected async Task CopyToClipboard(string text)
        {
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", text ?? "");
            NotificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Success,
                Summary = "コピーしました"
            });
        }
    }
}
