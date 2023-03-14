using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    /// <summary>
    /// 個人リソースシェアリングルールのエンティティ
    /// </summary>
    internal record ResourceSharingPersonRule : IValueObject
    {
        /// <summary>個人リソースシェアリングルールID</summary>
        public ResourceSharingPersonRuleId ResourceSharingPersonRuleId { get; }

        /// <summary>ルール名</summary>
        public string ResourceSharingRuleName { get; }

        /// <summary>共有対象のリリースパス</summary>
        public RelativeUri ResourcePath { get; }

        /// <summary>共有元ユーザーID</summary>
        public OpenId SharingFromUserId { get; }

        /// <summary>共有元メールアドレス</summary>
        public string SharingFromMailAddress { get; }

        /// <summary>共有先ユーザーID</summary>
        public OpenId SharingToUserId { get; }

        /// <summary>共有先メールアドレス</summary>
        public string SharingToMailAddress { get; }

        /// <summary>共有先ベンダーID</summary>
        public Guid? SharingToVendorId { get; }
        /// <summary>共有先システムID</summary>
        public Guid? SharingToSystemId { get; }

        /// <summary>共有条件のクエリー</summary>
        public ApiQuery Query { get; }

        /// <summary>共有条件を生成するスクリプト</summary>
        public Script Script { get; }

        /// <summary>有効フラグ</summary>
        public IsActive IsActive { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="resourceSharingPersonRuleId">個人リソースシェアリングルールID</param>
        /// <param name="resourceSharingRuleName">ルール名</param>
        /// <param name="resourcePath">共有対象のリリースパス</param>
        /// <param name="sharingFromUserId">共有元ユーザーID</param>
        /// <param name="sharingFromMailAddress">共有元メールアドレス</param>
        /// <param name="sharingToUserId">共有先ユーザーID</param>
        /// <param name="sharingToMailAddress">共有先メールアドレス</param>
        /// <param name="query">共有条件のクエリー</param>
        /// <param name="script">共有条件を生成するスクリプト</param>
        /// <param name="isActive">有効フラグ</param>
        /// <param name="sharingToVendorId">共有先ベンダーID</param>
        /// <param name="sharingToSystemId">共有先システムID</param>
        public ResourceSharingPersonRule(ResourceSharingPersonRuleId resourceSharingPersonRuleId,
                                         string resourceSharingRuleName,
                                         RelativeUri resourcePath,
                                         OpenId sharingFromUserId,
                                         string sharingFromMailAddress,
                                         OpenId sharingToUserId,
                                         string sharingToMailAddress,
                                         ApiQuery query,
                                         Script script,
                                         IsActive isActive,
                                         Guid? sharingToVendorId = null,
                                         Guid? sharingToSystemId = null)
        {
            ResourceSharingPersonRuleId = resourceSharingPersonRuleId;
            ResourceSharingRuleName = resourceSharingRuleName;
            ResourcePath = resourcePath ?? throw new ArgumentNullException(nameof(resourcePath));
            SharingFromUserId = sharingFromUserId;
            SharingFromMailAddress = sharingFromMailAddress;
            SharingToUserId = sharingToUserId;
            SharingToMailAddress = sharingToMailAddress;
            Query = query;
            Script = script;
            IsActive = isActive ?? throw new ArgumentNullException(nameof(isActive));
            SharingToVendorId = sharingToVendorId;
            SharingToSystemId = sharingToSystemId;

            //片方しか指定がない
            if (sharingToVendorId != null ^ sharingToSystemId != null)
            {
                throw new ArgumentException("ベンダーの指定が不完全です");
            }

            //ベンダーへの共有指定なのに fromかtoにユーザーの指定がある
            if (sharingToVendorId != null && sharingToSystemId != null
                && (!(sharingFromMailAddress == "*" && string.IsNullOrEmpty(sharingFromUserId?.Value))
                    || !(sharingToMailAddress == "*" && string.IsNullOrEmpty(sharingToUserId?.Value))))
            {
                throw new ArgumentException("ベンダー指定の共有では共有元先にユーザーは指定できません");
            }
        }

        public static bool operator ==(ResourceSharingPersonRule me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ResourceSharingPersonRule me, object other) => !me?.Equals(other) == true;
    }
}