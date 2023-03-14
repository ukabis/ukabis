using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using JP.DataHub.Com.Validations.Annotations.Attributes;

namespace JP.DataHub.ApiWeb.Models.ResourceGroup
{
    public class ResourceGroupViewModel
    {
        [Type(typeof(Guid))]
        public string ResourceGroupId { get; set; }
        [Required]
        public string ResourceGroupName { get; set; }
        [Required]
        public string TermsGroupCode { get; set; }
        public bool IsRequireConsent { get; set; }
        public IList<ResourceGroupInResourceViewMode> Resources { get; set; }
    }

    public class ResourceGroupInResourceViewMode
    {
        [Required]
        public string ControllerId { get; set; }
        public string ControllerUrl { get; set; }
        public bool IsPerson { get; set; }
    }
}
