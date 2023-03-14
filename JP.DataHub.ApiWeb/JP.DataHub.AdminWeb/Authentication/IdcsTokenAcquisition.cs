using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace JP.DataHub.AdminWeb.Authentication
{
    public class IdcsTokenAcquisition : ITokenAcquisition
    {
        public string AccessToken { get; set; }
        public string IdToken { get; set; }

        public Task<string> GetAccessTokenForAppAsync(string scope, string? authenticationScheme, string? tenant = null, TokenAcquisitionOptions? tokenAcquisitionOptions = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccessTokenForUserAsync(IEnumerable<string> scopes, string? authenticationScheme, string? tenantId = null, string? userFlow = null, ClaimsPrincipal? user = null, TokenAcquisitionOptions? tokenAcquisitionOptions = null)
        {
            return Task.FromResult(AccessToken);
        }

        public Task<AuthenticationResult> GetAuthenticationResultForAppAsync(string scope, string? authenticationScheme, string? tenant = null, TokenAcquisitionOptions? tokenAcquisitionOptions = null)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationResult> GetAuthenticationResultForUserAsync(IEnumerable<string> scopes, string? authenticationScheme, string? tenantId = null, string? userFlow = null, ClaimsPrincipal? user = null, TokenAcquisitionOptions? tokenAcquisitionOptions = null)
        {
            throw new NotImplementedException();
        }

        public string GetEffectiveAuthenticationScheme(string? authenticationScheme)
        {
            throw new NotImplementedException();
        }

        public void ReplyForbiddenWithWwwAuthenticateHeader(IEnumerable<string> scopes, MsalUiRequiredException msalServiceException, string? authenticationScheme, HttpResponse? httpResponse = null)
        {
            throw new NotImplementedException();
        }

        public Task ReplyForbiddenWithWwwAuthenticateHeaderAsync(IEnumerable<string> scopes, MsalUiRequiredException msalServiceException, HttpResponse? httpResponse = null)
        {
            throw new NotImplementedException();
        }
    }
}
