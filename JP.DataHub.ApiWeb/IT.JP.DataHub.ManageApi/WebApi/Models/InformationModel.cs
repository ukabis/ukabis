using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    [Serializable]
    public class InformationModel
    {
        public string InformationId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Date { get; set; }
        public bool IsVisibleApi { get; set; }
        public bool IsVisibleAdmin { get; set; }
    }
}
