
namespace JP.DataHub.ApiWeb.Infrastructure.Models.AzureAdb2c
{
    /// <summary>
    /// Azure ADB2Cに登録するためのユーザーのモデル
    /// </summary>
    internal class RegisterUser
    {
        public bool accountEnabled => true;

        public SignInName[] signInNames { get; set; }

        public string creationType => "LocalAccount";

        public string displayName { get; set; }

        public Password passwordProfile { get; set; }

        public string passwordPolicies => "DisablePasswordExpiration";

        public string systemId { get; set; }
    }
}
