using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    internal interface IDocumentService
    {
        DocumentInformationModel GetDocumentInformation(string documentId);
        IList<DocumentInformationModel> GetDocumentInformation(bool isPublicPortal, bool isPublicAdmin);
        IList<DocumentCategoryModel> GetDocumentCategory();
        IList<DocumentModel> GetDocumentList(string vendorId, bool? isEnable = null, bool? isAdminCheck = null, bool? isAdminStop = null, bool? isActive = null,
            bool? isPublicPortal = null, bool? isPublicAdmin = null, bool? isPublicPortalHidden = null, bool? isPublicAdminHidden = null);
        DocumentModel Register(DocumentModel model, IList<DocumentFileModel> files = null);
        DocumentModel Update(DocumentModel model, IList<DocumentFileModel> files = null);
        void DeleteDocument(string documentId);
    }
}
