using JP.DataHub.AdminWeb.WebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    public enum MethodItems
    {
        MethodUrl,
        CacheMinute,
        GatewayUrl,
        Script,
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateMethodAttribute : ValidationAttribute
    {
        private MethodItems Target { get; set; }

        private ApiModel ViewModel { get; set; } = new();

        public ValidateMethodAttribute(MethodItems apiItems)
        {
            this.Target = apiItems;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.ViewModel = validationContext.ObjectInstance as ApiModel;

            var ret = ValidationResult.Success;

            switch (Target)
            {
                case MethodItems.MethodUrl:
                    ret = this.ValidateMethodUrl(validationContext);
                    break;
                case MethodItems.CacheMinute:
                    ret = this.ValidateCacheMinute(validationContext);
                    break;
                case MethodItems.GatewayUrl:
                    ret = this.ValidateGatewayUrl(validationContext);
                    break;
                case MethodItems.Script:
                    ret = this.ValidateScript(validationContext);
                    break;
                default:
                    break;
            }

            return ret;
        }

        private ValidationResult ValidateMethodUrl(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ViewModel.MethodUrl))
            {
                return ValidationResult.Success;
            }

            // パラメータが重複していないかどうかチェックする
            var parameters = (new Regex(@"{[-_0-9a-zA-Z]+}")).Matches(ViewModel.MethodUrl)
                                                             .Cast<Match>()
                                                             .Select(x => x.Value.Replace("{", string.Empty).Replace("}", string.Empty));

            if(parameters?.GroupBy(x => x).Any(x => x.Count() > 1) != true)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("パラメータが重複しています。", new List<string>() { validationContext.DisplayName });
            }
        }

        private ValidationResult ValidateCacheMinute(ValidationContext validationContext)
        {
            if (ViewModel.IsCache && ViewModel.ActionTypeCd != "del")
            {
                if (ViewModel.CacheMinute < 1 || ViewModel.CacheMinute > int.MaxValue)
                {
                    return new ValidationResult($"1以上{int.MaxValue}以内の文字数で入力してください。", new List<string>() { validationContext.DisplayName });
                }
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateGatewayUrl(ValidationContext validationContext)
        {
            if (ViewModel.ActionTypeCd == "gtw" && string.IsNullOrEmpty(ViewModel.GatewayUrl))
            {
                return new ValidationResult("ゲートウェイURLは必須項目です。", new List<string>() { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateScript(ValidationContext validationContext)
        {
            // スクリプト設定時はスクリプトタイプの設定必須
            if (!string.IsNullOrEmpty(ViewModel.Script) && string.IsNullOrEmpty(ViewModel.ScriptTypeCd))
            {
                return new ValidationResult("スクリプトタイプは必須項目です。", new List<string>() { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }
    }
}
