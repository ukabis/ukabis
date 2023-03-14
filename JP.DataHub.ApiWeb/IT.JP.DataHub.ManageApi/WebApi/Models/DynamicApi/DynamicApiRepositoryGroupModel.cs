using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class DynamicApiRepositoryGroupModel
    {
        public string RepositoryGroupId { get; set; }

        public string RepositoryGroupName { get; set; }

        public string RepositoryTypeCd { get; set; }

        public string RepositoryTypeName { get; set; }

        public bool IsEnable { get; set; }
    }
}
