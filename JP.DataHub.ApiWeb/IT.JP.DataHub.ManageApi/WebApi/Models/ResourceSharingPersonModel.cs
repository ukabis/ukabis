using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    public class ResourceSharingPersonModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ResourceSharingPersonRuleId { get; set; }

        /// <summary>
        /// データ公開対象リソースURL
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// データ共有元ユーザーメールアドレス
        /// </summary>
        public string SharingFromMailAddress { get; set; }

        /// <summary>
        /// データ共有元ユーザーID
        /// </summary>
        public string SharingFromUserId { get; set; }

        /// <summary>
        /// データ共有先ユーザーメールアドレス
        /// </summary>
        public string SharingToMailAddress { get; set; }
        /// <summary>
        /// データ共有先ユーザーID
        /// </summary>
        public string SharingToUserId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ResourceSharingRuleName { get; set; }

        /// <summary>
        /// 条件クエリ
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// スクリプト
        /// </summary>
        public string Script { get; set; }


        /// <summary>
        /// 有効
        /// </summary>
        public bool IsEnable { get; set; } = true;

        /// <summary>
        /// データ所有先ベンダーID
        /// </summary>
        public string SharingToVendorId { get; set; }

        /// <summary>
        /// データ所有先システムID
        /// </summary>
        public string SharingToSystemId { get; set; }
    }
    public class RegisterResourceSharingPersonModel
    {
        /// <summary>
        /// データ公開対象リソースURL
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// データ共有元ユーザーメールアドレス
        /// </summary>
        public string SharingFromMailAddress { get; set; }

        /// <summary>
        /// データ共有元ユーザーID
        /// </summary>
        public string SharingFromUserId { get; set; }

        /// <summary>
        /// データ共有先ユーザーメールアドレス
        /// </summary>
        public string SharingToMailAddress { get; set; }
        /// <summary>
        /// データ共有先ユーザーID
        /// </summary>
        public string SharingToUserId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ResourceSharingRuleName { get; set; }

        /// <summary>
        /// 条件クエリ
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// スクリプト
        /// </summary>
        public string Script { get; set; }


        /// <summary>
        /// 有効
        /// </summary>
        public bool IsEnable { get; set; } = true;

        /// <summary>
        /// データ所有先ベンダーID
        /// </summary>
        public string SharingToVendorId { get; set; }

        /// <summary>
        /// データ所有先システムID
        /// </summary>
        public string SharingToSystemId { get; set; }
    }
}
