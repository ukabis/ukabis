using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class StaticApiModel
    {
        /// <summary>
        /// リソース
        /// </summary>
        public ControllerInformationModel Controller { get; set; }

        /// <summary>
        /// APIリスト
        /// </summary>
        public List<ApiInformationModel> ApiList { get; set; }

        /// <summary>
        /// ソースに存在するかどうか
        /// </summary>
        public bool DetectedInSourceCode { get; set; } = true;
    }
}
