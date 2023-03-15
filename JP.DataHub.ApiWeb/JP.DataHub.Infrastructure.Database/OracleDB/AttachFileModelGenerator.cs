﻿
// This file was automatically generated by the Dapper.SimpleCRUD T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: `AttachFile`
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `Server=tcp:jpdatahubdev.database.windows.net,1433;Initial Catalog=AttachFileSqlServer;Persist Security Info=False;User ID=;Password=******;`
//     Include Views:          `True`

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Msgpack=MessagePack;
using JP.DataHub.Com.Validations.Attributes;

namespace JP.DataHub.Infrastructure.Database.AttachFile
{
    public static class DatabaseConst
    {
        public const string DATABASE = "AttachFile";

        public static readonly string COLUMN_ATTACHFILESTORAGE_ATTACHFILE_STORAGE_ID = "attachfile_storage_id";
        public static readonly string COLUMN_ATTACHFILESTORAGE_ATTACHFILE_STORAGE_NAME = "attachfile_storage_name";
        public static readonly string COLUMN_ATTACHFILESTORAGE_CONNECTION_STRING = "connection_string";
        public static readonly string COLUMN_ATTACHFILESTORAGE_IS_FULL = "is_full";
        public static readonly string COLUMN_ATTACHFILESTORAGE_ATTACHFILE_STORAGE_DESCRIPTION = "attachfile_storage_description";
        public static readonly string COLUMN_ATTACHFILESTORAGE_REG_DATE = "reg_date";
        public static readonly string COLUMN_ATTACHFILESTORAGE_REG_USERNAME = "reg_username";
        public static readonly string COLUMN_ATTACHFILESTORAGE_UPD_DATE = "upd_date";
        public static readonly string COLUMN_ATTACHFILESTORAGE_UPD_USERNAME = "upd_username";
        public static readonly string COLUMN_ATTACHFILESTORAGE_IS_ACTIVE = "is_active";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_VENDOR_ATTACHFILESTORAGE_ID = "vendor_attachfilestorage_id";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_VENDOR_ID = "vendor_id";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_ATTACHFILE_STORAGE_ID = "attachfile_storage_id";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_IS_CURRENT = "is_current";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_REG_DATE = "reg_date";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_REG_USERNAME = "reg_username";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_UPD_DATE = "upd_date";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_UPD_USERNAME = "upd_username";
        public static readonly string COLUMN_VENDORATTACHFILESTORAGE_IS_ACTIVE = "is_active";
    }

    public enum Tables
    {
        AttachFileStorage,
        VendorAttachfilestorage,
    }

    public enum AttachFileAttachFileStorage
    {
        [DbMap("AttachFile","Attach_File_Storage","attachfile_storage_id")]
        attachfile_storage_id,
        [DbMap("AttachFile","Attach_File_Storage","attachfile_storage_name")]
        attachfile_storage_name,
        [DbMap("AttachFile","Attach_File_Storage","connection_string")]
        connection_string,
        [DbMap("AttachFile","Attach_File_Storage","is_full")]
        is_full,
        [DbMap("AttachFile","Attach_File_Storage","attachfile_storage_description")]
        attachfile_storage_description,
        [DbMap("AttachFile","Attach_File_Storage","reg_date")]
        reg_date,
        [DbMap("AttachFile","Attach_File_Storage","reg_username")]
        reg_username,
        [DbMap("AttachFile","Attach_File_Storage","upd_date")]
        upd_date,
        [DbMap("AttachFile","Attach_File_Storage","upd_username")]
        upd_username,
        [DbMap("AttachFile","Attach_File_Storage","is_active")]
        is_active,
    }
    public enum AttachFileVendorAttachfilestorage
    {
        [DbMap("AttachFile","Vendor_Attachfilestorage","vendor_attachfilestorage_id")]
        vendor_attachfilestorage_id,
        [DbMap("AttachFile","Vendor_Attachfilestorage","vendor_id")]
        vendor_id,
        [DbMap("AttachFile","Vendor_Attachfilestorage","attachfile_storage_id")]
        attachfile_storage_id,
        [DbMap("AttachFile","Vendor_Attachfilestorage","is_current")]
        is_current,
        [DbMap("AttachFile","Vendor_Attachfilestorage","reg_date")]
        reg_date,
        [DbMap("AttachFile","Vendor_Attachfilestorage","reg_username")]
        reg_username,
        [DbMap("AttachFile","Vendor_Attachfilestorage","upd_date")]
        upd_date,
        [DbMap("AttachFile","Vendor_Attachfilestorage","upd_username")]
        upd_username,
        [DbMap("AttachFile","Vendor_Attachfilestorage","is_active")]
        is_active,
    }


    /// <summary>
    /// A class which represents the AttachFileStorage table.
    /// </summary>
	[Table("Attach_File_Storage")]
	[Msgpack.MessagePackObject]
	public partial class DB_AttachFileStorage
	{
		[Key]
		[Msgpack.Key(0)]
		public virtual Guid attachfile_storage_id { get; set; }
		[MaxLength(260)]
		[Msgpack.Key(1)]
		public virtual string attachfile_storage_name { get; set; }
		[MaxLength(260)]
		[Msgpack.Key(2)]
		public virtual string connection_string { get; set; }
		[Msgpack.Key(3)]
		public virtual bool is_full { get; set; }
		[MaxLength(4000)]
		[Msgpack.Key(4)]
		public virtual string attachfile_storage_description { get; set; }
		[Dapper.IgnoreUpdate]
		[Msgpack.Key(5)]
		public virtual DateTime reg_date { get; set; }
		[Dapper.IgnoreUpdate]
		[MaxLength(260)]
		[Msgpack.Key(6)]
		public virtual string reg_username { get; set; }
		[Msgpack.Key(7)]
		public virtual DateTime upd_date { get; set; }
		[MaxLength(260)]
		[Msgpack.Key(8)]
		public virtual string upd_username { get; set; }
		[Msgpack.Key(9)]
		public virtual bool is_active { get; set; }
		[Msgpack.Key(10)]
		public virtual IEnumerable<DB_VendorAttachfilestorage> VendorAttachfilestorage { get; set; }
	}


    /// <summary>
    /// A class which represents the Vendor_Attachfilestorage table.
    /// </summary>
	[Table("Vendor_Attachfilestorage")]
	[Msgpack.MessagePackObject]
	public partial class DB_VendorAttachfilestorage
	{
		[Key]
		[Msgpack.Key(0)]
		public virtual Guid vendor_attachfilestorage_id { get; set; }
		[Msgpack.Key(1)]
		public virtual Guid vendor_id { get; set; }
		[Msgpack.Key(2)]
		public virtual Guid attachfile_storage_id { get; set; }
		[Msgpack.Key(3)]
		public virtual bool is_current { get; set; }
		[Dapper.IgnoreUpdate]
		[Msgpack.Key(4)]
		public virtual DateTime reg_date { get; set; }
		[Dapper.IgnoreUpdate]
		[MaxLength(260)]
		[Msgpack.Key(5)]
		public virtual string reg_username { get; set; }
		[Msgpack.Key(6)]
		public virtual DateTime upd_date { get; set; }
		[MaxLength(260)]
		[Msgpack.Key(7)]
		public virtual string upd_username { get; set; }
		[Msgpack.Key(8)]
		public virtual bool is_active { get; set; }
		[Msgpack.Key(9)]
		public virtual DB_AttachFileStorage DB_AttachFileStorage { get; set; }
	}

}