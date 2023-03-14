﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiResourceModel
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
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Apiの名前
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Apiの相対パス
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// Apiの説明
        /// </summary>
        public string ApiDescription { get; set; }

        /// <summary>
        /// このAPIを管理するベンダー
        /// </summary>
        public bool IsVendor { get; set; }

        /// <summary>
        /// 個人依存か
        /// </summary>
        public bool IsPerson { get; set; }

        /// <summary>
        /// StaticApiか
        /// </summary>
        public bool IsStaticApi { get; set; }

        /// <summary>
        /// Topページに表示させるか
        /// </summary>
        public bool IsTopPage { get; set; }

        /// <summary>
        /// ApiのスキーマーID
        /// </summary>
        public string? ApiSchemaId { get; set; }

        /// <summary>
        /// Apiのスキーマー名
        /// </summary>
        public string ApiSchemaName { get; set; }

        /// <summary>
        /// レポジトリーキー
        /// </summary>
        public string RepositoryKey { get; set; }

        /// <summary>
        /// パーティションキー
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// データか
        /// </summary>
        public bool IsData { get; set; }

        /// <summary>
        /// ビジネスロジックか
        /// </summary>
        public bool IsBusinessLogic { get; set; }

        /// <summary>
        /// 有料か
        /// </summary>
        public bool IsPay { get; set; }

        /// <summary>
        /// 使用料
        /// </summary>
        public string FeeDescription { get; set; }

        /// <summary>
        /// リソース作成者
        /// </summary>
        public string ResourceCreateUser { get; set; }

        /// <summary>
        /// メンテナー
        /// </summary>
        public string ResourceMaintainer { get; set; }

        /// <summary>
        /// リソース作成日
        /// </summary>
        public string ResourceCreateDate { get; set; }

        /// <summary>
        /// リソース最終更新日
        /// </summary>
        public string ResourceLatestDate { get; set; }

        /// <summary>
        /// 更新頻度
        /// </summary>
        public string UpdateFrequency { get; set; }

        /// <summary>
        /// 契約が必要か
        /// </summary>
        public bool IsContract { get; set; }

        /// <summary>
        /// 連絡先
        /// </summary>
        public string ContactInfomation { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 利用規約
        /// </summary>
        public string AgreeDescription { get; set; }

        /// <summary>
        /// 規約に同意ボタンを表示するか
        /// </summary>
        public bool IsVisibleAgreement { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 論理削除されているか
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 登録者
        /// </summary>
        public string RegUserName { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdUserName { get; set; }

        /// <summary>
        /// 登録日付
        /// </summary>
        public DateTime RegDate { get; set; }

        /// <summary>
        /// 更新日付
        /// </summary>
        public DateTime UpdDate { get; set; }

        /// <summary>
        /// 楽観排他を使用するかどうか
        /// </summary>
        public bool IsOptimisticConcurrency { get; set; }

        /// <summary>
        /// Methodリスト
        /// </summary>
        public IEnumerable<ApiModel> MethodList { get; set; }

        /// <summary>
        /// DynamicAPIの添付ファイル設定を有効にするかどうか
        /// </summary>
        public bool IsEnableAttachFile { get; set; }

        /// <summary>
        /// DynamicAPIとして管理するデータの履歴を残すかどうか
        /// </summary>
        public bool IsDocumentHistory { get; set; }

        /// <summary>
        /// ブロックチェーンを使用するかどうか
        /// </summary>
        public bool IsEnableBlockchain { get; set; }
        /// <summary>
        /// リソースのバージョンを使用するかどうか
        /// </summary>
        public bool IsEnableResourceVersion { get; set; }

        /// <summary>
        /// 依存をEnumで返却
        /// </summary>
        public DependenctFlags Dependency { 
            get
            {
                if (!IsPerson && !IsVendor) return DependenctFlags.NonDependency;
                if (IsPerson && IsVendor) return DependenctFlags.Double;
                return IsVendor ? DependenctFlags.IsVendor : DependenctFlags.IsPerson;
            } 
        }

        public enum DependenctFlags
        {
            NonDependency = 1,
            IsVendor = 2,
            IsPerson = 4,
            Double = 8
        }
    }
}
