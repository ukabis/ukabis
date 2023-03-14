using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface ISendMailRepository
    {
        Task<bool> SendMailAsync(string mailTemplateCd, Dictionary<string, object> parameters, Dictionary<string, string> loggingKeyValue = null);
    }
}
