using JP.DataHub.ManageApi.Service.DymamicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Consts
{
    internal static class ApiListConst
    {
        internal static IEnumerable<TransparentApiType> BaseTransparentApiList = new List<TransparentApiType>()
        {
            new TransparentApiType(HttpMethodType.MethodType.Post, "SetNewVersion", "SetNewVersion"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetCount", "GetCount"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetCurrentVersion", "GetCurrentVersion"),
            new TransparentApiType(HttpMethodType.MethodType.Post, "CreateRegisterVersion", "CreateRegisterVersion"),
            new TransparentApiType(HttpMethodType.MethodType.Post, "CompleteRegisterVersion", "CompleteRegisterVersion"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetRegisterVersion", "GetRegisterVersion"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetVersionInfo", "GetVersionInfo"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "OData", "OData"),
            new TransparentApiType(HttpMethodType.MethodType.Delete, "ODataDelete", "ODataDelete"),
            new TransparentApiType(HttpMethodType.MethodType.Patch, "ODataPatch", "ODataPatch"),
            new TransparentApiType(HttpMethodType.MethodType.Post, "AdaptResourceSchema", "AdaptResourceSchema"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetResourceSchema", "GetResourceSchema"),
        };

        internal static IEnumerable<TransparentApiType> AllTransparentApiList = new List<TransparentApiType>()
        {
            new TransparentApiType(HttpMethodType.MethodType.Post, "CreateAttachFile", "CreateAttachFile"),
            new TransparentApiType(HttpMethodType.MethodType.Delete, "DeleteAttachFile/{FileId}", "DeleteAttachFile/{FileId}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "DriveOutAttachFileDocument/{FileId}", "DriveOutAttachFileDocument/{FileId}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "DriveOutDocument", "DriveOutDocument"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetAttachFile/{FileId}", "GetAttachFile/{FileId}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetAttachFileDocumentHistory/{FileId}?version={version}", "GetAttachFileDocumentHistory/{FileId}?version={version}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetAttachFileDocumentVersion/{id}", "GetAttachFileDocumentVersion/{id}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetAttachFileMeta/{FileId}", "GetAttachFileMeta/{FileId}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetAttachFileMetaList", "GetAttachFileMetaList"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetDocumentHistory", "GetDocumentHistory"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "GetDocumentVersion", "GetDocumentVersion"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "ReturnAttachFileDocument/{FileId}", "ReturnAttachFileDocument/{FileId}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "ReturnDocument", "ReturnDocument"),
            new TransparentApiType(HttpMethodType.MethodType.Post, "UploadAttachFile/{FileId}", "UploadAttachFile/{FileId}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "ValidateAttachFileWithBlockchain/{fileid}", "ValidateAttachFileWithBlockchain/{fileid}"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "ValidateWithBlockchain/{id}", "ValidateWithBlockchain/{id}"),
            new TransparentApiType(HttpMethodType.MethodType.Post, "RegisterRawData", "RegisterRawData"),
            new TransparentApiType(HttpMethodType.MethodType.Get, "ODataRawData", "ODataRawData")
        }.Concat(BaseTransparentApiList);
    }
}
