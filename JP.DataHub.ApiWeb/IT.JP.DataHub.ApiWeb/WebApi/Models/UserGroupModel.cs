using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class UserGroupModel
    {
        /// <summary>
        /// ユーザーグループID		
        /// </summary>
        public string UserGroupId { get; set; }

        /// <summary>
        /// ユーザーグループ名		
        /// </summary>
        public string UserGroupName { get; set; }

        /// <summary>
        /// グループに参加しているメンバー		
        /// </summary>
        public List<string> Members { get; set; }
    }
    public class UserGroupRegisterResponseModel
    {
        public string UserGroupId { get; set; }
    }
}
