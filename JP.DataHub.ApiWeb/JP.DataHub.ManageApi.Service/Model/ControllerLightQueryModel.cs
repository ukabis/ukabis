using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class ControllerLightQueryModel
    {
        /// <summary>
        /// ControllerのID
        /// </summary>
        public string ControllerId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// Controllerの相対パス
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// このAPIを管理するベンダー
        /// </summary>
        public bool IsVendor { get; set; }

        /// <summary>
        /// 個人依存か
        /// </summary>
        public bool IsPerson { get; set; }

        /// <summary>
        /// コントローラーのスキーマーID
        /// </summary>
        public string ControllerSchemaId { get; set; }

        /// <summary>
        /// リポジトリグループID(メソッドのリポジトリグループが複数ある場合はnull)
        /// </summary>
        public string RepositoryGroupId { get; set; }
    }
}
