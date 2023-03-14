using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Factory
{
    internal static class DocumentHistoryTransparentApiFactory
    {
        private static IEnumerable<(TransparentApiType, ActionTypes)> TransparentApiList = new List<(TransparentApiType, ActionTypes)>()
        {
           ( new TransparentApiType(HttpMethodType.MethodType.Get, "GetDocumentVersion", "GetDocumentVersion"),ActionTypes.GetDocumentVersion),
           ( new TransparentApiType(HttpMethodType.MethodType.Get, "GetDocumentHistory", "GetDocumentHistory"),ActionTypes.GetDocumentHistory),
           ( new TransparentApiType(HttpMethodType.MethodType.Get, "DriveOutDocument", "DriveOutDocument"),ActionTypes.DriveOutDocument),
           ( new TransparentApiType(HttpMethodType.MethodType.Get, "ReturnDocument", "ReturnDocument"),ActionTypes.ReturnDocument),
           // 表示しないようにするため↓はコメントアウト
           // ( new TransparentApiType(HttpMethodType.MethodType.Delete, "HistoryThrowAway", "HistoryThrowAway"),ActionTypes.HistoryThrowAway),
        };

        private static bool _openIdConnectMustBeUsed;

        public static List<ApiInformationModel> CreateDocumentHistoryTransparentApiInformation(ControllerInformationModel controller, bool openIdConnectMustBeUsed)
        {
            _openIdConnectMustBeUsed = openIdConnectMustBeUsed;
            List<ApiInformationModel> result = new();
            foreach(var transApi in TransparentApiList)
            {
                result.Add(CreateApiInformation(controller, transApi.Item1.MethodType, transApi.Item1.MethodName, transApi.Item1.Description, transApi.Item2));
            }

            return result;
        }


        private static ApiInformationModel CreateApiInformation(ControllerInformationModel controller, HttpMethodType.MethodType methodType, string apiUrl, string description, ActionTypes actionTypes)
        {
            return new ApiInformationModel()
            {
                VendorId = controller.VendorId,
                ControllerUrl = controller.Url,
                ApiDescription = description,
                ControllerId = controller.ControllerId,
                MethodType = methodType.ToString(),
                Url = apiUrl,
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
                IsTransparentApi = true,
            };
        }
    }
}
