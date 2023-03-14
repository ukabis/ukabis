using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JP.DataHub.ManageApi.Models.RepositoryGroup
{
    public class RegisterRepositoryGroupViewModel
    {
        public Guid? RepositoryGroupId { get; set; }

        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, "RepositoryGroup", "repository_group_name")]
        public string RepositoryGroupName { get; set; }

        [Required(ErrorMessage = "必須項目です。")]
        [ForeignKey("RepositoryType")]
        [JpDataHubMaxLength(Domains.DynamicApi, "RepositoryGroup", "repository_type_cd")]
        public string RepositoryTypeCd { get; set; }

        [Required(ErrorMessage = "必須項目です。")]
        public int SortNo { get; set; }

        public bool IsDefault { get; set; } = false;

        public bool IsEnable { get; set; } = true;

        public List<PhysicalRepositoryViewModel> PhysicalRepositoryList { get; set; }
    }
}
