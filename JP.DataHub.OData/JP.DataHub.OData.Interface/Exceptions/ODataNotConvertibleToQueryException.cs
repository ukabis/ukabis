
namespace JP.DataHub.OData.Interface.Exceptions
{
    [Serializable]
    public class ODataNotConvertibleToQueryException : JPDataHubODataException
    {
        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public ODataNotConvertibleToQueryException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        public ODataNotConvertibleToQueryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
        public ODataNotConvertibleToQueryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
