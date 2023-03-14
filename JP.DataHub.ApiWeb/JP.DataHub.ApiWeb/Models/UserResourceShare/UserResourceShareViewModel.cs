using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Models.UserResourceShare
{
    public class UserResourceShareViewModel
    {
        /// <summary>
        /// データ公開設定ID		
        /// </summary>
        [Type(typeof(Guid))]
        public string UserResourceGroupId { get; set; }
        /// <summary>
        /// 自分のOpenId		
        /// </summary>
        [Type(typeof(Guid))]
        public string OpenId { get; set; }
        /// <summary>
        /// リソースグループID		
        /// </summary>
        [Type(typeof(Guid))]
        [Required]
        public string ResourceGroupId { get; set; }
        /// <summary>
        /// 共有指定コード		
        /// nsd : 非共有
        /// stg : グループに共有
        /// uls : 無制限に共有
        /// </summary>
        [Required]
        [MaxLength(3)]
        public string UserShareTypeCode { get; set; }
        /// <summary>
        /// UserグループID		
        /// </summary>
        [Type(typeof(Guid))]
        [Required]
        public string UserGroupId { get; set; }

    }
}
