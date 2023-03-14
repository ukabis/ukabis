namespace JP.DataHub.ManageApi.Service.DymamicApi
{
    public class RepositoryInfo
    {
        public enum RepositoryType
        {
            /// <summary>
            /// nothing
            /// </summary>
            none,

            /// <summary>
            /// SQL Server(旧版)
            /// </summary>
            [Obsolete("互換性のために残している旧版です。ss2を使用してください。")]
            ssd,

            /// <summary>
            /// SQL Server
            /// </summary>
            ss2,

            /// <summary>
            /// Blob File Sharding
            /// </summary>
            bfs,

            /// <summary>
            /// Document DB(DataStoreRepository)
            /// </summary>
            ddb,

            /// <summary>
            /// Blob(DataStoreRepository)
            /// </summary>
            blb,

            /// <summary>
            /// DataLakeStore
            /// </summary>
            dls,

            /// <summary>
            /// QueueStorage
            /// </summary>
            qus,

            /// <summary>
            /// EventHub
            /// </summary>
            ehb,

            /// <summary>
            /// All EventHub
            /// </summary>
            aeh,

            /// <summary>
            /// Fiware
            /// </summary>
            fiw,

            /// <summary>
            /// AttachFile Blob
            /// </summary>
            afb,

            /// <summary>
            /// AttachFile Meta
            /// </summary>
            afm,

            /// <summary>
            /// BlockchainNode
            /// </summary>
            bcn,

            /// <summary>
            /// DocumentHistory Storage
            /// </summary>
            dhs,

            /// <summary>
            /// Mongo DB
            /// </summary>
            mng,

            /// <summary>
            /// Time Series Insights
            /// </summary>
            tsi,

            /// <summary>
            /// Oracle
            /// </summary>
            ora,
        }
    }
}
