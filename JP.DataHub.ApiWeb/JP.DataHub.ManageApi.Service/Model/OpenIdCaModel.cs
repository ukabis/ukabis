using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class OpenIdCaModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// アプリケーション名
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// アクセス制御（alw:許可 / dny:拒否 / inh:継承）
        /// </summary>
        public string AccessControl { get; set; }
    }
}
