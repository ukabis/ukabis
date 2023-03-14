using JP.DataHub.AdminWeb.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Core.Misc
{
    public static class AdminFunc
    {
        public const string TOP = "DI_002";

        public const string VENDOR = "DI_011";

        public const string SYSTEM = "DI_012";

        public const string SYSTEM_ADMIN = "DI_050";

        public const string API = "DI_021";

        public const string METHOD = "DI_022";

        public const string MODEL = "DI_023";

        public const string REPOSITORY_GROUP = "DI_051";

        public const string VENDOR_REPOSITORY_GROUP = "DI_042";

        public const string USER_INVITATION = "DI_013";

        public static string GetAdminFunc(FunctionPolicy policy)
        {
            switch (policy)
            {
                case FunctionPolicy.Top:
                case FunctionPolicy.TopWrite:
                    return TOP;
                case FunctionPolicy.Vendor:
                case FunctionPolicy.VendorWrite:
                    return VENDOR;
                case FunctionPolicy.System:
                case FunctionPolicy.SystemWrite:
                    return SYSTEM;
                case FunctionPolicy.SystemAdmin:
                case FunctionPolicy.SystemAdminWrite:
                    return SYSTEM_ADMIN;
                case FunctionPolicy.Api:
                case FunctionPolicy.ApiWrite:
                    return API;
                case FunctionPolicy.Method:
                case FunctionPolicy.MethodWrite:
                    return METHOD;
                case FunctionPolicy.Model:
                case FunctionPolicy.ModelWrite:
                    return MODEL;
                case FunctionPolicy.RepositoryGroup:
                case FunctionPolicy.RepositoryGroupWrite:
                    return REPOSITORY_GROUP;
                case FunctionPolicy.VendorRepositoryGroup:
                case FunctionPolicy.VendorRepositoryGroupWrite:
                    return VENDOR_REPOSITORY_GROUP;
                case FunctionPolicy.UserInvitationWrite:
                    return USER_INVITATION;
                default:
                    return string.Empty;
            };
        }
    }
}
