
namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class GroupsModel
    {
        public string groupId { get; set; }
        public List<string> scope { get; set; }
        public string groupName { get; set; }
        public GroupsMemberModel representativeMember { get; set; }
        public List<GroupsMemberModel> member { get; set; }
        public List<string> manager { get; set; }
        public string CompanyId { get; set; }
    }
    public class GroupsMemberModel
    {
        public string openId { get; set; }
        public string mailAddress { get; set; }
        public List<string> accessControl { get; set; }
    }
    public class IsGroupMemberModel
    {
        public bool isGroupMember { get; set; }
    }
}
