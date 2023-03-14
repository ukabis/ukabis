using System;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.System
{
    public class SystemAdminModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// 管理者認証キー
        /// </summary>
        public string AdminSecret { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }
}
