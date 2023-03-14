using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class SecondaryRepositoryMapQueryModel
    {
        /// <summary>
        /// SecondaryRepositoryMapId
        /// </summary>
        public string SecondaryRepositoryMapId { get; set; }

        /// <summary>
        /// RepositoryGroupId
        /// </summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>
        /// RepositoryGroupName
        /// </summary>
        public string RepositoryGroupName { get; set; }

        /// <summary>
        /// IsPrimary
        /// </summary>
        public bool IsPrimary { get; set; }
    }
}
