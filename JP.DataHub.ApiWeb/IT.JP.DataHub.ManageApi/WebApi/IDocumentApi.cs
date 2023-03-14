using IT.JP.DataHub.ManageApi.WebApi.Models.Document;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Manage/Document", typeof(DocumentModel))]
    public interface IDocumentApi : IAttachFileResource
    {
        [WebApi("GetList?areaToPublic={areaToPublic}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DocumentModel>> GetList(string areaToPublic = null);

        [WebApi("GetAgreementList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DocumentAgreementModel>> GetAgreementList();

        [WebApi("GetCategoryList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DocumentCategoryModel>> GetCategoryList();

        [WebApi("GetDocument?documentId={documentId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<DocumentModel> GetDocument(string documentId);

        [WebApi("GetDocumentList?vendorId={vendorId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<DocumentWithoutFileModel>> GetDocumentList(string vendorId);

        [WebApiPost("RegisterDocument")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<DocumentWithoutFileModel> RegisterDocument(RegisterDocumentModel model);

        [WebApiPost("UpdateDocument")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<DocumentWithoutFileViewModel> UpdateDocument(UpdateDocumentModel model);

        [WebApiDelete("DeleteDocument?documentId={documentId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteDocument(string documentId);
    }
}
