using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class DynamicApiAttachFileStorageModel
    {
        /// <summary>
        /// リポジトリグループID
        /// </summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>
        /// リポジトリグループ名
        /// </summary>
        public string RepositoryGroupName { get; set; }
    }
}
