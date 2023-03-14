using System;

namespace JP.DataHub.ApiWeb.Models.Revoke
{
    public class UserRevokeViewModel
    {
        public string UserRevokeId { get; set; }
        public string UserTermsId { get; set; }
        public string TermsId { get; set; }
        public bool IsFinish { get; set; }
        public string OpenId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
