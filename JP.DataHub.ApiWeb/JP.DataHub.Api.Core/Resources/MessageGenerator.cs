using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Helper;
using JP.DataHub.Com.Resources;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Api.Core.ErrorCode;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Unity;
using Microsoft.Extensions.Configuration;

// .NET6
// この定義はMessageGenerator.ttから自動生成したものである
// このクラスを直接変更するのではなく、TTテンプレートから自動生成を必ず行うこと
namespace JP.DataHub.Api.Core.ErrorCode
{
    public static partial class ErrorCodeMessage
    {
        private const string MEDIATYPE_JSON = "application/json";

        public enum Code
        {
            [MessageDefinition("OpenIdAuthRequired", "OpenIdAuthRequired_Detail", HttpStatusCode.Forbidden)]
            E01401,
            [MessageDefinition("OpenIdNotAllowedCA", "OpenIdNotAllowedCA_Detail", HttpStatusCode.Forbidden)]
            E01402,
            [MessageDefinition("AccessControlAuthFailed", "OpenidAccessControlAuthFailed_Detail", HttpStatusCode.Forbidden)]
            E01403,
            [MessageDefinition("ToAuthenticateTheOpenIdHasFailed", "AccessControlAuthFailed_Detail", HttpStatusCode.Unauthorized)]
            E01404,
            [MessageDefinition("OpenIdAccessTokenExpired", "OpenIdAccessTokenExpired_Detail", HttpStatusCode.Unauthorized)]
            E01405,
            [MessageDefinition("OpenIdAccessTokenInvalid", "OpenIdAccessTokenInvalid_Detail", HttpStatusCode.Unauthorized)]
            E01406,
            [MessageDefinition("AccessControlAuthFailed", "AccessControlAuthFailed_Detail", HttpStatusCode.Forbidden)]
            E02401,
            [MessageDefinition("VendorSystemAuthRequired", "VendorSystemAuthRequired_Detail", HttpStatusCode.Forbidden)]
            E02402,
            [MessageDefinition("NotAllowedForSystem", "NotAllowedForSystem_Detail", HttpStatusCode.Forbidden)]
            E02403,
            [MessageDefinition("AdminAuthFailed", "AdminAuthFailed_Detail", HttpStatusCode.Forbidden)]
            E02404,
            [MessageDefinition("ClientIdNotFoundOrVendorSystemUnusable", "ClientIdNotFoundOrVendorSystemUnusable_Detail", HttpStatusCode.Unauthorized)]
            E02405,
            [MessageDefinition("VendorSystemAccessTokenNotFound", "VendorSystemAccessTokenNotFound_Detail", HttpStatusCode.Unauthorized)]
            E02406,
            [MessageDefinition("VendorSystemTokenExpired", "", HttpStatusCode.Unauthorized)]
            E02407,
            [MessageDefinition("VendorSystemTokenInvalid", "", HttpStatusCode.Unauthorized)]
            E02408,
            [MessageDefinition("ClientCertificateNotFoundOrVendorSystemUnusable", "ClientCertificateNotFoundOrVendorSystemUnusable_Detail", HttpStatusCode.Unauthorized)]
            E02409,
            [MessageDefinition("ClientCertificateExpired", "ClientCertificateExpired_Detail", HttpStatusCode.Unauthorized)]
            E02410,
            [MessageDefinition("ClientCertificateAuthenticationRequired", "ClientCertificateAuthenticationRequired_Detail", HttpStatusCode.Forbidden)]
            E02411,
            [MessageDefinition("ReferenceFailureToRollBackDueToNotifyOfUpdateFilure", "ReferenceFailureToRollBackDueToNotifyOfUpdateFilure_Detail", HttpStatusCode.BadRequest)]
            E10401,
            [MessageDefinition("OneOrMoreJsonValidationErrorsOccurred", "", HttpStatusCode.BadRequest)]
            E10402,
            [MessageDefinition("OneOrMoreErrorsOccurred", "InvalidMethodType", HttpStatusCode.BadRequest)]
            E10403,
            [MessageDefinition("OneOrMoreErrorsOccurred", "RequestBodyIsEmpty", HttpStatusCode.BadRequest)]
            E10404,
            [MessageDefinition("OneOrMoreErrorsOccurred", "RequestBodyIsArrayOnly", HttpStatusCode.BadRequest)]
            E10405,
            [MessageDefinition("OneOrMoreErrorsOccurred", "KeyPropertyIsRequiredInRequestData", HttpStatusCode.BadRequest)]
            E10406,
            [MessageDefinition("OneOrMoreErrorsOccurred", "NoDataFound", HttpStatusCode.BadRequest)]
            E10407,
            [MessageDefinition("OneOrMoreErrorsOccurred", "TooMachData", HttpStatusCode.BadRequest)]
            E10408,
            [MessageDefinition("OneOrMoreErrorsOccurred", "InvalidTypeExpectedObject_ButGotArray", HttpStatusCode.BadRequest)]
            E10409,
            [MessageDefinition("ResourceIsNotPublic", "", HttpStatusCode.BadRequest)]
            E10410,
            [MessageDefinition("DefinedInVendorNotUsedFlag", "DefinedInVendorNotUsedFlag_Detail", HttpStatusCode.Forbidden)]
            E10411,
            [MessageDefinition("DefinedOtherVendorNotUsedFlag", "NotAgreedAndApproved_Detail", HttpStatusCode.Forbidden)]
            E10412,
            [MessageDefinition("NotAgreedAndApproved", "NotAgreedAndApproved_Detail", HttpStatusCode.Forbidden)]
            E10413,
            [MessageDefinition("VendorOrSystemUnspecified", "", HttpStatusCode.BadRequest)]
            E10414,
            [MessageDefinition("CantCallInternalCallsFromOutside", "", HttpStatusCode.BadRequest)]
            E10415,
            [MessageDefinition("WrongInternalCallingKeyword", "", HttpStatusCode.BadRequest)]
            E10416,
            [MessageDefinition("APIIPFilter", "", HttpStatusCode.Unauthorized)]
            E10417,
            [MessageDefinition("MethodIPFilter", "", HttpStatusCode.Unauthorized)]
            E10418,
            [MessageDefinition("MethodTypeIsIncorrect", "", HttpStatusCode.BadRequest)]
            E10419,
            [MessageDefinition("MissingRepositoryKey", "", HttpStatusCode.BadRequest)]
            E10420,
            [MessageDefinition("DeleteWasSuccessful", "", HttpStatusCode.NotFound)]
            E10421,
            [MessageDefinition("QueryResultsDidNotHaveData.", "", HttpStatusCode.NotFound)]
            E10422,
            [MessageDefinition("WeCallTheGatewayDidNotHaveTheData", "", HttpStatusCode.NotFound)]
            E10423,
            [MessageDefinition("GatewayURLDefinedError", "", HttpStatusCode.BadRequest)]
            E10424,
            [MessageDefinition("YouCanNotUseIt", "HttpMethodTypeDoesNotSupport", HttpStatusCode.NotImplemented)]
            E10425,
            [MessageDefinition("SyntaxErrorOfOData", "SyntaxErrorOfOData_Detail", HttpStatusCode.BadRequest)]
            E10426,
            [MessageDefinition("IMadeASearchByODataDidNotHaveTheData", "", HttpStatusCode.NotFound)]
            E10427,
            [MessageDefinition("ItWasExecutedODataDelete", "", HttpStatusCode.NotFound)]
            E10428,
            [MessageDefinition("ExceptionOccured", "", HttpStatusCode.BadRequest)]
            E10429,
            [MessageDefinition("IsTransparentApiIncorrect", "", HttpStatusCode.BadRequest)]
            E10430,
            [MessageDefinition("QuerySyntaxErrorException", "InvalidApiQuerySql_ResourceIdSpecified", HttpStatusCode.BadRequest)]
            E10431,
            [MessageDefinition("QuerySyntaxErrorException", "InvalidApiQuerySql_TableToJoinNotFound", HttpStatusCode.BadRequest)]
            E10432,
            [MessageDefinition("QuerySyntaxErrorException", "InvalidApiQuerySql_TableJoinNotAllowed", HttpStatusCode.BadRequest)]
            E10433,
            [MessageDefinition("QuerySyntaxErrorException", "InvalidApiQuerySql_SelectInvalidColumn", HttpStatusCode.BadRequest)]
            E10434,
            [MessageDefinition("ODataPatchUnavailable", "ODataPatchUnavailable_NotSupportedRepository", HttpStatusCode.BadRequest)]
            E10435,
            [MessageDefinition("ODataPatchUnavailable", "ODataPatchUnavailable_OptimisticConcurrencyEnabled", HttpStatusCode.BadRequest)]
            E10436,
            [MessageDefinition("ODataPatchUnavailable", "ODataPatchUnavailable_UnsaportedFeaturesEnabled", HttpStatusCode.BadRequest)]
            E10437,
            [MessageDefinition("ODataPatchRequestInvalid", "", HttpStatusCode.BadRequest)]
            E10438,
            [MessageDefinition("ODataPatchRequestInvalid", "ODataPatchRequestInvalid_UneditableProperty", HttpStatusCode.BadRequest)]
            E10439,
            [MessageDefinition("ODataPatchRequestInvalid", "ODataPatchRequestInvalid_AdditionalConditionInvalid", HttpStatusCode.BadRequest)]
            E10440,
            [MessageDefinition("OperatingVendorUserOnly", "OperatingVendorUserOnly_Detail", HttpStatusCode.Forbidden)]
            E10441,
            [MessageDefinition("DataEncryptionError", "DataEncryptionError_CommonKeyNotFound", HttpStatusCode.BadRequest)]
            E10442,
            [MessageDefinition("DataEncryptionError", "DataEncryptionError_DecryptionFailed", HttpStatusCode.BadRequest)]
            E10443,
            [MessageDefinition("AnInternalServerErrorHasOccurred", "", HttpStatusCode.InternalServerError)]
            E10501,
            [MessageDefinition("ExceptionOccured", "", HttpStatusCode.InternalServerError)]
            E10502,
            [MessageDefinition("AsyncDynamicApiError", "", HttpStatusCode.InternalServerError)]
            E10503,
            [MessageDefinition("FileIdRequired", "FileIdRequired_Detail", HttpStatusCode.BadRequest)]
            E20401,
            [MessageDefinition("KeyMissMatch", "KeyMissMatch_Detail", HttpStatusCode.BadRequest)]
            E20402,
            [MessageDefinition("YouCanNotUseIt", "YouCanNotUseIt_Detail", HttpStatusCode.NotImplemented)]
            E20403,
            [MessageDefinition("HttpMethodTypeDoesNotMatch", "HttpMethodTypeDoesNotMatch_Detail", HttpStatusCode.BadRequest)]
            E20404,
            [MessageDefinition("RepositoryKeyHasNotBeenSet", "RepositoryKeyHasNotBeenSet_Detail", HttpStatusCode.BadRequest)]
            E20405,
            [MessageDefinition("DoesNotExistDataToBeUpdatedIs", "DoesNotExistDataToBeUpdatedIs_Detail", HttpStatusCode.NotFound)]
            E20406,
            [MessageDefinition("YouCanNotUseIt", "YouCanNotUseIt_Detail", HttpStatusCode.BadRequest)]
            E20407,
            [MessageDefinition("MethodTypeDoesNotMatch", "MethodTypeDoesNotMatch_Detail", HttpStatusCode.BadRequest)]
            E20408,
            [MessageDefinition("RepositoryKeyHasNotBeenSet", "RepositoryKeyHasNotBeenSet_Detail", HttpStatusCode.NotFound)]
            E20409,
            [MessageDefinition("FileIdNoSettingRequired", "", HttpStatusCode.BadRequest)]
            E20410,
            [MessageDefinition("BlockedRequestedContentTypeOrFileTypeByList", "BlockedRequestedContentTypeOrFileTypeByList_Detail", HttpStatusCode.BadRequest)]
            E20411,
            [MessageDefinition("ExternalAttachiFileError", "ExternalAttachiFileError_UnnecessaryUpload", HttpStatusCode.BadRequest)]
            E20412,
            [MessageDefinition("ExternalAttachiFileError", "ExternalAttachiFileError_FileInfoMissing", HttpStatusCode.BadRequest)]
            E20413,
            [MessageDefinition("ExternalAttachiFileError", "ExternalAttachiFileError_InvalidFileInfo", HttpStatusCode.BadRequest)]
            E20414,
            [MessageDefinition("ExternalAttachiFileError", "ExternalAttachiFileError_ConnectionFailed", HttpStatusCode.BadRequest)]
            E20415,
            [MessageDefinition("ExternalAttachiFileError", "ExternalAttachiFileError_FileNotFound", HttpStatusCode.BadRequest)]
            E20416,
            [MessageDefinition("InvalidAttachFileName", "", HttpStatusCode.BadRequest)]
            E20417,
            [MessageDefinition("ExternalAttachiFileError", "ExternalAttachiFileError_DocumentHistoryUnsupported", HttpStatusCode.BadRequest)]
            E20418,
            [MessageDefinition("OneOrMoreErrorsOccurred", "XReferenceHeaderIsInvalid", HttpStatusCode.BadRequest)]
            E30401,
            [MessageDefinition("IdIsRequired", "IdIsRequired", HttpStatusCode.BadRequest)]
            E30402,
            [MessageDefinition("VersionIsRequired", "VersionIsRequired", HttpStatusCode.BadRequest)]
            E30403,
            [MessageDefinition("IdDoesNotMatch", "IdDoesNotMatch", HttpStatusCode.NotFound)]
            E30404,
            [MessageDefinition("TypeOfVersionIsIntOrGuid", "TypeOfVersionIsIntOrGuid", HttpStatusCode.BadRequest)]
            E30405,
            [MessageDefinition("VersionDoesNotExistInHistory", "VersionDoesNotExistInHistory", HttpStatusCode.NotFound)]
            E30406,
            [MessageDefinition("DocumentHasBeenDeleted", "DocumentHasBeenDeleted", HttpStatusCode.NotFound)]
            E30407,
            [MessageDefinition("DataIsAlreadyRegistered", "DataIsAlreadyRegistered", HttpStatusCode.BadRequest)]
            E30408,
            [MessageDefinition("HistoryDataNotFound", "HistoryDataNotFound", HttpStatusCode.NotFound)]
            E30409,
            [MessageDefinition("MethodTypeIsIncorrect", "MethodTypeIsIncorrect", HttpStatusCode.BadRequest)]
            E30410,
            [MessageDefinition("HistoryDatNotFound", "HistoryDatNotFound", HttpStatusCode.NotFound)]
            E30411,
            [MessageDefinition("FailedWithDriveOutTransparentAPI", "FailedWithDriveOutTransparentAPI_Detail", HttpStatusCode.NotFound)]
            E30412,
            [MessageDefinition("ThisURLDoesNotSupportDocumentHistory", "", HttpStatusCode.NotImplemented)]
            E30501,
            [MessageDefinition("BlockchainValidate_AttachFile_NoSource", "BlockchainValidate_AttachFile_NoSource_Detail", HttpStatusCode.OK)]
            E40201,
            [MessageDefinition("BlockchainValidate_AttachFile_NoBlockchainHash", "BlockchainValidate_AttachFile_NoBlockchainHash_Detail", HttpStatusCode.OK)]
            E40202,
            [MessageDefinition("BlockchainValidate_AttachFile_Mismatch", "BlockchainValidate_AttachFile_Mismatch_Detail", HttpStatusCode.OK)]
            E40203,
            [MessageDefinition("BlockchainValidate_NoSource", "BlockchainValidate_NoSource_Detail", HttpStatusCode.OK)]
            E40204,
            [MessageDefinition("BlockchainValidate_NoBlockchainHash", "BlockchainValidate_NoBlockchainHash_Detail", HttpStatusCode.OK)]
            E40205,
            [MessageDefinition("BlockchainValidate_Mismatch", "BlockchainValidate_Mismatch_Detail", HttpStatusCode.OK)]
            E40206,
            [MessageDefinition("BlockchainValidate_DocumentHistory_Disable", "BlockchainValidate_DocumentHistory_Disable_Detail", HttpStatusCode.BadRequest)]
            E40401,
            [MessageDefinition("BlockchainValidate_DocumentHistory_VersionNotFound", "BlockchainValidate_DocumentHistory_VersionNotFound_Detail", HttpStatusCode.BadRequest)]
            E40402,
            [MessageDefinition("HttpResponseException", "", HttpStatusCode.NotFound)]
            E50401,
            [MessageDefinition("RoslynScriptRuntimeException", "", HttpStatusCode.InternalServerError)]
            E50402,
            [MessageDefinition("AggregateException", "", HttpStatusCode.InternalServerError)]
            E50403,
            [MessageDefinition("XVersionNotFoundException", "InvalidXVersion", HttpStatusCode.NotFound)]
            E50404,
            [MessageDefinition("QuerySyntaxErrorException", "QuerySyntaxError", HttpStatusCode.BadRequest)]
            E50405,
            [MessageDefinition("ODataException", "", HttpStatusCode.BadRequest)]
            E50406,
            [MessageDefinition("ApiException", "", HttpStatusCode.InternalServerError)]
            E50407,
            [MessageDefinition("NotParseCsvException", "CsvParseFailed", HttpStatusCode.BadRequest)]
            E50408,
            [MessageDefinition("ResourceSchemaNotAdaptable", "ResourceSchemaNotAdaptable_ResourceSchemaRequired", HttpStatusCode.BadRequest)]
            E50409,
            [MessageDefinition("ResourceSchemaNotAdaptable", "ResourceSchemaNotAdaptable_OneOrMoreErrorsOccurred", HttpStatusCode.BadRequest)]
            E50410,
            [MessageDefinition("QuerySyntaxErrorException", "InvalidApiQuerySql_TableNotFound", HttpStatusCode.BadRequest)]
            E50411,
            [MessageDefinition("Dummy", "", HttpStatusCode.BadRequest)]
            E99998,
            [MessageDefinition("Exception", "", HttpStatusCode.InternalServerError)]
            E99999,
            [MessageDefinition("InvalidResourceAgreement", "InvalidResourceAgreement_Detail", HttpStatusCode.BadRequest)]
            E60401,
            [MessageDefinition("InvaliOtherResourceSqlAccessRepositoryType", "InvaliOtherResourceSqlAccessRepositoryType_Detail", HttpStatusCode.BadRequest)]
            E60402,
            [MessageDefinition("TermsGroupNotFound", "TermsGroupNotFound_Detail", HttpStatusCode.NotFound)]
            W60403,
            [MessageDefinition("NotDeleteTermsGroupKeyNotMatch", "NotDeleteTermsGroupKeyNotMatch_Detail", HttpStatusCode.NotFound)]
            W60404,
            [MessageDefinition("NotUpdateTermsGroupKeyNotMatch", "NotUpdateTermsGroupKeyNotMatch_Detail", HttpStatusCode.NotFound)]
            W60405,
            [MessageDefinition("TermsNotFound", "TermsNotFound_Detail", HttpStatusCode.NotFound)]
            W60406,
            [MessageDefinition("NotDeleteTermsKeyNotMatch", "NotDeleteTermsKeyNotMatch_Detail", HttpStatusCode.NotFound)]
            W60407,
            [MessageDefinition("NotUpdateTermsGKeyNotMatch", "NotUpdateTermsGKeyNotMatch_Detail", HttpStatusCode.NotFound)]
            W60408,
            [MessageDefinition("NotAgreementNoTerms", "NotAgreementNoTerms_Detail", HttpStatusCode.NotFound)]
            W60409,
            [MessageDefinition("NoAgreementAlreadyAgreement", "NoAgreementAlreadyAgreement_Detail", HttpStatusCode.BadRequest)]
            E60410,
            [MessageDefinition("NoRevokeNotAgreement", "NoRevokeNotAgreement_Detail", HttpStatusCode.BadRequest)]
            E60411,
            [MessageDefinition("UserTermsNotFound", "UserTermsNotFound_Detail", HttpStatusCode.NotFound)]
            W60412,
            [MessageDefinition("CertifiedApplicationNotFound", "CertifiedApplicationNotFound_Detail", HttpStatusCode.NotFound)]
            W60413,
            [MessageDefinition("NoValidVendorOrSystem", "NoValidVendorOrSystem_Detail", HttpStatusCode.BadRequest)]
            E60414,
            [MessageDefinition("NotDeleteCertifiedApplicationNotMatch", "NotDeleteCertifiedApplicationNotMatch_Detail", HttpStatusCode.NotFound)]
            W60415,
            [MessageDefinition("UserGroupNotFound", "UserGroupNotFound_Detail", HttpStatusCode.NotFound)]
            W60416,
            [MessageDefinition("NotDeleteUserGroupKeyNotMatch", "NotDeleteUserGroupKeyNotMatch_Detail", HttpStatusCode.NotFound)]
            W60417,
            [MessageDefinition("UserResourceShare_GetList_NotFound", "UserResourceShare_GetList_NotFound_Detail", HttpStatusCode.NotFound)]
            W60418,
            [MessageDefinition("UserResourceShare_Register_UnMatchKey", "UserResourceShare_Register_UnMatchKey_Detail", HttpStatusCode.NotFound)]
            W60419,
            [MessageDefinition("UserResourceShare_Register_UserShareTypeCodeMissMatch", "UserResourceShare_Register_UserShareTypeCodeMissMatch_Detail", HttpStatusCode.BadRequest)]
            E60420,
            [MessageDefinition("UserResourceShare_Delete_KeyNotMatch", "UserResourceShare_Delete_KeyNotMatch_Detail", HttpStatusCode.BadRequest)]
            E60421,
            [MessageDefinition("Revoke_Start_NotAgreement", "Revoke_Start_NotAgreement_Detail", HttpStatusCode.BadRequest)]
            E60422,
            [MessageDefinition("Revoke_Start_FK", "Revoke_Start_FK_Detail", HttpStatusCode.BadRequest)]
            E60423,
            [MessageDefinition("Revoke_Stop_NotAgreement", "Revoke_Stop_NotAgreement_Detail", HttpStatusCode.BadRequest)]
            E60424,
            [MessageDefinition("Revoke_Stop_NotStart", "Revoke_Stop_NotStart_Detail", HttpStatusCode.BadRequest)]
            E60425,
            [MessageDefinition("Revoke_RemoveResourceStart_Fail", "Revoke_RemoveResourceStart_Fail_Detail", HttpStatusCode.BadRequest)]
            E60426,
            [MessageDefinition("Revoke_RemoveResourceEnd_NotFound", "Revoke_RemoveResourceEnd_NotFound_Detail", HttpStatusCode.BadRequest)]
            E60427,
            [MessageDefinition("Revoke_RemoveResourceEnd_Fail", "Revoke_RemoveResourceEnd_Fail_Detail", HttpStatusCode.BadRequest)]
            E60428,
            [MessageDefinition("ResourceGroup_GetList_NotFound", "ResourceGroup_GetList_NotFound_Detail", HttpStatusCode.NotFound)]
            W60429,
            [MessageDefinition("ResourceGroup_Register_NotFound", "ResourceGroup_Register_NotFound_Detail", HttpStatusCode.BadRequest)]
            E60430,
            [MessageDefinition("ResourceGroup_Delete_NotFound", "ResourceGroup_Delete_NotFound_Detail", HttpStatusCode.BadRequest)]
            E60431,
            [MessageDefinition("HasDefinedKeywordDataSchemaException", "", HttpStatusCode.BadRequest)]
            E70401,
            [MessageDefinition("InvalidAccountFormatException", "", HttpStatusCode.BadRequest)]
            E80401,
            [MessageDefinition("IMadeASearchByODataDidNotHaveTheData", "", HttpStatusCode.NotFound)]
            I10401,
            [MessageDefinition("ODataPatchNotFound", "", HttpStatusCode.NotFound)]
            I10402,
            [MessageDefinition("QueryResultsDidNotHaveData.", "", HttpStatusCode.NotFound)]
            I10403,
            [MessageDefinition("MustAgreeToTheTerms", "", HttpStatusCode.Forbidden)]
            E50412,
            [MessageDefinition("AnOpenIDWasSpecifiedThatIsNotAllowedToBeShared", "An OpenID was specified that is not allowed to be shared", HttpStatusCode.Forbidden)]
            E50413,
            [MessageDefinition("CannotSetX-UserResourceSharingWhenIsContainerDynamicSeparationIsSet", "CannotSetX-UserResourceSharingWhenIsContainerDynamicSeparationIsSet", HttpStatusCode.BadRequest)]
            E50414,
            [MessageDefinition("X-UserResourceSharingCanBeSpecifiedForDataRetrieval", "X-UserResourceSharingCanBeSpecifiedForDataRetrieval", HttpStatusCode.BadRequest)]
            E50415,
        }

        private static Lazy<System.Resources.ResourceManager> _resourceManager = new Lazy<System.Resources.ResourceManager>(() =>
        {
            var list = RuntimeReflectionExtensions.GetRuntimeProperties(typeof(DynamicApiMessages)).ToList();
            var prop = list?.Where(x => x.Name == "ResourceManager").FirstOrDefault();
            var result = (System.Resources.ResourceManager)prop?.GetValue(null);
            return result;
        });

        private static System.Resources.ResourceManager ResourceManager => _resourceManager.Value;

        public static string GetString(string resourceName, CultureInfo cultureInfo)
        {
            IDataContainer dataContainer;
            try
            {
                dataContainer = UnityCore.Resolve<IDataContainer>();
            }
            catch (ResolutionFailedException)
            {
                dataContainer = UnityCore.Resolve<IDataContainer>("multiThread");
            }
            return ResourceManager.GetString(resourceName, cultureInfo);
        }

        public static string GetString(string resourceName)
        {
            IDataContainer dataContainer;
            try
            {
                dataContainer = UnityCore.Resolve<IDataContainer>();
            }
            catch (ResolutionFailedException)
            {
                dataContainer = UnityCore.Resolve<IDataContainer>("multiThread");
            }
            return ResourceManager.GetString(resourceName, dataContainer.CultureInfo);
        }

        public static RFC7807ProblemDetailExtendErrors GetRFC7807(this Code errorCode, CultureInfo cultureInfo, string relativeUrl = null)
        {
            // エラーの変換が定義されていればエラーコードを変換する
            var newcodestring = UnityCore.Resolve<IConfiguration>().GetValue<string>($"Rfc7807ErrorMap:{errorCode}.To", null);
            if (!string.IsNullOrEmpty(newcodestring) && newcodestring.TryParse<Code>(out var newcode))
            {
                errorCode = newcode;
            }

            var rpdc = new RFC7807ProblemDetailExtendErrors();
            if (string.IsNullOrEmpty(relativeUrl) == false)
            {
                rpdc.Instance = new Uri(relativeUrl, UriKind.Relative);
            }
            rpdc.ErrorCode = errorCode.ToString();

            var attr = errorCode.GetAttribute<MessageDefinitionAttribute>();
            if (attr != null)
            {
                rpdc.Title = GetString(attr.Message, cultureInfo);
                rpdc.Detail = GetString(attr.Detail, cultureInfo);
                rpdc.Status = (int)attr.HttpStatusCode;
            }
            return rpdc;
        }

        public static RFC7807ProblemDetailExtendErrors GetRFC7807(this Code errorCode, string relativeUrl = null)
        {
            // エラーの変換が定義されていればエラーコードを変換する
            var newcodestring = UnityCore.Resolve<IConfiguration>().GetValue<string>($"Rfc7807ErrorMap:{errorCode}.To", null);
            if (!string.IsNullOrEmpty(newcodestring) && newcodestring.TryParse<Code>(out var newcode))
            {
                errorCode = newcode;
            }

            var rpdc = new RFC7807ProblemDetailExtendErrors();
            if (string.IsNullOrEmpty(relativeUrl) == false)
            {
                rpdc.Instance = new Uri(relativeUrl, UriKind.Relative);
            }
            rpdc.ErrorCode = errorCode.ToString();

            var attr = errorCode.GetAttribute<MessageDefinitionAttribute>();
            if (attr != null)
            {
                rpdc.Title = GetString(attr.Message);
                rpdc.Detail = GetString(attr.Detail);
                rpdc.Status = (int)attr.HttpStatusCode;
            }
            return rpdc;
        }

        public static HttpResponseMessage GetRFC7807HttpResponseMessage(Code errorCode, CultureInfo cultureInfo, string relativeUrl = null, string title = null, string detail = null)
        {
            var error = GetRFC7807(errorCode, relativeUrl);
            return GetRFC7807HttpResponseMessage(error, relativeUrl, title, detail);
        }

        public static HttpResponseMessage GetRFC7807HttpResponseMessage(Code errorCode, string relativeUrl = null, string title = null, string detail = null)
        {
            var error = GetRFC7807(errorCode, relativeUrl);
            return GetRFC7807HttpResponseMessage(error, relativeUrl, title, detail);
        }

        public static HttpResponseMessage GetRFC7807HttpResponseMessage(RFC7807ProblemDetailExtendErrors error, string relativeUrl = null, string title = null, string detail = null)
        {
            if (string.IsNullOrEmpty(relativeUrl) == false)
            {
                error.Instance = new Uri(relativeUrl, UriKind.Relative);
            }

            if (string.IsNullOrEmpty(title) == false)
            {
                error.Title = title;
            }

            if (string.IsNullOrEmpty(detail) == false)
            {
                error.Detail = detail;
            }

            return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MEDIATYPE_JSON) };
        }
    }
}

