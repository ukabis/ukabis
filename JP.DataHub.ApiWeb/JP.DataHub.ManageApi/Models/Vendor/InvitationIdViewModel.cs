namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class InvitationIdViewModel
    {
        /// <summary>
        /// 招待ID
        /// </summary>
        public string InvitationId { get; set; }

        public InvitationIdViewModel(string invitationId)
        {
            InvitationId = invitationId;
        }
    }
}
