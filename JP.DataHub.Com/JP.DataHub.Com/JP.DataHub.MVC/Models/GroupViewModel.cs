using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Models
{
    public class GroupViewModel
    {
        /// <summary>
        /// ★グループID
        /// </summary>
        public string groupId { get; set; }
        /// <summary>
        /// 適用範囲(All, SmartFoodChain, Sensor, FMIS)
        /// </summary>
        public List<string> scope { get; set; }
        /// <summary>
        /// グループ名
        /// </summary>
        public string groupName { get; set; }
        /// <summary>
        /// 代表者
        /// </summary>
        public GroupMemberViewModel representativeMember { get; set; }
        /// <summary>
        /// メンバー
        /// </summary>
        public List<GroupMemberViewModel> member { get; set; }
        /// <summary>
        /// 管理者のOpenId
        /// </summary>
        public List<string> manager { get; set; }
        /// <summary>
        /// 事業者ID
        /// </summary>
        public string CompanyId { get; set; }

        public List<OwnGroupViewModel> OwnGroupList { get; set; }
    }

    public class GroupMemberViewModel
    {
        /// <summary>
        /// OpenID
        /// </summary>
        public string openId { get; set; }
        /// <summary>
        /// メールアドレス
        /// </summary>
        public string mailAddress { get; set; }
        /// <summary>
        /// アクセスコントロール (Read, ReadWrite, Nothing)
        /// </summary>
        public List<string> accessControl { get; set; }
    }

    public class OwnGroupViewModel
    {
        public string groupId { get; set; }
        public string groupName { get; set; }
        public bool isAdmin { get; set; }
    }
}
