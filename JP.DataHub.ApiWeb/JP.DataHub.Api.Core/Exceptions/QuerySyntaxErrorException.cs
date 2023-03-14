using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class QuerySyntaxErrorException : JPDataHubException
    {
        private const string ErrorMessageFormat = "Queryの構文に誤りがあります。{0}";

        public QuerySyntaxErrorException(string message) : base(string.Format(ErrorMessageFormat, message))
        {
        }
    }
}
