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
    public class DocumentCategoryModel
    {
        public string CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
