using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class DocumentFileModel
    {
        public string FileId { get; set; }
        public string DocumentId { get; set; }
        public string DocumentTitle { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsEnable { get; set; }
        public DateTime UpdDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsFileUpDate { get; set; }
        public string HtmlLink { get; set; }
        public int OrderNo { get; set; }
        public DocumentFileChangeInfo DocumentFileChangeInfo { get; set; }
    }

    public enum DocumentFileChangeInfo
    {
        Unknown,
        NoChange,
        Create,
        Update,
        Delete
    }
}
