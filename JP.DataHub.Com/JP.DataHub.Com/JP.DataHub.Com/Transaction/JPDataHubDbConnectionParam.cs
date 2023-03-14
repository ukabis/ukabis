using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Transaction
{
    public class JPDataHubDbConnectionParam
    {
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
        public bool IsMultithread { get; set; }
        public bool IsTransactionManage { get; set; }
        public bool IsTransactionScope { get; set; }
        public Dictionary<string,string> Options { get; set; }

        public JPDataHubDbConnectionParam()
        {
        }
    }
}
