using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class UserResourceShareModel
    {
        /// <summary>
        /// データ公開設定ID		
        /// </summary>
        public string UserResourceGroupId { get; set; }
        /// <summary>
        /// 自分のOpenId		
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// リソースグループID		
        /// </summary>
        public string ResourceGroupId { get; set; }
        /// <summary>
        /// 共有指定コード		
        /// nsd : 非共有
        /// stg : グループに共有
        /// uls : 無制限に共有
        /// </summary>
        public string UserShareTypeCode { get; set; }
        public string UserGroupId { get; set; }

    }
    public class UserResourceShareRegistreResponseModel
    {
        public string UserResourceGroupId { get; set; }
    }

}
