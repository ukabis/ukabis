
namespace JP.DataHub.Com.Consts
{
    public static class JsonPropertyConst
    {
#if Oracle
        public const string TYPE = "ora_type";
        public const string VENDORID = "ora_vendor_id";
        public const string SYSTEMID = "ora_system_id";
        public const string OPENID = "ora_reguser_id";
        public const string OWNERID = "ora_owner_id";
        public const string REGDATE = "ora_regdate";
        public const string UPDUSERID = "ora_upduser_id";
        public const string UPDDATE = "ora_upddate";
        public const string VERSION_COLNAME = "ora_version";
        public const string PARTITIONKEY = "ora_partitionkey";
        public const string VERSION_VALUE = "version";
        public const string SEPARATOR = "~";
        public const string ID = "id";
        public const string _ID = "ora_id";
        public const string ETAG = "ora_etag";

        public const string CURRENT_VERSION = "currentversion";
        public const string DOLLER_ONE = "$1";
#else
        public const string TYPE = "_Type";
        public const string VENDORID = "_Vendor_Id";
        public const string SYSTEMID = "_System_Id";
        public const string OPENID = "_Reguser_Id";
        public const string OWNERID = "_Owner_Id";
        public const string REGDATE = "_Regdate";
        public const string UPDUSERID = "_Upduser_Id";
        public const string UPDDATE = "_Upddate";
        public const string VERSION_COLNAME = "_Version";
        public const string PARTITIONKEY = "_partitionkey";
        public const string VERSION_VALUE = "version";
        public const string SEPARATOR = "~";
        public const string ID = "id";
        public const string _ID = "_id";
        public const string ETAG = "_etag";

        public const string CURRENT_VERSION = "currentversion";
        public const string DOLLER_ONE = "$1";
#endif
    }
}
