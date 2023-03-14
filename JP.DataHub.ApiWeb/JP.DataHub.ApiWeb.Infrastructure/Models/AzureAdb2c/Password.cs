
namespace JP.DataHub.ApiWeb.Infrastructure.Models.AzureAdb2c
{
    /// <summary>
    /// Azure ADB2CのユーザーのPasswordProfile
    /// </summary>
    internal class Password
    {
        public string password { get; set; }
        public bool forceChangePasswordNextLogin { get; set; } = false;
    }
}
