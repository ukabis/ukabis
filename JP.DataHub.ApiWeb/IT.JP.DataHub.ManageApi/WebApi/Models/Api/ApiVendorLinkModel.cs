using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Api
{
    /// <summary>
    /// リンク情報の基底クラス
    /// </summary>
    public class LinkBase
    {
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Url { get; set; }
        public bool IsVisible { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
    public class ApiVendorLinkModel : LinkBase
    {
        public string VendorLinkId { get; set; }
        public string VendorId { get; set; }
    }
    public class ApiDescriptionSystemLinkModel : LinkBase
    {
        public string SystemLinkId { get; set; }
        public string SystemId { get; set; }
    }
}
