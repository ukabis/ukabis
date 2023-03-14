using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace JP.DataHub.ManageApi.Models.ApiWebhook
{
    /// <summary>
    /// HTTPヘッダーViewModel
    /// </summary>
    public class HttpHeaderViewModel
    {
        /// <summary>
        /// フィールド名
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [RegularExpression("[0-9a-zA-Z-]+", ErrorMessage = "半角英数字と\" - \"のみ使用可能です。")]
        public string FieldName { get; set; }

        /// <summary>
        /// 値
        /// </summary>
        [AllowHtml]
        [Required(ErrorMessage = "必須項目です。")]
        public string Value { get; set; }
    }
}