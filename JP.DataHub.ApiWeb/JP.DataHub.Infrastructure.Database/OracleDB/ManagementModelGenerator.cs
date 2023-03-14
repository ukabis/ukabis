﻿
// This file was automatically generated by the Dapper.SimpleCRUD T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: `Management`
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `Server=tcp:jpdatahubdev.database.windows.net,1433;Initial Catalog=ManagementSqlServer;Persist Security Info=False;User ID=;Password=******;`
//     Include Views:          `True`

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using JP.DataHub.Com.Validations.Attributes;

namespace JP.DataHub.Infrastructure.Database.Management
{
    public static class DatabaseConst
    {
        public const string DATABASE = "Management";

        public static readonly string COLUMN_MAILTEMPLATE_MAIL_TEMPLATE_CD = "mail_template_cd";
        public static readonly string COLUMN_MAILTEMPLATE_MAIL_TEMPLATE_NAME = "mail_template_name";
        public static readonly string COLUMN_MAILTEMPLATE_FROM_MAILADDRESS = "from_mailaddress";
        public static readonly string COLUMN_MAILTEMPLATE_TO_MAILADDRESS = "to_mailaddress";
        public static readonly string COLUMN_MAILTEMPLATE_CC_MAILADDRESS = "cc_mailaddress";
        public static readonly string COLUMN_MAILTEMPLATE_TITLE = "title";
        public static readonly string COLUMN_MAILTEMPLATE_BODY = "body";
        public static readonly string COLUMN_MAILTEMPLATE_REMARK = "remark";
        public static readonly string COLUMN_MAILTEMPLATE_REG_DATE = "reg_date";
        public static readonly string COLUMN_MAILTEMPLATE_REG_USERNAME = "reg_username";
        public static readonly string COLUMN_MAILTEMPLATE_UPD_DATE = "upd_date";
        public static readonly string COLUMN_MAILTEMPLATE_UPD_USERNAME = "upd_username";
        public static readonly string COLUMN_MAILTEMPLATE_IS_ACTIVE = "is_active";
    }

    public enum Tables
    {
        MailTemplate,
    }

    public enum ManagementMailTemplate
    {
        [DbMap("Management","Mail_Template","mail_template_cd")]
        mail_template_cd,
        [DbMap("Management","Mail_Template","mail_template_name")]
        mail_template_name,
        [DbMap("Management","Mail_Template","from_mailaddress")]
        from_mailaddress,
        [DbMap("Management","Mail_Template","to_mailaddress")]
        to_mailaddress,
        [DbMap("Management","Mail_Template","cc_mailaddress")]
        cc_mailaddress,
        [DbMap("Management","Mail_Template","title")]
        title,
        [DbMap("Management","Mail_Template","body")]
        body,
        [DbMap("Management","Mail_Template","remark")]
        remark,
        [DbMap("Management","Mail_Template","reg_date")]
        reg_date,
        [DbMap("Management","Mail_Template","reg_username")]
        reg_username,
        [DbMap("Management","Mail_Template","upd_date")]
        upd_date,
        [DbMap("Management","Mail_Template","upd_username")]
        upd_username,
        [DbMap("Management","Mail_Template","is_active")]
        is_active,
    }


    /// <summary>
    /// A class which represents the MailTemplate table.
    /// </summary>
	[Table("Mail_Template")]
	public partial class DB_MailTemplate
	{
		[Key]
		[MaxLength(30)]
		public virtual string mail_template_cd { get; set; }
		[MaxLength(100)]
		public virtual string mail_template_name { get; set; }
		[MaxLength(1000)]
		public virtual string from_mailaddress { get; set; }
		[MaxLength(1000)]
		public virtual string to_mailaddress { get; set; }
		[MaxLength(1000)]
		public virtual string cc_mailaddress { get; set; }
		[MaxLength(200)]
		public virtual string title { get; set; }
		[MaxLength(2000)]
		public virtual string body { get; set; }
		[MaxLength(1000)]
		public virtual string remark { get; set; }
		[Dapper.IgnoreUpdate]
		public virtual DateTime reg_date { get; set; }
		[Dapper.IgnoreUpdate]
		[MaxLength(260)]
		public virtual string reg_username { get; set; }
		public virtual DateTime upd_date { get; set; }
		[MaxLength(260)]
		public virtual string upd_username { get; set; }
		public virtual bool is_active { get; set; }
	}

}
