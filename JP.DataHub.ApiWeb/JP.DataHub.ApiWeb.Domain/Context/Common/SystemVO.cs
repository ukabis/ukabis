using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class SystemVO
    {
        public Guid system_id { get; private set; }
        public string system_name { get; private set; }

        public SystemVO(Guid system_id, string system_name)
        {
            this.system_id = system_id;
            this.system_name = system_name;
        }

        public static bool operator ==(SystemVO me, object other) => me?.Equals(other) == true;

        public static bool operator !=(SystemVO me, object other) => !me?.Equals(other) == true;
    }
}
