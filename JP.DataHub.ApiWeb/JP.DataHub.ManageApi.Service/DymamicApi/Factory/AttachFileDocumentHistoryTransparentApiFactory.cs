using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Factory
{
    internal static class AttachFileDocumentHistoryTransparentApiFactory
    {
        private static IEnumerable<(TransparentApiType, ActionTypes)> TransparentApiList = new List<(TransparentApiType, ActionTypes)>()
        {
           ( new TransparentApiType(HttpMethodType.MethodType.Get, "GetAttachFileDocumentVersion/{id}", "GetDocumentAttachFileVersion"),ActionTypes.GetDocumentVersion),
           ( new TransparentApiType(HttpMethodType.MethodType.Get, "GetAttachFileDocumentHistory/{FileId}?version={version}", "GetDocumentAttachFileHistory"),ActionTypes.GetAttachFileDocumentHistory),
           ( new TransparentApiType(HttpMethodType.MethodType.Get, "DriveOutAttachFileDocument/{FileId}", "DriveOutAttachFileDocument"),ActionTypes.DriveOutAttachFileDocument),
           ( new TransparentApiType(HttpMethodType.MethodType.Get, "ReturnAttachFileDocument/{FileId}", "ReturnAttachFileDocument"),ActionTypes.ReturnAttachFileDocument),
        };

        private static bool _openIdConnectMustBeUsed;

        public static List<ApiInformationModel> CreateDocumentHistoryTransparentApiInformation(ControllerInformationModel controller, bool openIdConnectMustBeUsed)
        {
            _openIdConnectMustBeUsed = openIdConnectMustBeUsed;

            List<ApiInformationModel> result = new();
            foreach(var transApi in TransparentApiList)
            {
                var api = CreateApiInformation(controller, transApi.Item1.MethodType, transApi.Item1.MethodName, transApi.Item1.Description, transApi.Item2);
                api.SecondaryRepositoryMapList.Add(CreateSecondaryRepositoryMap(controller.AttachFileSettings.BlobRepositoryId, api.ApiId));
                result.Add(api);
            }
            return result;
        }


        private static ApiInformationModel CreateApiInformation(ControllerInformationModel controller, HttpMethodType.MethodType methodType, string apiUrl, string description, ActionTypes actionType)
        {
            return new ApiInformationModel()
            {
                VendorId = controller.VendorId,
                ControllerUrl = controller.Url,
                ApiDescription = description,
                ControllerId = controller.ControllerId,
                MethodType = methodType.ToString(),
                Url = apiUrl,
                RepositoryGroupId = controller.AttachFileSettings.MetaRepositoryId,
                IsEnable = true,
                IsHeaderAuthentication = true,
                IsOpenIdAuthentication = _openIdConnectMustBeUsed,
                ActionType = new ActionType(actionType).Value.GetCode(),
                IsHidden = true,
                IsActive = true,
                ApiAccessVendorList = new(),
                SecondaryRepositoryMapList = new List<SecondaryRepositoryMapModel>(),
                ApiLinkList = new(),
                SampleCodeList = new(),
                IsTransparentApi = true
            };
        }

        private static SecondaryRepositoryMapModel CreateSecondaryRepositoryMap(string repositoryGroupId, string apiId)
        {
            return new SecondaryRepositoryMapModel()
            {
                SecondaryRepositoryMapId = Guid.NewGuid().ToString(),
                RepositoryGroupId = repositoryGroupId,
                ApiId = apiId,
                IsActive = true
            };
        }
    }
}
