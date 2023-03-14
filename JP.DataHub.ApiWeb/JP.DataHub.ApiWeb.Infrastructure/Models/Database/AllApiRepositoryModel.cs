using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Database
{
    /// <summary>
    /// GetAllApi用
    /// </summary>
    [MessagePackObject]
    public class AllApiRepositoryModel
    {
        [Key(0)]
        public Guid api_id { get; set; }
        [Key(1)]
        public Guid? repository_group_id { get; set; }
        [Key(2)]
        public string repository_type_cd { get; set; }
        [Key(3)]
        public bool is_primary { get; set; }
        [Key(4)]
        public bool is_enable { get; set; }
        [Key(5)]
        public bool is_secondary_primary { get; set; }
        [Key(6)]
        public List<AllApiPhysicalRepositoryModel> physical_repository_list { get; set; }
    }
    public static class AllApiPhysicalRepositoryModelList
    {
        public static List<Tuple<string, bool, Guid?>> ToTuple(this AllApiRepositoryModel list)
        {
            return list.physical_repository_list.Select(x => new Tuple<string, bool, Guid?>(x.repository_connection_string, x.is_full, x.PhysicalRepositoryId)).ToList();
        }
    }
}
