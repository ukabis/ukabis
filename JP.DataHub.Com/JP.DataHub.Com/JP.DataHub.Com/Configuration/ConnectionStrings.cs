using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JP.DataHub.Com.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.IO;

namespace JP.DataHub.Com.Configuration
{
    public class ConnectionStrings : List<ConnectionStringConfig>, IConnectionStrings
    {
        public bool IsTransactionManage { get; set; } = false;
        public bool IsTransactionScope { get; set; } = false;
        public string LifeTimeManager { get; set; }

        public ConnectionStrings(string fileName)
        {
            string contents = fileName.ReadFileContents();
            var json = contents.ToJson();
            var root = json?["ConnectionStrings"];
            foreach (JProperty item in root)
            {
                if (item.Value.Path == "ConnectionStrings.TransactionScope")
                {
                    IsTransactionScope = bool.Parse(item.Value.ToString());
                }
                else if (item.Value.Path == "ConnectionStrings.IsTransactionManage")
                {
                    IsTransactionManage = bool.Parse(item.Value.ToString());
                }
                else if (item.Value.Path == "ConnectionStrings.LifeTimeManager")
                {
                    LifeTimeManager = item.Value.ToString();
                }
                else
                {
                    var csc = item.Value.ToString().ToJson<ConnectionStringConfig>();
                    csc.Name = item.Name;
                    this.Add(csc);
                }
            }
        }
    }
}
