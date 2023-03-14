using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Factory
{
    internal static class BlockchainTransparentApiFactory
    {
        private static bool _openIdConnectMustBeUsed;

        public static List<ApiInformationModel> CreateBlockchainTransparentApiInformation(ControllerInformationModel controller, bool openIdConnectMustBeUsed)
        {
            _openIdConnectMustBeUsed = openIdConnectMustBeUsed;

            List<ApiInformationModel> result = new()
            {
                CreateValidateWithBlockchainById(controller)
            };

            if (controller.AttachFileSettings?.IsEnable ?? false)
            {
                result.Add(CreateValidateAttachFileWithBlockchainByFileId(controller, controller.AttachFileSettings.MetaRepositoryId));
            }
            return result;
        }

        private static ApiInformationModel CreateValidateWithBlockchainById(ControllerInformationModel controller)
        {
            var discription = "CosmosDBのデータとブロックチェーンのデータを突合して改ざんの有無を検証する";
            var api = CreateApiInformation(controller, HttpMethodType.MethodType.Get, ActionTypes.Query, "ValidateWithBlockchain/{id}", discription, null);
            return api;
        }

        private static ApiInformationModel CreateValidateAttachFileWithBlockchainByFileId(ControllerInformationModel controllerInformation, string repositoryGroupId)
        {
            var discription = "DynamicApiのAttachFileのデータとブロックチェーンのデータを突合して改ざんの有無を検証する";
            var api = CreateApiInformation(controllerInformation, HttpMethodType.MethodType.Get, ActionTypes.Query, "ValidateAttachFileWithBlockchain/{fileid}", discription, repositoryGroupId);
            api.SecondaryRepositoryMapList.Add(CreateSecondaryRepositoryMap(controllerInformation.AttachFileSettings.BlobRepositoryId, api.ApiId));
            return api;
        }


        private static ApiInformationModel CreateApiInformation(ControllerInformationModel controller, HttpMethodType.MethodType methodType, ActionTypes actionTypes, string apiUrl, string description, string repositoryGroupId)
        {
            return new ApiInformationModel()
            {
                VendorId = controller.VendorId,
                ControllerUrl = controller.Url,
                ApiDescription = description,
                ControllerId = controller.ControllerId,
                MethodType = methodType.ToString(),
                Url = apiUrl,
                RepositoryGroupId = repositoryGroupId,
                IsEnable = true,
                IsHeaderAuthentication = true,
                IsOpenIdAuthentication = _openIdConnectMustBeUsed,
                ActionType = new ActionType(actionTypes).Value.GetCode(),
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
