
namespace JP.DataHub.ApiWeb.Infrastructure.Models.AzureAdb2c
{
    /// <summary>
    /// Azure ADB2CのユーザーのSignInName
    /// </summary>
    internal class SignInName
    {
        public string type { get; set; } = "emailAddress";
        public string value { get; set; }
    }
}
