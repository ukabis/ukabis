using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ApiLightViewModel
    {
        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// Apiの相対パス
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
        /// ApiのスキーマーID
        /// </summary>
        public string ApiSchemaId { get; set; }

        /// <summary>
        /// リポジトリグループID(メソッドのリポジトリグループが複数ある場合はnull)
        /// </summary>
        public string RepositoryGroupId { get; set; }
    }
}
