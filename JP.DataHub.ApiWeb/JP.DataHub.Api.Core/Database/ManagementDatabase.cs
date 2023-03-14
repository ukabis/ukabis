using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Database
{
    public static class ManagementDatabase
    {
#if Oracle
        public const string DATABASE = "MANAGEMENT";
#else
        public const string DATABASE = "Management";
#endif
        public const string TABLE_MAILTEMPLATE = "MailTemplate";
        public const string COLUMN_MAILTEMPLATE_MAIL_TEMPLATE_CD = "mail_template_cd";
        public const string COLUMN_MAILTEMPLATE_MAIL_TEMPLATE_NAME = "mail_template_name";
        public const string COLUMN_MAILTEMPLATE_FROM_MAILADDRESS = "from_mailaddress";
        public const string COLUMN_MAILTEMPLATE_TO_MAILADDRESS = "to_mailaddress";
        public const string COLUMN_MAILTEMPLATE_CC_MAILADDRESS = "cc_mailaddress";
        public const string COLUMN_MAILTEMPLATE_TITLE = "title";
        public const string COLUMN_MAILTEMPLATE_BODY = "body";
        public const string COLUMN_MAILTEMPLATE_REMARK = "remark";
        public const string COLUMN_MAILTEMPLATE_REG_DATE = "reg_date";
        public const string COLUMN_MAILTEMPLATE_REG_USERNAME = "reg_username";
        public const string COLUMN_MAILTEMPLATE_UPD_DATE = "upd_date";
        public const string COLUMN_MAILTEMPLATE_UPD_USERNAME = "upd_username";
        public const string COLUMN_MAILTEMPLATE_IS_ACTIVE = "is_active";
    }
}
