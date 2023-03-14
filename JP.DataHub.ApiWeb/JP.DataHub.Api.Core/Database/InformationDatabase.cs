namespace JP.DataHub.Api.Core.Database
{
    public static class InformationDatabase
    {
#if Oracle
        public const string DATABASE = "INFORMATION";
        public const string COLUMN_INFORMATION_DATE = "inf_date";
#else
        public const string DATABASE = "Information";
        public const string COLUMN_INFORMATION_DATE = "date";
#endif
        public const string TABLE_INFORMATION = "Information";
        public const string COLUMN_INFORMATION_INFORMATION_ID = "information_id";
        public const string COLUMN_INFORMATION_TITLE = "title";
        public const string COLUMN_INFORMATION_DETAIL = "detail";
        public const string COLUMN_INFORMATION_REG_DATE = "reg_date";
        public const string COLUMN_INFORMATION_REG_USERNAME = "reg_username";
        public const string COLUMN_INFORMATION_UPD_DATE = "upd_date";
        public const string COLUMN_INFORMATION_UPD_USERNAME = "upd_username";
        public const string COLUMN_INFORMATION_IS_ACTIVE = "is_active";
        public const string COLUMN_INFORMATION_IS_VISIBLE_API = "is_visible_api";
        public const string COLUMN_INFORMATION_IS_VISIBLE_ADMIN = "is_visible_admin";
    }
}
