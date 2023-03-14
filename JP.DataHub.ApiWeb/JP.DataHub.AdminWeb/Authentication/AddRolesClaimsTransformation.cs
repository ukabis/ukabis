using JP.DataHub.AdminWeb.Core.Authentication;
using JP.DataHub.AdminWeb.Service.Interface;
using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.AdminWeb.WebAPI.Resources;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Security.Claims;
using Unity;

namespace JP.DataHub.AdminWeb.Authentication
{
    public class AddRolesClaimsTransformation : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var clone = principal.Clone();
            var newIdentity = (ClaimsIdentity)clone.Identity;

            var openId = principal.GetNameIdentifier();

            var commonService = UnityCore.UnityContainer.Resolve<ICommonAdminService>();
            var vendorService = UnityCore.UnityContainer.Resolve<IAdminVendorService>();

            var taskVendor = Task.Run(() =>
            {
                var vendorId = vendorService.GetVendorByOpenId<ICommon>(openId)
                                            .Throw(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, x => "権限の取得に失敗しました。")
                                            .Result
                                            ?.VendorId
                                            .ToString();
                // ベンダーID設定
                if (vendorId != null)
                {
                    newIdentity?.AddClaim(new Claim(AdminClaimTypes.VendorIdentifier, vendorId));
                }
            });
            var taskRole = Task.Run(() =>
            {
                var roles = commonService.GetRoleDetailEx<ICommon>(openId)
                                         .Throw(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, x => "権限の取得に失敗しました。")
                                         .Result;
                // 権限設定
                if (roles?.Any() == true)
                {
                    newIdentity?.AddClaims(
                        roles.Select(role => (new UserAuthority(role)).ToClaim()).Where(claim => claim != null));
                }
            });

            await Task.WhenAll(taskVendor, taskRole);

            return clone;
        }
    }
}
