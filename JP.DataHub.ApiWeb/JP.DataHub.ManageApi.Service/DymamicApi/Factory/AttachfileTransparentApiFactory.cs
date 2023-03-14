using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Factory
{
    internal static class AttachfileTransparentApiFactory
    {
        private static string DynamicApiAttachfileMateDataSchemaId => UnityCore.Resolve<string>("DynamicApiAttachfileMateDataSchemaId");
        private static string DynamicApiAttachfileCreateResponseDataSchemaId => UnityCore.Resolve<string>("DynamicApiAttachfileCreateResponseDataSchemaId");

        private static bool _openIdConnectMustBeUsed;

        public static List<ApiInformationModel> CreateAttachfileTransparentApiInformation(ControllerInformationModel controllerInformation, bool openIdConnectMustBeUsed)
        {
            _openIdConnectMustBeUsed = openIdConnectMustBeUsed;

            List<ApiInformationModel> result = new()
            {
                CreateCreateAttachFile(controllerInformation),
                CreateUploadAttachFile(controllerInformation),
                CreateGetAttachFile(controllerInformation),
                CreateDeleteAttachFile(controllerInformation),
                CreateGetAttachFileMeta(controllerInformation),
                CreateGetAttachFileMetaList(controllerInformation)
            };
            return result;
        }

        private static ApiInformationModel CreateCreateAttachFile(ControllerInformationModel controllerInformation)
        {
            var discription = "添付ファイルの情報を作成する。";
            var api = CreateApiInformation(controllerInformation, HttpMethodType.MethodType.Post, ActionTypes.Regist, "CreateAttachFile", discription);
            api.RequestSchemaId = DynamicApiAttachfileMateDataSchemaId;
            api.ResponseSchemaId = DynamicApiAttachfileCreateResponseDataSchemaId;

            return api;
        }
        private static ApiInformationModel CreateUploadAttachFile(ControllerInformationModel controllerInformation)
        {
            var discription = @"添付ファイルをアップロードする。
添付ファイルをバイナリで送信する。 1度に送信できる容量は10MBまで。 
それ以上の容量のデータを送信したい場合はContent-Rangeにて送信内容をチャンクしながらアップロードを行う。 
Contents-Typeはapplication/octet-streamで送信する。";
            var api = CreateApiInformation(controllerInformation, HttpMethodType.MethodType.Post, ActionTypes.AttachFileUpload, "UploadAttachFile/{FileId}", discription);
            api.SecondaryRepositoryMapList.Add(CreateSecondaryRepositoryMap(controllerInformation.AttachFileSettings.BlobRepositoryId, api.ApiId));
            return api;
        }
        private static ApiInformationModel CreateGetAttachFile(ControllerInformationModel controllerInformation)
        {
            var discription = "添付ファイルを取得する。";
            var api = CreateApiInformation(controllerInformation, HttpMethodType.MethodType.Get, ActionTypes.AttachFileDownload, "GetAttachFile/{FileId}", discription);
            api.SecondaryRepositoryMapList.Add(CreateSecondaryRepositoryMap(controllerInformation.AttachFileSettings.BlobRepositoryId, api.ApiId));
            return api;
        }
        private static ApiInformationModel CreateDeleteAttachFile(ControllerInformationModel controllerInformation)
        {
            var discription = "添付ファイルを削除する。";
            var api = CreateApiInformation(controllerInformation, HttpMethodType.MethodType.Delete, ActionTypes.AttachFileDelete, "DeleteAttachFile/{FileId}", discription);
            api.SecondaryRepositoryMapList.Add(CreateSecondaryRepositoryMap(controllerInformation.AttachFileSettings.BlobRepositoryId, api.ApiId));
            return api;
        }
        private static ApiInformationModel CreateGetAttachFileMeta(ControllerInformationModel controllerInformation)
        {
            var discription = "添付ファイルのメタ情報を取得する。";
            var api = CreateApiInformation(controllerInformation, HttpMethodType.MethodType.Get, ActionTypes.Query, "GetAttachFileMeta/{FileId}", discription);
            api.ResponseSchemaId = DynamicApiAttachfileMateDataSchemaId;

            return api;
        }
        private static ApiInformationModel CreateGetAttachFileMetaList(ControllerInformationModel controllerInformation)
        {
            var discription = @"アップロードした添付ファイルを検索する。
MetaKey1＝MetaValue1＆MetaKey2＝MetaValue2の形式で検索対象となる添付ファイルのメタをGETパラメータとして指定する。";
            var api = CreateApiInformation(controllerInformation, HttpMethodType.MethodType.Get, ActionTypes.Query, "GetAttachFileMetaList", discription);
            api.ResponseSchemaId = DynamicApiAttachfileMateDataSchemaId;
            api.PostDataType = "array";

            return api;
        }


        private static ApiInformationModel CreateApiInformation(ControllerInformationModel controllerInformation, HttpMethodType.MethodType methodType, ActionTypes actionTypes, string apiUrl, string description)
        {
            return new ApiInformationModel()
            {
                VendorId = controllerInformation.VendorId,
                Url = apiUrl,
                ApiDescription = description,
                ControllerId = controllerInformation.ControllerId,
                MethodType = methodType.ToString(),
                ControllerUrl = controllerInformation.Url,
                RepositoryGroupId = controllerInformation.AttachFileSettings.MetaRepositoryId,
                IsEnable = true,
                IsHeaderAuthentication = true,
                IsOpenIdAuthentication = _openIdConnectMustBeUsed,
                ActionType = new ActionType(actionTypes).Value.GetCode(),
                IsHidden = true,
                IsActive = true,
                ApiAccessVendorList = new List<ApiAccessVendorModel>(),
                SecondaryRepositoryMapList = new List<SecondaryRepositoryMapModel>(),
                ApiLinkList = new List<ApiLinkModel>(),
                SampleCodeList = new List<SampleCodeModel>(),
                IsTransparentApi = true
            };
        }

        private static SecondaryRepositoryMapModel CreateSecondaryRepositoryMap( string repositoryGroupId, string apiId)
        {
            return new SecondaryRepositoryMapModel()
            {
                RepositoryGroupId = repositoryGroupId,
                IsActive = true,
                ApiId = apiId,
            };
        }
    }
}
