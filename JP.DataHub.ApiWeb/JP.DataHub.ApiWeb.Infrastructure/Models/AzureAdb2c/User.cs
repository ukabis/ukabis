using System;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.AzureAdb2c
{
    /// <summary>
    /// Azure ADB2Cに登録されたユーザーのモデル
    /// </summary>
    internal class User
    {
        public string objectId { get; set; }
        public SignInName[] signInNames { get; set; }
        public string[] otherMails { get; set; }
        public string displayName { get; set; }
        public Password passwordProfile { get; set; }
        public string systemId { get; set; }
        public DateTime? createdDateTime { get; set; }
    }
}
