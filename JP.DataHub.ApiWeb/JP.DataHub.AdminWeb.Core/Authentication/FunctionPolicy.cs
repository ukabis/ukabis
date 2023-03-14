using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Core.Authentication
{
    public enum FunctionPolicy
    {
        SystemAdministrator,
        Top,
        TopWrite,
        Vendor,
        VendorWrite,
        System,
        SystemWrite,
        SystemAdmin,  // システムのAdmin認証設定
        SystemAdminWrite,
        Api,
        ApiWrite,
        Method,
        MethodWrite,
        Model,
        ModelWrite,
        RepositoryGroup,
        RepositoryGroupWrite,
        VendorRepositoryGroup,
        VendorRepositoryGroupWrite,
        UserInvitationWrite,
    }
}
