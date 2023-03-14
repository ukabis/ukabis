﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace JP.DataHub.ManageApi.Models.ResourceSharingPerson
{
    public class RegisterResourceSharingPersonViewModel
    {
        /// <summary>
        /// データ公開対象リソースURL
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public string ResourcePath { get; set; }

        /// <summary>
        /// データ共有元ユーザーメールアドレス
        /// </summary>
        public string SharingFromMailAddress { get; set; }

        /// <summary>
        /// データ共有元ユーザーID
        /// </summary>
        public Guid? SharingFromUserId { get; set; }

        /// <summary>
        /// データ共有先ユーザーメールアドレス
        /// </summary>
        public string SharingToMailAddress { get; set; }
        /// <summary>
        /// データ共有先ユーザーID
        /// </summary>
        public Guid? SharingToUserId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public string ResourceSharingRuleName { get; set; }

        /// <summary>
        /// 条件クエリ
        /// </summary>
        [AllowHtml]
        public string Query { get; set; }

        /// <summary>
        /// スクリプト
        /// </summary>
        [AllowHtml]
        public string Script { get; set; }


        /// <summary>
        /// 有効
        /// </summary>
        public bool IsEnable { get; set; } = true;

        /// <summary>
        /// データ所有先ベンダーID
        /// </summary>
        public Guid? SharingToVendorId { get; set; }

        /// <summary>
        /// データ所有先システムID
        /// </summary>
        public Guid? SharingToSystemId { get; set; }
    }
}
