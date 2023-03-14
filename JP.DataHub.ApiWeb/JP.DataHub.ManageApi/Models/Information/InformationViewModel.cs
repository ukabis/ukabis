using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using System;
using System.ComponentModel.DataAnnotations;
namespace JP.DataHub.ManageApi.Models.Information
{
    public class InformationViewModel
    {
        [ValidateGuid]
        public string InformationId { get; set; }
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Information, "Information", "title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Information, "Information", "detail")]
        public string Detail { get; set; }
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateInformationAttribute.IsValidInformationDate]
        public string Date { get; set; }
        public bool IsVisibleApi { get; set; }
        public bool IsVisibleAdmin { get; set; }
    }

}
