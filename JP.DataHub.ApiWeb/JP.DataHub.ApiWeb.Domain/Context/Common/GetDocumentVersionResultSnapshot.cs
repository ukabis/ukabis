using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal class GetDocumentVersionResultSnapshot
    {
        public string CreateDate { get; set; }
        public string LocationType { get; set; }
        public Guid? RepositoryGroupId { get; set; }
        public Guid? PhysicalRepositoryId { get; set; }
        public string Location { get; set; }
    }
}
