using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;

namespace JP.DataHub.Com.Resources
{
    public static class Rfc7807
    {
        private static string GetString<T>(string resourceName, CultureInfo culture = null)
        {
            return ResourceManagerEx.GetResourceManager(typeof(T)).GetString(resourceName, culture);
        }

        public static RFC7807ProblemDetail GetRfc7807<T>(string code, MessageDefinitionAttribute attribute, IDictionary<string, string[]> errors = null, string relativeUrl = null, CultureInfo culture = null)
        {
            var rpdc = new RFC7807ProblemDetail();
            if (string.IsNullOrEmpty(relativeUrl) == false)
            {
                rpdc.Instance = new Uri(relativeUrl, UriKind.Relative);
            }
            rpdc.ErrorCode = code;

            culture ??= DataContainerUtil.ResolveDataContainer()?.CultureInfo;
            
            if (attribute != null)
            {
                rpdc.Title = GetString<T>(attribute.Message, culture);
                rpdc.Detail = GetString<T>(attribute.Detail, culture);
                rpdc.Status = (int)attribute.HttpStatusCode;
            }
            rpdc.Errors = errors;
            return rpdc;
        }
    }
}
