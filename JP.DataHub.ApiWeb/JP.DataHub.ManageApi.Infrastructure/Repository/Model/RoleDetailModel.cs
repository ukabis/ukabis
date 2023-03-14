using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Infrastructure.Repository.Model
{
    internal class RoleDetailModel
    {
        public Guid role_id { get; set; }
        public string role_name { get; set; }
        public string admin_name { get; set; }
        public bool is_read { get; set; }
        public bool is_write { get; set; }
        public Guid admin_func_id { get; set; }
        public Guid admin_func_role_id { get; set; }
        public string display_description { get; set; }
    }
}
