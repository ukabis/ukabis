using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.Models.Api;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredBlobWhenAttachFileEnableAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as ApiResourceAttachFileSettingsModel;

            if (string.IsNullOrEmpty(value as string))
            {
                if (model?.IsEnable == true)
                    return new ValidationResult("添付ファイルを有効にする場合はBlobRepositoryを設定してください。", new List<string>() { validationContext.DisplayName });
            }
            return ValidationResult.Success;
        }
    }
}
