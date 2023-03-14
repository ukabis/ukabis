using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Models.UserGroup
{
    public class UserGroupViewModel
    {
        /// <summary>
        /// ユーザーグループID		
        /// </summary>
        [Type(typeof(Guid))]
        public string UserGroupId { get; set; }

        /// <summary>
        /// ユーザーグループ名		
        /// </summary>
        [Required]
        public string UserGroupName { get; set; }

        /// <summary>
        /// グループに参加しているメンバー		
        /// </summary>
        [Required]
        public IList<string> Members { get; set; }
    }
}
