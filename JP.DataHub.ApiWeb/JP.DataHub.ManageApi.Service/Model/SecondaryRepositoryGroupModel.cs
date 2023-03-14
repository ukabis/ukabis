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
    public class SecondaryRepositoryGroupModel
    {
        public Guid SecondaryRepositoryId { get; set; }
        public Guid MethodId { get; set; }
        public bool IsEnable { get; set; }
    }
}
