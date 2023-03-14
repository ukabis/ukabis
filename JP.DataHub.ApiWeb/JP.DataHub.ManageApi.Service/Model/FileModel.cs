using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class FileModel
    {
        public string FileId { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public bool IsEnable { get; set; }

        public DateTime UpdDate { get; set; }

        public bool IsActive { get; set; }

        public int OrderNo { get; set; }
    }
}
