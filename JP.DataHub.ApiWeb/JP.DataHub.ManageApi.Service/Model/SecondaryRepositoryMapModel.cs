using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class SecondaryRepositoryMapModel
    {
        public string SecondaryRepositoryMapId { get; set; }

        public string ApiId { get; set; }

        public string RepositoryGroupId { get; set; }

        public string RepositoryGroupName { get; set; }

        public bool IsContainerDynamicSeparation { get; set; }

        public bool IsActive { get; set; }

        public string RepositoryTypeCd { get; set; }
        public string RepositoryTypeName { get; set; }

        public bool IsRdbmsRepository { get; set; }
    }
}
