using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.WindowsAzure.Storage;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Attributes;
using Oracle.ManagedDataAccess.Client;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record RepositoryInfo : IValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRelativeUri" /> class.
        /// </summary>
        public RepositoryInfo(string repositoryType, Dictionary<string, bool> connectionStrings)
        {
            Type = repositoryType.CodeToRepositoryType();
            PhysicalRepositoryInfoList = connectionStrings.Select(x => new PhysicalRepositoryInfo(x.Key, x.Value, Type)).ToList();
        }

        public RepositoryInfo(Guid? repositoryGroupId, string repositoryType, Tuple<string, bool, Guid?> connectionStrings)
        {
            RepositoryGroupId = repositoryGroupId;
            Type = repositoryType.CodeToRepositoryType();
            PhysicalRepositoryInfoList = new List<PhysicalRepositoryInfo>();
            PhysicalRepositoryInfoList.Add(new PhysicalRepositoryInfo(connectionStrings.Item1, connectionStrings.Item2, Type, connectionStrings.Item3));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRelativeUri" /> class.
        /// </summary>
        public RepositoryInfo(Guid? repositoryGroupId, string repositoryType, List<Tuple<string, bool, Guid?>> connectionStrings)
        {
            RepositoryGroupId = repositoryGroupId;
            Type = repositoryType.CodeToRepositoryType();
            PhysicalRepositoryInfoList = connectionStrings.Select(x => new PhysicalRepositoryInfo(x.Item1, x.Item2, Type, x.Item3)).ToList();
        }

        public RepositoryType Type { get; }
        public Guid? RepositoryGroupId { get; internal set; }
        public PhysicalRepositoryId PhysicalRepositoryId
        {
            get
            {
                var value = PhysicalRepositoryInfoList.OrderBy(x => x.Isfull).FirstOrDefault()?.PhysicalRepositoryId?.ToString();
                return value == null ? null : new PhysicalRepositoryId(value);
            }
        }

        public string ConnectionString
        {
            get { return PhysicalRepositoryInfoList.OrderBy(x => x.Isfull).FirstOrDefault()?.ConnectionString; }
        }

        public string Endpoint
        {
            get { return PhysicalRepositoryInfoList.OrderBy(x => x.Isfull).FirstOrDefault()?.Endpoint; }
        }

        public bool CanSave
        {
            get { return PhysicalRepositoryInfoList.Any(x => !x.Isfull); }
        }

        public List<PhysicalRepositoryInfo> PhysicalRepositoryInfoList { get; internal set; }

        /// <summary>
        /// RDBMSのリポジトリかどうか
        /// </summary>
        public bool IsRdbmsRepository => Type.GetType().GetField(Type.ToString())?.GetCustomAttribute<RdbmsRepositoryAttribute>() is RdbmsRepositoryAttribute;


        public string GetConnectionStringFromEndPoint(string endPoint)
        {
            foreach (var p in PhysicalRepositoryInfoList)
            {
                if (p.Endpoint == endPoint)
                {
                    return p.ConnectionString;
                }
            }
            return "";
        }

        public class PhysicalRepositoryInfo
        {
            public readonly string[] AzureStrageKeys = new string[] { "DefaultEndpointsProtocol", "AccountName", "AccountKey", "EndpointSuffix" };
            public readonly string CosmosDbEndpointStrageKey = "AccountEndpoint";

            public string ConnectionString { get; }
            public bool Isfull { get; }
            public string Endpoint { get => GetEndpoint(); }
            public Guid? PhysicalRepositoryId { get; }

            private string _endpoint = null;
            private RepositoryType _repositoryType;

            public PhysicalRepositoryInfo(string connectionString, bool isfull, RepositoryType repositoryType, Guid? physicalRepositoryId = null)
            {
                this.ConnectionString = connectionString;
                this.Isfull = isfull;
                _repositoryType = repositoryType;
                PhysicalRepositoryId = physicalRepositoryId;
            }

            private string GetEndpoint()
            {
                if (_endpoint != null)
                {
                    return _endpoint;
                }
                switch (_repositoryType)
                {
                    case RepositoryType.AttachFileBlob:
                    case RepositoryType.BlobFileSharding:
                        CloudStorageAccount account = CloudStorageAccount.Parse(ConvertPerAzureStrageConnectionString(ConnectionString));
                        _endpoint = account.BlobEndpoint.ToString();
                        break;
                    case RepositoryType.QueueStorage:
                        CloudStorageAccount accountq = CloudStorageAccount.Parse(ConvertPerAzureStrageConnectionString(ConnectionString));
                        _endpoint = accountq.QueueEndpoint.ToString();
                        break;
                    case RepositoryType.SQLServer:
                    case RepositoryType.SQLServer2:
                        var builder = new SqlConnectionStringBuilder(ConnectionString);
                        _endpoint = builder.DataSource;
                        break;
                    case RepositoryType.OracleDb:
                        var orabuilder = new OracleConnectionStringBuilder(ConnectionString);
                        _endpoint = orabuilder.DataSource;
                        break;
                    case RepositoryType.CosmosDB:
                        _endpoint = ConnectionString.Split(';').Where(x => (x.Split('='))[0] == CosmosDbEndpointStrageKey).FirstOrDefault()?.Replace(CosmosDbEndpointStrageKey + "=", "");
                        break;
                    case RepositoryType.BlockchainNode:
                    case RepositoryType.EventHub:
                    case RepositoryType.AllEventHub:
                    default:
                        _endpoint = "";
                        break;
                }
                return _endpoint;
            }

            /// <summary>
            /// AzureStorage接続文字列を独自拡張している可能性があるため必要なもののみに変換する。（独自拡張部分が残っているとパーサーがエラートとなる）
            /// </summary>
            private string ConvertPerAzureStrageConnectionString(string connectionString)
            {
                StringBuilder result = new StringBuilder();
                var sp = connectionString.Split(';');
                foreach (var val in sp)
                {
                    var key = val.Split('=');
                    if (AzureStrageKeys.Contains(key[0]))
                    {
                        result.Append(val).Append(";");
                    }
                }
                return result.ToString();
            }
        }

        public static bool operator ==(RepositoryInfo me, object other) => me?.Equals(other) == true;

        public static bool operator !=(RepositoryInfo me, object other) => !me?.Equals(other) == true;
    }
}
