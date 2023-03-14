using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiSimpleModel
    {
        /// <summary>
        /// MethodId
        /// </summary>
        public string MethodId { get; set; }

        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// メソッドタイプ
        /// </summary>
        public string MethodType { get; set; }

        /// <summary>
        /// MethodUrl
        /// </summary>
        public string MethodUrl { get; set; }

        /// <summary>
        /// 透過Methodか
        /// </summary>
        public bool IsTransparent { get; set; }

        /// <summary>
        /// 新規登録か(画面制御用)
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 作成またはURL変更された(画面制御用)
        /// </summary>
        public bool IsCreatedOrRenamed { get; set; } = false;
    }
}
