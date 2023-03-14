using MessagePack;
using System;
using System.Collections.Generic;

namespace JP.DataHub.ApiWeb.Domain.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class UserGroupModel
    {
        public string UserGroupId { get; set; }

        public string UserGroupName { get; set; }

        public string OpenId { get; set; }

        public IList<string> Members { get; set; } 
    }
}
