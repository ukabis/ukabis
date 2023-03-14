using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.WebRequest
{
    public enum ServerType
    {
        default_server,
        ea_xml,
    }

    public static class ServerTypeExtention
    {
        public static bool IsServer(this ServerType type)
            => type == ServerType.default_server;
    }
}
