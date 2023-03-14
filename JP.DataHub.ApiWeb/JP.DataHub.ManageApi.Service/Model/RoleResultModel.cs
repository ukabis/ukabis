using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class RoleResultModel
    {
        [Required]
        public Guid RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
