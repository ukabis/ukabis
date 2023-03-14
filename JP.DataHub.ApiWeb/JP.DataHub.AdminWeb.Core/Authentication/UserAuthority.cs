using JP.DataHub.AdminWeb.WebAPI.Models;
using System.Security.Claims;

namespace JP.DataHub.AdminWeb.Core.Authentication
{
    public class UserAuthority
    {
        public string FuncName { get; }

        public bool CanRead { get; }

        public bool CanWrite { get; }

        public UserAuthority(RoleDetailModel roleDetail)
        {
            FuncName = roleDetail.FuncName;
            CanRead = roleDetail.IsRead;
            CanWrite = roleDetail.IsWrite;
        }

        public Claim? ToClaim()
        {
            if (CanWrite)
            {
                return new Claim(FuncName, $"Write");
            }
            else if (CanRead)
            {
                return new Claim(FuncName, "Read");
            }

            return null;
        }
    }
}
