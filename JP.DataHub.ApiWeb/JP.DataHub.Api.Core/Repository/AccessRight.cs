using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.Api.Core.Repository
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class AccessRight
    {
        public string FunctionName { get; set; }
        public bool IsWrite { get; set; }
        public bool IsRead { get; set; }
    }
}
