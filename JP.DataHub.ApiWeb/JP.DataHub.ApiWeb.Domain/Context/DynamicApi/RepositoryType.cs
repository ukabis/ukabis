using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Attributes;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public class RepositoryTypeCodeAttribute : Attribute
    {
        public string Code { get; set; }
        
        public RepositoryTypeCodeAttribute(string code)
            => Code = code;
    }

    public enum RepositoryType
    {
        /// <summary>nothing</summary>
        [RepositoryTypeCodeAttribute("none")]
        Unknown,

        /// <summary>SQL Server</summary>
        [RepositoryTypeCodeAttribute("ssd")]
        [Obsolete("互換性のために残している旧版です。ss2を使用してください。")]
        SQLServer,

        [RdbmsRepository]
        [RepositoryTypeCodeAttribute("ss2")]
        [ODataPatchSupported(ODataPatchSupport.BulkUpdate)]
        SQLServer2,

        /// <summary>Blob File Sharding</summary>
        [RepositoryTypeCodeAttribute("bfs")]
        BlobFileSharding,

        /// <summary>Document DB</summary>
        [RepositoryTypeCodeAttribute("ddb")]
        CosmosDB,

        /// <summary>Blob</summary>
        [RepositoryTypeCodeAttribute("blb")]
        BlobFile,

        /// <summary>DataLakeStore</summary>
        [RepositoryTypeCodeAttribute("dls")]
        DataLake,

        /// <summary>QueueStorage</summary>
        [RepositoryTypeCodeAttribute("qus")]
        QueueStorage,

        /// <summary>EventHub</summary>
        [RepositoryTypeCodeAttribute("ehb")]
        EventHub,

        /// <summary>All EventHub</summary>
        [RepositoryTypeCodeAttribute("aeh")]
        AllEventHub,

        /// <summary>Fiware</summary>
        [RepositoryTypeCodeAttribute("fiw")]
        Fiware,

        /// <summary>AttachFile Blob</summary>
        [RepositoryTypeCodeAttribute("afb")]
        AttachFileBlob,

        /// <summary>AttachFile Meta(CosmosDB)</summary>
        [RepositoryTypeCodeAttribute("afm")]
        AttachFileMetaCosmosDb,

        /// <summary>AttachFile Meta(SQLServer)</summary>
        [RepositoryTypeCodeAttribute("afs")]
        AttachFileMetaSqlServer,

        /// <summary>BlockchainNode</summary>
        [RepositoryTypeCodeAttribute("bcn")]
        BlockchainNode,

        /// <summary>DocumentHistory Storage</summary>
        [RepositoryTypeCodeAttribute("dhs")]
        DocumentHistoryStorage,

        /// <summary>Momgo DB</summary>
        [RepositoryTypeCodeAttribute("mng")]
        MomgoDB,

        /// <summary>Time Series Insights</summary>
        [RepositoryTypeCodeAttribute("tsi")]
        TimeSeriesInsights,

        /// <summary>Oracle DB</summary>
        [RepositoryTypeCodeAttribute("ora")]
        [ODataPatchSupported(ODataPatchSupport.BulkUpdate)]
        OracleDb,
    }

    internal static class RepositoryTypeExtensions
    {
        private static Dictionary<RepositoryType, string> s_listCodeMap1;
        private static Dictionary<string, RepositoryType> s_listCodeMap2;

        internal static RepositoryType CodeToRepositoryType(this string val)
            => s_listCodeMap2[val];

        internal static string ToCode(this RepositoryType type)
             => s_listCodeMap1[type];

        static RepositoryTypeExtensions()
        {
            s_listCodeMap1 = new Dictionary<RepositoryType, string>();
            s_listCodeMap2 = new Dictionary<string, RepositoryType>();
            foreach (RepositoryType value in Enum.GetValues(typeof(RepositoryType)))
            {
                var attrCode = value.GetType()?.GetMember(value.ToString())?.FirstOrDefault()?.GetCustomAttribute< RepositoryTypeCodeAttribute>();
                if (attrCode != null)
                {
                    s_listCodeMap1.Add(value, attrCode.Code);
                    s_listCodeMap2.Add(attrCode.Code, value);
                }
            }
        }
    }
}
