using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile
{
    // .NET6
    internal record ExternalAttachFileInfomation : IValueObject
    {
        /// <summary>
        /// サポート対象のデータソース種別
        /// </summary>
        public static List<string> SupportedDataSourceTypes = new List<string>
        {
            "az-blob"
        };


        /// <summary>
        /// 外部添付ファイルのデータソース種別
        /// </summary>
        public string DataSourceType { get; }

        /// <summary>
        /// 外部添付ファイルの認証情報
        /// </summary>
        public List<string> Credentials { get; }

        /// <summary>
        /// 外部添付ファイルのパス
        /// </summary>
        public string FilePath { get; }


        public static ExternalAttachFileInfomation PerseFromJToken(JToken json)
        {
            if (json.Type == JTokenType.Null)
            {
                return null;
            }

            var credentials = new List<string>();
            if (json.IsExistProperty(nameof(Credentials)))
            {
                foreach (var credential in json[nameof(Credentials)].Children())
                {
                    credentials.Add(credential.Value<string>());
                }
            }

            return new ExternalAttachFileInfomation(
                dataSourceType: json.Value<string>(nameof(DataSourceType)),
                credentials: credentials,
                filePath: json.Value<string>(nameof(FilePath)));
        }

        public ExternalAttachFileInfomation(
            string dataSourceType,
            List<string> credentials,
            string filePath)
        {
            if (dataSourceType == null)
            {
                throw new ArgumentNullException("DataSourceType");
            }
            if (credentials == null)
            {
                throw new ArgumentNullException("Credentials");
            }
            if (filePath == null)
            {
                throw new ArgumentNullException("FilePath");
            }

            DataSourceType = dataSourceType;
            Credentials = credentials;
            FilePath = filePath;
        }
    }
}
