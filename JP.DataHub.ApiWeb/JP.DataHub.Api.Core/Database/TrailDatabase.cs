using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Database
{
    public static class TrailDatabase
    {
#if Oracle
        public const string DATABASE = "TRAIL";
#else
        public const string DATABASE = "Trail";
#endif
        public const string TABLE_ADMIN = "Admin";
        public const string TABLE_STAFF = "Staff";
        public const string TABLE_TRAIL = "Trail";
        public const string TABLE_TRAILTYPE = "TrailType";
        public const string TABLE_VENDOR = "Vendor";
        public const string COLUMN_ADMIN_TRAIL_ID = "trail_id";
        public const string COLUMN_ADMIN_SCREEN = "screen";
        public const string COLUMN_ADMIN_OPERATION = "operation";
        public const string COLUMN_ADMIN_CONTOLLER_CLASS_NAME = "contoller_class_name";
        public const string COLUMN_ADMIN_ACTION_METHOD_NAME = "action_method_name";
        public const string COLUMN_ADMIN_OPEN_ID = "open_id";
        public const string COLUMN_ADMIN_VENDOR_ID = "vendor_id";
        public const string COLUMN_ADMIN_IP_ADDRESS = "ip_address";
        public const string COLUMN_ADMIN_USER_AGENT = "user_agent";
        public const string COLUMN_ADMIN_URL = "url";
        public const string COLUMN_ADMIN_HTTP_METHOD_TYPE = "http_method_type";
        public const string COLUMN_ADMIN_HTTP_STATUS_CODE = "http_status_code";
        public const string COLUMN_ADMIN_METHOD_PARAMETER = "method_parameter";
        public const string COLUMN_ADMIN_METHOD_RESULT = "method_result";
        public const string COLUMN_ADMIN_REG_DATE = "reg_date";
        public const string COLUMN_STAFF_STAFF_ID = "staff_id";
        public const string COLUMN_STAFF_ACCOUNT = "account";
        public const string COLUMN_STAFF_VENDOR_ID = "vendor_id";
        public const string COLUMN_STAFF_UPD_DATE = "upd_date";
        public const string COLUMN_STAFF_UPD_USERNAME = "upd_username";
        public const string COLUMN_STAFF_IS_ACTIVE = "is_active";
        public const string COLUMN_STAFF_EMAIL_ADDRESS = "email_address";
        public const string COLUMN_TRAIL_TRAIL_ID = "trail_id";
        public const string COLUMN_TRAIL_TRAIL_TYPE_CD = "trail_type_cd";
        public const string COLUMN_TRAIL_RESULT = "result";
        public const string COLUMN_TRAIL_DETAIL = "detail";
        public const string COLUMN_TRAIL_REG_DATE = "reg_date";
        public const string COLUMN_TRAILTYPE_TRAIL_TYPE_CD = "trail_type_cd";
        public const string COLUMN_TRAILTYPE_TRAIL_TYPE_NAME = "trail_type_name";
        public const string COLUMN_TRAILTYPE_REG_DATE = "reg_date";
        public const string COLUMN_TRAILTYPE_REG_USERNAME = "reg_username";
        public const string COLUMN_TRAILTYPE_UPD_DATE = "upd_date";
        public const string COLUMN_TRAILTYPE_UPD_USERNAME = "upd_username";
        public const string COLUMN_VENDOR_VENDOR_ID = "vendor_id";
        public const string COLUMN_VENDOR_VENDOR_NAME = "vendor_name";
        public const string COLUMN_VENDOR_IS_ENABLE = "is_enable";
        public const string COLUMN_VENDOR_UPD_DATE = "upd_date";
        public const string COLUMN_VENDOR_UPD_USERNAME = "upd_username";
        public const string COLUMN_VENDOR_IS_ACTIVE = "is_active";
    }
}
