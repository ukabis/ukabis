using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.Model
{
    /// <summary>
    /// TestServiceで利用するモデルクラス
    /// このモデルはキャッシュに入れたりするので２つの属性は必須
    /// </summary>
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class TestModel
    {
        //[VendorId]
        public string VendorId { get; set; }
        //[SystemId]
        public string SystemId { get; set; }
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
