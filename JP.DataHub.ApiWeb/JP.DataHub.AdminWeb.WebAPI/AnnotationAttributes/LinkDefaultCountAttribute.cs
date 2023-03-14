using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.Models;

namespace JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class LinkDefaultCountAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<SystemLinkModel> systemLinkList;
            List<VendorLinkModel> vendorLinkList;

            var errMsg = "リンクのデフォルトは１つのみ設定してください";

            if (value is List<SystemLinkModel>)
            {
                systemLinkList = value as List<SystemLinkModel>;

                if (systemLinkList != null && systemLinkList.Any())
                {
                    // デフォルトは１件のみ登録可能
                    if (systemLinkList.Where(x => x.IsVisible && x.IsDefault).Count() > 1)
                    {
                        return new ValidationResult(errMsg, new List<string>() { validationContext.DisplayName });
                    }
                }
            }
            else if (value is List<VendorLinkModel>)
            {
                vendorLinkList = value as List<VendorLinkModel>;

                if (vendorLinkList != null && vendorLinkList.Any())
                {
                    // デフォルトは１件のみ登録可能
                    if (vendorLinkList.Where(x => x.IsVisible && x.IsDefault).Count() > 1)
                    {
                        return new ValidationResult(errMsg, new List<string>() { validationContext.DisplayName });
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
