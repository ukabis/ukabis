using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using JP.DataHub.AdminWeb.WebAPI.Models.Api;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    public enum ApiItems
    {
        Url,
        RepositoryKey,
        PartitionKey,
        ResourceCreateDate,
        ResourceLatestDate,
        ApiIpFilterList,
        AttachFileMetaSettings,
        AttachFileBlobSettings,
        HistorySettings
    }

    /// <summary>
    /// API作成パラメタのValidate
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateApiResourceAttribute : ValidationAttribute
    {
        private ApiItems Target { get; set; }

        private ApiResourceInformationModel ViewModel { get; set; }

        public ValidateApiResourceAttribute(ApiItems apiItems)
        {
            this.Target = apiItems;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.ViewModel = validationContext.ObjectInstance as ApiResourceInformationModel;

            var ret = ValidationResult.Success;

            switch (Target)
            {
                case ApiItems.Url:
                    ret = this.ValidateUrl(validationContext);
                    break;
                case ApiItems.RepositoryKey:
                    ret = this.ValidateRepositoryKey(validationContext);
                    break;
                case ApiItems.PartitionKey:
                    ret = this.ValidatePartitionKey(validationContext);
                    break;
                case ApiItems.ResourceCreateDate:
                    ret = this.ValidateResourceCreateDate(validationContext);
                    break;
                case ApiItems.ResourceLatestDate:
                    ret = this.ValidateResourceLatestDate(validationContext);
                    break;
                case ApiItems.ApiIpFilterList:
                    ret = this.ValidateApiIpFilterList(validationContext);
                    break;
                default:
                    break;
            }

            return ret;
        }

        private ValidationResult ValidateUrl(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(this.ViewModel.Url))
            {
                return ValidationResult.Success;
            }

            if (!Regex.IsMatch(this.ViewModel.Url, "^/[-/_a-zA-Z0-9]+[a-zA-Z0-9]$"))
            {
                return new ValidationResult("正しいURLではありません。", new List<string>() { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateRepositoryKey(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(this.ViewModel.RepositoryKey))
            {
                return ValidationResult.Success;
            }

            // 「/」で始まっているか
            if (!this.ViewModel.RepositoryKey.StartsWith("/"))
            {
                return new ValidationResult("リポジトリキーは「/」から始めてください。", new List<string>() { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidatePartitionKey(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(this.ViewModel.PartitionKey))
            {
                return ValidationResult.Success;
            }

            // 「/」で始まっているか
            if (!this.ViewModel.PartitionKey.StartsWith("/"))
            {
                return new ValidationResult("パーティションキーは「/」から始めてください。", new List<string>() { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateResourceCreateDate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(this.ViewModel.ResourceCreateDate))
            {
                return ValidationResult.Success;
            }

            DateTime dt;
            if (!DateTime.TryParse(this.ViewModel.ResourceCreateDate, out dt))
            {
                return new ValidationResult("作成日に入力された内容は日付ではありません。", new List<string>() { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateResourceLatestDate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(this.ViewModel.ResourceLatestDate))
            {
                return ValidationResult.Success;
            }

            DateTime dt;
            if (!DateTime.TryParse(this.ViewModel.ResourceLatestDate, out dt))
            {
                return new ValidationResult("最終更新日に入力された内容は日付ではありません。", new List<string>() { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }

        const string IpAddressPattern = @"^(([1-9]?[0-9]|1[[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([1-9]?[0-9]|1[[0-9]{2}|2[0-4][0-9]|25[0-5])\/([1-9]|1[0-9]|2[0-9]|3[0-2])$";
        private ValidationResult ValidateApiIpFilterList(ValidationContext validationContext)
        {

            if (this.ViewModel.ApiIpFilterList == null)
            {
                return ValidationResult.Success;
            }

            // IPアドレスの重複確認
            var ipAddressCountList = this.ViewModel.ApiIpFilterList.GroupBy(x => x.IpAddress).Select(x => new { IpAddress = x.Key, Count = x.Count() });
            var ipAdrList = ipAddressCountList.Where(x => x.Count > 1);
            if (ipAdrList.Any())
            {
                return new ValidationResult("IPアドレスが重複しています。", new List<string>() { validationContext.DisplayName });
            }
            return ValidationResult.Success;
        }
    }
}