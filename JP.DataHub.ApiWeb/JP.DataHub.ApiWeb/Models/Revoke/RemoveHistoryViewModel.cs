using System;

namespace JP.DataHub.ApiWeb.Models.Revoke
{
    public class RemoveHistoryViewModel
    {
        public string RevokeHistoryId { get; set; }
        public string ControllerId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
