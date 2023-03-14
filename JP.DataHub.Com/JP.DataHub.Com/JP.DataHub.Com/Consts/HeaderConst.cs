using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Consts
{
    public static class HeaderConst
    {
        // system reserved
        public static readonly string ContentType = "Content-Type";
        public static readonly string ContentLength = "Content-Length";
        public static readonly string Accept = "Accept";
        public static readonly string ContentRange = "Content-Range";
        public static readonly string TransferEncoding = "Transfer-Encoding";

        // reserved
        public static readonly string LoggingLogId = "LoggingLogId";
        public static readonly string X_GetInternalAllField = "X-GetInternalAllField";
        public static readonly string X_RequestContinuation = "X-RequestContinuation";
        public static readonly string X_ResponseContinuation = "X-ResponseContinuation";
        public static readonly string X_DocumentHistory = "X-DocumentHistory";
        public static readonly string X_ReferenceHistory = "X-ReferenceHistory";
        public static readonly string X_ResourceSharingWith = "X-ResourceSharingWith";
        public static readonly string X_ResourceSharingPerson = "X-ResourceSharingPerson";
        public static readonly string X_Cache = "X-Cache";
        public static readonly string X_NoOptimistic = "X-NoOptimistic";
        public static readonly string X_RegisterConflictStop = "X-RegisterConflictStop";
        public static readonly string X_ScriptRuntimeLogId = "X-ScriptRuntimeLog-Id";
        public static readonly string X_ScriptRuntimeLogException = "X-ScriptRuntimeLog-Exception";
        public static readonly string X_Version = "X-Version";
        public static readonly string X_IsAsync = "X-IsAsync";
        public static readonly string X_UserResourceSharing = "X-UserResourceSharing";

        // Authentication
        public static readonly string Authorization = "Authorization";
        public static readonly string Bearer = "Bearer";
        public static readonly string XAuthorization = "X-Authorization";
        public static readonly string XAdmin = "X-Admin";
        public static readonly string XVendor = "X-Vendor";
        public static readonly string XSystem = "X-System";
    }
}
