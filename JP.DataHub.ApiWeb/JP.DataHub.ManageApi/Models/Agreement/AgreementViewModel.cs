using JP.DataHub.ManageApi.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.Agreement
{
    public class AgreementViewModel
    {
        [ValidateGuid]
        public string AgreementId { get; set; }

        [Required(ErrorMessage = "必須項目です。")]
        public string Title { get; set; }

        [Required(ErrorMessage = "必須項目です。")]
        public string Detail { get; set; }

        [Required(ErrorMessage = "必須項目です。")]
        public string VendorId { get; set; }
    }
}
