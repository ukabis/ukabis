using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Database
{
    public static class AttachFileDatabase
    {
#if Oracle
        public const string DATABASE = "ATTACHFILE";
#else
        public const string DATABASE = "AttachFile";
#endif
        public const string TABLE_ATTACHFILESTORAGE = "AttachFileStorage";
        public const string TABLE_VENDORATTACHFILESTORAGE = "VendorAttachfilestorage";
        public const string COLUMN_ATTACHFILESTORAGE_ATTACHFILE_STORAGE_ID = "attachfile_storage_id";
        public const string COLUMN_ATTACHFILESTORAGE_ATTACHFILE_STORAGE_NAME = "attachfile_storage_name";
        public const string COLUMN_ATTACHFILESTORAGE_CONNECTION_STRING = "connection_string";
        public const string COLUMN_ATTACHFILESTORAGE_IS_FULL = "is_full";
        public const string COLUMN_ATTACHFILESTORAGE_ATTACHFILE_STORAGE_DESCRIPTION = "attachfile_storage_description";
        public const string COLUMN_ATTACHFILESTORAGE_REG_DATE = "reg_date";
        public const string COLUMN_ATTACHFILESTORAGE_REG_USERNAME = "reg_username";
        public const string COLUMN_ATTACHFILESTORAGE_UPD_DATE = "upd_date";
        public const string COLUMN_ATTACHFILESTORAGE_UPD_USERNAME = "upd_username";
        public const string COLUMN_ATTACHFILESTORAGE_IS_ACTIVE = "is_active";
        public const string COLUMN_VENDORATTACHFILESTORAGE_VENDOR_ATTACHFILESTORAGE_ID = "vendor_attachfilestorage_id";
        public const string COLUMN_VENDORATTACHFILESTORAGE_VENDOR_ID = "vendor_id";
        public const string COLUMN_VENDORATTACHFILESTORAGE_ATTACHFILE_STORAGE_ID = "attachfile_storage_id";
        public const string COLUMN_VENDORATTACHFILESTORAGE_IS_CURRENT = "is_current";
        public const string COLUMN_VENDORATTACHFILESTORAGE_REG_DATE = "reg_date";
        public const string COLUMN_VENDORATTACHFILESTORAGE_REG_USERNAME = "reg_username";
        public const string COLUMN_VENDORATTACHFILESTORAGE_UPD_DATE = "upd_date";
        public const string COLUMN_VENDORATTACHFILESTORAGE_UPD_USERNAME = "upd_username";
        public const string COLUMN_VENDORATTACHFILESTORAGE_IS_ACTIVE = "is_active";
    }
}
