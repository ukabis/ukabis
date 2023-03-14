using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Misc;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    [MessagePackObject]
    public class ApiTreeNode
    {
        [Key(0)]
        public string RelativePath { get; set; }
        [Key(1)]
        public List<ApiTreeNode> NextNode { get; set; } = new List<ApiTreeNode>();
        [Key(2)]
        public List<AllApiEntityIdentifier> Api { get; set; } = new List<AllApiEntityIdentifier>();

        public void AddApiEntity(AllApiEntityIdentifier api)
        {
            string[] splitUrl = UriUtil.GetUrlNonQuery(api.controller_relative_url).Split("/".ToCharArray());
            ComputeNode(splitUrl, 0, api);
        }

        public void Remove(Guid api_id)
        {
            _Remove(this, api_id);
        }

        public static void _Remove(ApiTreeNode node, Guid api_id)
        {
            if (node.Api != null && node.Api.Count > 0)
            {
                int before = node.Api.Count;
                node.Api.Where(x => x.api_id == api_id).ToList().ForEach(x => node.Api.Remove(x));
                int after = node.Api.Count;
            }
            if (node.NextNode != null && node.NextNode.Count > 0)
            {
                node.NextNode.ForEach(x => _Remove(x, api_id));
            }
        }

        public AllApiEntityIdentifier FindApiIdentifier(string normalizedRelativeUri, string[] splitUrl, HttpMethodType httpMethodType, GetQuery getQuery)
        {
            ApiTreeNode tmp = this;
            for (int i = 1; i < splitUrl.Length; i++)
            {
                if (tmp == null)
                {
                    break;
                }
                var hit = tmp.NextNode.Where(x => x.RelativePath == splitUrl[i]).FirstOrDefault();
                if (hit?.Api != null && hit?.Api.Count > 0)
                {
                    foreach (var api in hit.Api)
                    {
                        if (api.IsMatch(httpMethodType, normalizedRelativeUri, getQuery?.Value, splitUrl) == true)
                        {
                            return api;
                        }
                    }
                }
                tmp = hit;
            }
            return null;
        }

        private void ComputeNode(string[] splitUrl, int pos, AllApiEntityIdentifier api)
        {
            ApiTreeNode now = this;
            for (int i = 1; i < splitUrl.Length; i++)
            {
                bool last = i == splitUrl.Length - 1;
                var next = now.NextNode.Where(x => x.RelativePath == splitUrl[i]).FirstOrDefault();
                if (next == null)
                {
                    var x = new ApiTreeNode() { RelativePath = splitUrl[i] };
                    now.NextNode.Add(x);
                    now = x;
                }
                else
                {
                    now = next;
                }
                if (last)
                {
                    now.Api.Add(api);
                }
            }
        }
    }
}
