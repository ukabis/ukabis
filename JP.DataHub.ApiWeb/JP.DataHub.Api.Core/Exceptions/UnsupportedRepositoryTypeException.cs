using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class UnsupportedRepositoryTypeException : DomainException
    {
        public UnsupportedRepositoryTypeException()
        {
        }

        public UnsupportedRepositoryTypeException(string message) : base(message)
        {
        }

        public UnsupportedRepositoryTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
