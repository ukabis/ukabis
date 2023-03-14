using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.ApiDescription
{
    public class RegisterStaticApiRequestViewModel
    {
        [Required(ErrorMessage = "必須項目です。")]
        public Guid ApiId { get; set; }
    }
}
