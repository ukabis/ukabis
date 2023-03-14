using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Database
{
    public static class DocumentDatabase
    {
#if Oracle
        public const string DATABASE = "DOCUMENT";
#else
        public const string DATABASE = "Document";
#endif
        public const string TABLE_AGREEMENT = "Agreement";
        public const string TABLE_CATEGORY = "Category";
        public const string TABLE_DOCUMENT = "Document";
        public const string TABLE_FILE = "File";
        public const string TABLE_SYSTEM = "System";
        public const string TABLE_VENDOR = "Vendor";
        public const string TABLE_STAFF = "Staff";
        public const string COLUMN_AGREEMENT_AGREEMENT_ID = "agreement_id";
        public const string COLUMN_AGREEMENT_VENDOR_ID = "vendor_id";
        public const string COLUMN_AGREEMENT_TITLE = "title";
        public const string COLUMN_AGREEMENT_DETAIL = "detail";
        public const string COLUMN_AGREEMENT_REG_DATE = "reg_date";
        public const string COLUMN_AGREEMENT_REG_USERNAME = "reg_username";
        public const string COLUMN_AGREEMENT_UPD_DATE = "upd_date";
        public const string COLUMN_AGREEMENT_UPD_USERNAME = "upd_username";
        public const string COLUMN_AGREEMENT_IS_ACTIVE = "is_active";
        public const string COLUMN_CATEGORY_CATEGORY_ID = "category_id";
        public const string COLUMN_CATEGORY_CATEGORY_NAME = "category_name";
        public const string COLUMN_CATEGORY_ORDER_NO = "order_no";
        public const string COLUMN_CATEGORY_REG_DATE = "reg_date";
        public const string COLUMN_CATEGORY_REG_USERNAME = "reg_username";
        public const string COLUMN_CATEGORY_UPD_DATE = "upd_date";
        public const string COLUMN_CATEGORY_UPD_USERNAME = "upd_username";
        public const string COLUMN_CATEGORY_IS_ACTIVE = "is_active";
        public const string COLUMN_DOCUMENT_DOCUMENT_ID = "document_id";
        public const string COLUMN_DOCUMENT_TITLE = "title";
        public const string COLUMN_DOCUMENT_DETAIL = "detail";
        public const string COLUMN_DOCUMENT_CATEGORY_ID = "category_id";
        public const string COLUMN_DOCUMENT_VENDOR_ID = "vendor_id";
        public const string COLUMN_DOCUMENT_SYSTEM_ID = "system_id";
        public const string COLUMN_DOCUMENT_IS_ENABLE = "is_enable";
        public const string COLUMN_DOCUMENT_IS_ADMIN_CHECK = "is_admin_check";
        public const string COLUMN_DOCUMENT_IS_ADMIN_STOP = "is_admin_stop";
        public const string COLUMN_DOCUMENT_AGREEMENT_ID = "agreement_id";
        public const string COLUMN_DOCUMENT_REG_DATE = "reg_date";
        public const string COLUMN_DOCUMENT_REG_USERNAME = "reg_username";
        public const string COLUMN_DOCUMENT_UPD_DATE = "upd_date";
        public const string COLUMN_DOCUMENT_UPD_USERNAME = "upd_username";
        public const string COLUMN_DOCUMENT_IS_ACTIVE = "is_active";
        public const string COLUMN_DOCUMENT_IS_PUBLIC_PORTAL = "is_public_portal";
        public const string COLUMN_DOCUMENT_IS_PUBLIC_ADMIN = "is_public_admin";
        public const string COLUMN_DOCUMENT_IS_PUBLIC_PORTAL_HIDDEN = "is_public_portal_hidden";
        public const string COLUMN_DOCUMENT_IS_PUBLIC_ADMIN_HIDDEN = "is_public_admin_hidden";
        public const string COLUMN_DOCUMENT_PASSWORD = "password";
        public const string COLUMN_FILE_FILE_ID = "file_id";
        public const string COLUMN_FILE_DOCUMENT_ID = "document_id";
        public const string COLUMN_FILE_TITLE = "title";
        public const string COLUMN_FILE_URL = "url";
        public const string COLUMN_FILE_IS_ENABLE = "is_enable";
        public const string COLUMN_FILE_REG_DATE = "reg_date";
        public const string COLUMN_FILE_REG_USERNAME = "reg_username";
        public const string COLUMN_FILE_UPD_DATE = "upd_date";
        public const string COLUMN_FILE_UPD_USERNAME = "upd_username";
        public const string COLUMN_FILE_IS_ACTIVE = "is_active";
        public const string COLUMN_FILE_FILE_UPDATE_DATE = "file_update_date";
        public const string COLUMN_FILE_HTML_LINK = "html_link";
        public const string COLUMN_FILE_ORDER_NO = "order_no";
        public const string COLUMN_SYSTEM_SYSTEM_ID = "system_id";
        public const string COLUMN_SYSTEM_VENDOR_ID = "vendor_id";
        public const string COLUMN_SYSTEM_SYSTEM_NAME = "system_name";
        public const string COLUMN_SYSTEM_OPENID_APPLICATIONID = "openid_applicationid";
        public const string COLUMN_SYSTEM_OPENID_CLIENT_SECRET = "openid_client_secret";
        public const string COLUMN_SYSTEM_IS_ENABLE = "is_enable";
        public const string COLUMN_SYSTEM_REG_DATE = "reg_date";
        public const string COLUMN_SYSTEM_REG_USERNAME = "reg_username";
        public const string COLUMN_SYSTEM_UPD_DATE = "upd_date";
        public const string COLUMN_SYSTEM_UPD_USERNAME = "upd_username";
        public const string COLUMN_SYSTEM_IS_ACTIVE = "is_active";
        public const string COLUMN_VENDOR_VENDOR_ID = "vendor_id";
        public const string COLUMN_VENDOR_VENDOR_NAME = "vendor_name";
        public const string COLUMN_VENDOR_IS_ENABLE = "is_enable";
        public const string COLUMN_VENDOR_IS_DATA_OFFER = "is_data_offer";
        public const string COLUMN_VENDOR_IS_DATA_USE = "is_data_use";
        public const string COLUMN_VENDOR_REG_DATE = "reg_date";
        public const string COLUMN_VENDOR_REG_USERNAME = "reg_username";
        public const string COLUMN_VENDOR_UPD_DATE = "upd_date";
        public const string COLUMN_VENDOR_UPD_USERNAME = "upd_username";
        public const string COLUMN_VENDOR_IS_ACTIVE = "is_active";
        public const string COLUMN_STAFF_STAFF_ID = "staff_id";
        public const string COLUMN_STAFF_ACCOUNT = "account";
        public const string COLUMN_STAFF_VENDOR_ID = "vendor_id";
        public const string COLUMN_STAFF_REG_DATE = "reg_date";
        public const string COLUMN_STAFF_REG_USERNAME = "reg_username";
        public const string COLUMN_STAFF_UPD_DATE = "upd_date";
        public const string COLUMN_STAFF_UPD_USERNAME = "upd_username";
        public const string COLUMN_STAFF_IS_ACTIVE = "is_active";
        public const string COLUMN_STAFF_EMAIL_ADDRESS = "email_address";
    }
}
