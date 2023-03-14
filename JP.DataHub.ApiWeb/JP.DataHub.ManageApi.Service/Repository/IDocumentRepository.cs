using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IDocumentRepository
    {
        DocumentInformationModel GetDocumentInformation(string documentId);
        IList<DocumentInformationModel> GetDocumentInformation(bool isPublicPortal, bool isPublicAdmin);
        IList<DocumentCategoryModel> GetDocumentCategory();
        IList<DocumentModel> GetDocumentList(string vendorId, bool? isEnable = null, bool? isAdminCheck = null, bool? isAdminStop = null, bool? isActive = null,
            bool? isPublicPortal = null, bool? isPublicAdmin = null, bool? isPublicPortalHidden = null, bool? isPublicAdminHidden = null);
        DocumentModel Register(DocumentModel model, IList<DocumentFileModel> files = null);
        DocumentModel Update(DocumentModel model, IList<DocumentFileModel> files = null);
        void DeleteDocument(string documentId);
        IList<DocumentFileModel> GetDocumentFileList(string documentId, string documentTitle);
        IList<string> GetDocumentOwnerEmailAddressList(string documentId);
        VendorSystemCategoryNameResultModel GetVendorSystemCategoryName(string vendorId, string systemId, string categoryId);
    }
}
