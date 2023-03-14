using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.RFC7807
{
    public class Rfc7807Exception : Exception
    {
        public RFC7807ProblemDetail Rfc7807 { get; set; }

        public Rfc7807Exception()
        {
        }

        public Rfc7807Exception(RFC7807ProblemDetail detail, Exception innerException = null)
            : base(detail.Title, innerException)
        {
            Rfc7807 = detail;
        }
    }
}
