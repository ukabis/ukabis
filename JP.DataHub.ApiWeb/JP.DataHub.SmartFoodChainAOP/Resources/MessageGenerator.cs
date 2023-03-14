using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Helper;
using JP.DataHub.Com.Resources;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.SmartFoodChainAOP.Resources;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Unity;
using Microsoft.Extensions.Configuration;

// .NET6
// この定義はMessageGenerator.ttから自動生成したものである
// このクラスを直接変更するのではなく、TTテンプレートから自動生成を必ず行うこと
namespace JP.DataHub.SmartFoodChainAOP.ErrorCode
{
    public static partial class ErrorCodeMessage
    {
        private const string MEDIATYPE_JSON = "application/json";

        public enum Code
        {
            [MessageDefinition("E100400_title", "E100400_detail", HttpStatusCode.Conflict)]
            E100400,
            [MessageDefinition("E100401_title", "E100401_detail", HttpStatusCode.Conflict)]
            E100401,
            [MessageDefinition("E100402_title", "E100402_detail", HttpStatusCode.NotFound)]
            E100402,
            [MessageDefinition("E100403_title", "E100403_detail", HttpStatusCode.NotFound)]
            E100403,
            [MessageDefinition("E100404_title", "E100404_detail", HttpStatusCode.Forbidden)]
            E100404,
            [MessageDefinition("E100405_title", "E100405_detail", HttpStatusCode.BadRequest)]
            E100405,
            [MessageDefinition("E100406_title", "E100406_detail", HttpStatusCode.Conflict)]
            E100406,
            [MessageDefinition("E100407_title", "E100407_detail", HttpStatusCode.Conflict)]
            E100407,
            [MessageDefinition("E100408_title", "E100408_detail", HttpStatusCode.BadRequest)]
            E100408,
            [MessageDefinition("E100409_title", "E100409_detail", HttpStatusCode.BadRequest)]
            E100409,
            [MessageDefinition("E100410_title", "E100410_detail", HttpStatusCode.Forbidden)]
            E100410,
            [MessageDefinition("E100411_title", "E100411_detail", HttpStatusCode.BadRequest)]
            E100411,
            [MessageDefinition("E100412_title", "E100412_detail", HttpStatusCode.BadRequest)]
            E100412,
            [MessageDefinition("E101400_title", "E101400_detail", HttpStatusCode.BadRequest)]
            E101400,
            [MessageDefinition("E101401_title", "E101401_detail", HttpStatusCode.BadRequest)]
            E101401,
            [MessageDefinition("E101402_title", "E101402_detail", HttpStatusCode.BadRequest)]
            E101402,
            [MessageDefinition("E101403_title", "E101403_detail", HttpStatusCode.BadRequest)]
            E101403,
            [MessageDefinition("E101404_title", "E101404_detail", HttpStatusCode.BadRequest)]
            E101404,
            [MessageDefinition("E101405_title", "E101405_detail", HttpStatusCode.BadRequest)]
            E101405,
            [MessageDefinition("E101406_title", "E101406_detail", HttpStatusCode.BadRequest)]
            E101406,
            [MessageDefinition("E101407_title", "E101407_detail", HttpStatusCode.BadRequest)]
            E101407,
            [MessageDefinition("E101408_title", "E101408_detail", HttpStatusCode.NotFound)]
            E101408,
            [MessageDefinition("E101501_title", "E101501_detail", HttpStatusCode.InternalServerError)]
            E101501,
            [MessageDefinition("E102401_title", "E102401_detail", HttpStatusCode.BadRequest)]
            E102401,
            [MessageDefinition("E102402_title", "E102402_detail", HttpStatusCode.BadRequest)]
            E102402,
            [MessageDefinition("E102403_title", "E102403_detail", HttpStatusCode.BadRequest)]
            E102403,
            [MessageDefinition("E102404_title", "E102404_detail", HttpStatusCode.BadRequest)]
            E102404,
            [MessageDefinition("E102405_title", "E102405_detail", HttpStatusCode.BadRequest)]
            E102405,
            [MessageDefinition("E102406_title", "E102406_detail", HttpStatusCode.BadRequest)]
            E102406,
            [MessageDefinition("E102407_title", "E102407_detail", HttpStatusCode.BadRequest)]
            E102407,
            [MessageDefinition("E102408_title", "E102408_detail", HttpStatusCode.BadRequest)]
            E102408,
            [MessageDefinition("E102409_title", "E102409_detail", HttpStatusCode.NotFound)]
            E102409,
            [MessageDefinition("E102410_title", "E102410_detail", HttpStatusCode.NotFound)]
            E102410,
            [MessageDefinition("E102411_title", "E102411_detail", HttpStatusCode.NotFound)]
            E102411,
            [MessageDefinition("E102412_title", "E102412_detail", HttpStatusCode.NotFound)]
            E102412,
            [MessageDefinition("E102413_title", "E102413_detail", HttpStatusCode.NotFound)]
            E102413,
            [MessageDefinition("E102414_title", "E102414_detail", HttpStatusCode.BadRequest)]
            E102414,
            [MessageDefinition("E102415_title", "E102415_detail", HttpStatusCode.BadRequest)]
            E102415,
            [MessageDefinition("E102416_title", "E102416_detail", HttpStatusCode.BadRequest)]
            E102416,
            [MessageDefinition("E102417_title", "E102417_detail", HttpStatusCode.BadRequest)]
            E102417,
            [MessageDefinition("E102418_title", "E102418_detail", HttpStatusCode.BadRequest)]
            E102418,
            [MessageDefinition("E103400_title", "E103400_detail", HttpStatusCode.BadRequest)]
            E103400,
            [MessageDefinition("E103401_title", "E103401_detail", HttpStatusCode.BadRequest)]
            E103401,
            [MessageDefinition("E103402_title", "E103402_detail", HttpStatusCode.BadRequest)]
            E103402,
            [MessageDefinition("E103403_title", "E103403_detail", HttpStatusCode.BadRequest)]
            E103403,
            [MessageDefinition("E103404_title", "E103404_detail", HttpStatusCode.BadRequest)]
            E103404,
            [MessageDefinition("E103405_title", "E103405_detail", HttpStatusCode.BadRequest)]
            E103405,
            [MessageDefinition("E103406_title", "E103406_detail", HttpStatusCode.NotFound)]
            E103406,
            [MessageDefinition("E103407_title", "E103407_detail", HttpStatusCode.NotFound)]
            E103407,
            [MessageDefinition("E103408_title", "E103408_detail", HttpStatusCode.BadRequest)]
            E103408,
            [MessageDefinition("E103409_title", "E103409_detail", HttpStatusCode.BadRequest)]
            E103409,
            [MessageDefinition("E103410_title", "E103410_detail", HttpStatusCode.BadRequest)]
            E103410,
            [MessageDefinition("E103411_title", "E103411_detail", HttpStatusCode.BadRequest)]
            E103411,
            [MessageDefinition("E103501_title", "E103501_detail", HttpStatusCode.InternalServerError)]
            E103501,
            [MessageDefinition("E103502_title", "E103502_detail", HttpStatusCode.InternalServerError)]
            E103502,
            [MessageDefinition("E104400_title", "E104400_detail", HttpStatusCode.BadRequest)]
            E104400,
            [MessageDefinition("E104401_title", "E104401_detail", HttpStatusCode.BadRequest)]
            E104401,
            [MessageDefinition("E104402_title", "E104402_detail", HttpStatusCode.BadRequest)]
            E104402,
            [MessageDefinition("E104403_title", "E104403_detail", HttpStatusCode.NotFound)]
            E104403,
            [MessageDefinition("E104404_title", "E104404_detail", HttpStatusCode.BadRequest)]
            E104404,
            [MessageDefinition("E104405_title", "E104405_detail", HttpStatusCode.BadRequest)]
            E104405,
            [MessageDefinition("E104406_title", "E104406_detail", HttpStatusCode.BadRequest)]
            E104406,
            [MessageDefinition("E104407_title", "E104407_detail", HttpStatusCode.BadRequest)]
            E104407,
            [MessageDefinition("E104408_title", "E104408_detail", HttpStatusCode.BadRequest)]
            E104408,
            [MessageDefinition("E104409_title", "E104409_detail", HttpStatusCode.NotFound)]
            E104409,
            [MessageDefinition("E105400_title", "E105400_detail", HttpStatusCode.BadRequest)]
            E105400,
            [MessageDefinition("E106501_title", "E106501_detail", HttpStatusCode.InternalServerError)]
            E106501,
            [MessageDefinition("E106402_title", "E106402_detail", HttpStatusCode.BadRequest)]
            E106402,
            [MessageDefinition("E106403_title", "E106403_detail", HttpStatusCode.BadRequest)]
            E106403,
            [MessageDefinition("E106404_title", "E106404_detail", HttpStatusCode.BadRequest)]
            E106404,
            [MessageDefinition("E106405_title", "E106405_detail", HttpStatusCode.BadRequest)]
            E106405,
            [MessageDefinition("E106406_title", "E106406_detail", HttpStatusCode.BadRequest)]
            E106406,
            [MessageDefinition("E106507_title", "E106507_detail", HttpStatusCode.InternalServerError)]
            E106507,
            [MessageDefinition("E106408_title", "E106408_detail", HttpStatusCode.BadRequest)]
            E106408,
            [MessageDefinition("E103412_title", "E103412_detail", HttpStatusCode.BadRequest)]
            E103412,
            [MessageDefinition("E103413_title", "E103413_detail", HttpStatusCode.BadRequest)]
            E103413,
            [MessageDefinition("E107401_title", "E107401_detail", HttpStatusCode.BadRequest)]
            E107401,
            [MessageDefinition("E107402_title", "E107402_detail", HttpStatusCode.NotFound)]
            E107402,
            [MessageDefinition("E107403_title", "E107403_detail", HttpStatusCode.BadRequest)]
            E107403,
            [MessageDefinition("E107501_title", "E107501_detail", HttpStatusCode.InternalServerError)]
            E107501,
            [MessageDefinition("E107502_title", "E107502_detail", HttpStatusCode.InternalServerError)]
            E107502,
        }

        private static Lazy<System.Resources.ResourceManager> _resourceManager = new Lazy<System.Resources.ResourceManager>(() =>
        {
            var list = RuntimeReflectionExtensions.GetRuntimeProperties(typeof(SmartFoodChainAOPMessages)).ToList();
            var prop = list?.Where(x => x.Name == "ResourceManager").FirstOrDefault();
            var result = (System.Resources.ResourceManager)prop?.GetValue(null);
            return result;
        });

        private static System.Resources.ResourceManager ResourceManager => _resourceManager.Value;

        public static string GetString(string resourceName, CultureInfo cultureInfo)
        {
            IDataContainer dataContainer;
            try
            {
                dataContainer = UnityCore.Resolve<IDataContainer>();
            }
            catch (ResolutionFailedException)
            {
                dataContainer = UnityCore.Resolve<IDataContainer>("multiThread");
            }
            return ResourceManager.GetString(resourceName, cultureInfo);
        }

        public static string GetString(string resourceName)
        {
            IDataContainer dataContainer;
            try
            {
                dataContainer = UnityCore.Resolve<IDataContainer>();
            }
            catch (ResolutionFailedException)
            {
                dataContainer = UnityCore.Resolve<IDataContainer>("multiThread");
            }
            return ResourceManager.GetString(resourceName, dataContainer.CultureInfo);
        }

        public static RFC7807ProblemDetailExtendErrors GetRFC7807(this Code errorCode, CultureInfo cultureInfo, string relativeUrl = null)
        {
            // エラーの変換が定義されていればエラーコードを変換する
            var newcodestring = UnityCore.Resolve<IConfiguration>().GetValue<string>($"Rfc7807ErrorMap:{errorCode}.To");
            if (!string.IsNullOrEmpty(newcodestring) && newcodestring.TryParse<Code>(out var newcode))
            {
                errorCode = newcode;
            }

            var rpdc = new RFC7807ProblemDetailExtendErrors();
            if (string.IsNullOrEmpty(relativeUrl) == false)
            {
                rpdc.Instance = new Uri(relativeUrl, UriKind.Relative);
            }
            rpdc.ErrorCode = errorCode.ToString();

            var attr = errorCode.GetAttribute<MessageDefinitionAttribute>();
            if (attr != null)
            {
                rpdc.Title = GetString(attr.Message, cultureInfo);
                rpdc.Detail = GetString(attr.Detail, cultureInfo);
                rpdc.Status = (int)attr.HttpStatusCode;
            }
            return rpdc;
        }

        public static RFC7807ProblemDetailExtendErrors GetRFC7807(this Code errorCode, string relativeUrl = null)
        {
            // エラーの変換が定義されていればエラーコードを変換する
            var newcodestring = UnityCore.Resolve<IConfiguration>().GetValue<string>($"Rfc7807ErrorMap:{errorCode}.To");
            if (!string.IsNullOrEmpty(newcodestring) && newcodestring.TryParse<Code>(out var newcode))
            {
                errorCode = newcode;
            }

            var rpdc = new RFC7807ProblemDetailExtendErrors();
            if (string.IsNullOrEmpty(relativeUrl) == false)
            {
                rpdc.Instance = new Uri(relativeUrl, UriKind.Relative);
            }
            rpdc.ErrorCode = errorCode.ToString();

            var attr = errorCode.GetAttribute<MessageDefinitionAttribute>();
            if (attr != null)
            {
                rpdc.Title = GetString(attr.Message);
                rpdc.Detail = GetString(attr.Detail);
                rpdc.Status = (int)attr.HttpStatusCode;
            }
            return rpdc;
        }

        public static HttpResponseMessage GetRFC7807HttpResponseMessage(Code errorCode, CultureInfo cultureInfo, string relativeUrl = null, string title = null, string detail = null)
        {
            var error = GetRFC7807(errorCode, relativeUrl);
            return GetRFC7807HttpResponseMessage(error, relativeUrl, title, detail);
        }

        public static HttpResponseMessage GetRFC7807HttpResponseMessage(Code errorCode, string relativeUrl = null, string title = null, string detail = null)
        {
            var error = GetRFC7807(errorCode, relativeUrl);
            return GetRFC7807HttpResponseMessage(error, relativeUrl, title, detail);
        }

        public static HttpResponseMessage GetRFC7807HttpResponseMessage(RFC7807ProblemDetailExtendErrors error, string relativeUrl = null, string title = null, string detail = null)
        {
            if (string.IsNullOrEmpty(relativeUrl) == false)
            {
                error.Instance = new Uri(relativeUrl, UriKind.Relative);
            }

            if (string.IsNullOrEmpty(title) == false)
            {
                error.Title = title;
            }

            if (string.IsNullOrEmpty(detail) == false)
            {
                error.Detail = detail;
            }

            return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MEDIATYPE_JSON) };
        }
    }
}

