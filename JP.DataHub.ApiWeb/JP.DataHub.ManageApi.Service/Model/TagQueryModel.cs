using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class TagQueryModel
    {
        public TagQueryModel()
        {
            Children = new List<TagQueryModel>();
        }

        public string TagId { get; }

        public string TagCode { get; }

        public string TagCode2 { get; }

        public string TagName { get; }

        public string ParentTagId { get; }

        public string TagTypeId { get; }

        public string TagTypeName { get; }

        public string TagTypeDetail { get; }

        public IList<TagQueryModel> Children { get; internal set; }
    }
}
