using JP.DataHub.ManageApi.Models.DynamicApi;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JP.DataHub.ManageApi.Attributes
{
    public enum ApiItems
    {
        RepositoryKey,
        PartitionKey,
        ResourceCreateDate,
        ResourceLatestDate,
        ApiIpFilterList,
        AttachFileSettings,
        HistorySettings
    }

    /// <summary>
    /// API作成パラメタのValidate
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateApiAttribute : ValidationAttribute
    {
        private ApiItems Target { get; set; }

        private RegisterApiRequestViewModel ViewModel { get; set; }

        public ValidateApiAttribute(ApiItems apiItems)
        {
            this.Target = apiItems;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.ViewModel = (RegisterApiRequestViewModel)validationContext.ObjectInstance;

            var ret = ValidationResult.Success;

            switch (Target)
            {
                case ApiItems.RepositoryKey:
                    ret = this.ValidateRepositoryKey();
                    break;

                case ApiItems.PartitionKey:
                    ret = this.ValidatePartitionKey();
                    break;
                case ApiItems.ResourceCreateDate:
                    ret = this.ValidateResourceCreateDate();
                    break;
                case ApiItems.ResourceLatestDate:
                    ret = this.ValidateResourceLatestDate();
                    break;
                case ApiItems.ApiIpFilterList:
                    ret = this.ValidateApiIpFilterList();
                    break;
                case ApiItems.AttachFileSettings:
                    ret = this.ValidateAttachFileSettings();
                    break;
                case ApiItems.HistorySettings:
                    ret = this.ValidateHistorySettings();
                    break;
                default:
                    break;
            }

            return ret;
        }

        private ValidationResult ValidateRepositoryKey()
        {
            if (string.IsNullOrEmpty(this.ViewModel.RepositoryKey))
            {
                return ValidationResult.Success;
            }

            // 「/」で始まっているか
            if (!this.ViewModel.RepositoryKey.StartsWith("/"))
            {
                return new ValidationResult("リポジトリキーは「/」から始めてください。");
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidatePartitionKey()
        {
            if (string.IsNullOrEmpty(this.ViewModel.PartitionKey))
            {
                return ValidationResult.Success;
            }

            // 「/」で始まっているか
            if (!this.ViewModel.PartitionKey.StartsWith("/"))
            {
                return new ValidationResult("パーティションキーは「/」から始めてください。");
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateResourceCreateDate()
        {
            if (string.IsNullOrEmpty(this.ViewModel.ResourceCreateDate))
            {
                return ValidationResult.Success;
            }

            DateTime dt;
            if (!DateTime.TryParse(this.ViewModel.ResourceCreateDate, out dt))
            {
                return new ValidationResult("入力された内容は日付ではありません。");
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateResourceLatestDate()
        {
            if (string.IsNullOrEmpty(this.ViewModel.ResourceLatestDate))
            {
                return ValidationResult.Success;
            }

            DateTime dt;
            if (!DateTime.TryParse(this.ViewModel.ResourceLatestDate, out dt))
            {
                return new ValidationResult("入力された内容は日付ではありません。");
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateApiIpFilterList()
        {
            if (this.ViewModel.ApiIpFilterList == null)
            {
                return ValidationResult.Success;
            }

            // IPアドレスの重複確認
            for (int i = 0; i < this.ViewModel.ApiIpFilterList.Count; i++)
            {
                if (string.IsNullOrEmpty(this.ViewModel.ApiIpFilterList[i].IpAddress))
                {
                    return new ValidationResult("IPアドレスは入力必須です。");
                }
            }
            var ipAddressCountList = this.ViewModel.ApiIpFilterList.GroupBy(x => x.IpAddress).Select(x => new { IpAddress = x.Key, Count = x.Count() });
            var ipAdrList = ipAddressCountList.Where(x => x.Count > 1);
            if (ipAdrList.Any())
            {
                for (int i = 0; i < this.ViewModel.ApiIpFilterList.Count; i++)
                {
                    if (ipAdrList.Any(x => x.IpAddress == this.ViewModel.ApiIpFilterList[i].IpAddress))
                    {
                        return new ValidationResult("IPアドレスが重複しています。");
                    }
                }
            }
            return ValidationResult.Success;
        }
        private ValidationResult ValidateAttachFileSettings()
        {
            var isAttachFileEnable = this.ViewModel?.AttachFileSettings?.IsEnable;
            if (isAttachFileEnable != null && this.ViewModel.AttachFileSettings.IsEnable)
            {
                if (string.IsNullOrEmpty(this.ViewModel.AttachFileSettings.MetaRepositoryId))
                {
                    return new ValidationResult("添付ファイルを有効にする場合はMetaRepositoryIdを指定してください。");
                }
            }

            return ValidationResult.Success;
        }
        private ValidationResult ValidateHistorySettings()
        {
            if (this.ViewModel?.DocumentHistorySettings?.IsEnable == true)
            {
                if (string.IsNullOrEmpty(this.ViewModel.DocumentHistorySettings.HistoryRepositoryId))
                {
                    return new ValidationResult("履歴機能を有効にする場合はHistoryRepositoryIdを指定してください。");
                }
            }

            return ValidationResult.Success;
        }
    }
}
