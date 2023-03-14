using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class FunctionNodeModel
    {
        public string FunctionName { get; set; }

        public string AbsoluteUrl { get; set; }

        public bool Authrize { get; set; }

        public List<FunctionNodeModel> ChildList { get; set; }
    }
}
