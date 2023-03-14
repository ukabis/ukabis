using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Actions.AllApi
{
    /// <summary>
    /// GetAllApi用
    /// </summary>
    [MessagePackObject]
    internal class AllApiCategoryModel
    {
        [Key(0)]
        public string controller_category_id { get; set; }
        [Key(1)]
        public string controller_id { get; set; }

        [Key(2)]
        public string category_id { get; set; }
        [Key(3)]
        public string category_name { get; set; }

        [Key(4)]
        public DateTime reg_date { get; set; }
        [Key(5)]
        public string reg_username { get; set; }
        [Key(6)]
        public DateTime upd_date { get; set; }
        [Key(7)]
        public string upd_username { get; set; }
        [Key(8)]
        public bool is_active { get; set; }
        [Key(9)]
        public int sort_order { get; set; }
    }
}
