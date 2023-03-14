using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class MethodLinkModel : LinkBase
    {
        public Guid MethodId { get; set; }
        public Guid MethodLinkId { get; set; }
    }
}
