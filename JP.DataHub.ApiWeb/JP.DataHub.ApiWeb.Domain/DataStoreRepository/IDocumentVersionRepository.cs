using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    // .NET6
    internal interface IDocumentVersionRepository
    {
        INewDynamicApiDataStoreRepository PhysicalRepository { get; set; }
        RepositoryKeyInfo RepositoryKeyInfo { get; }

        DocumentHistories GetDocumentVersion(DocumentKey documentKey);
        DocumentHistories SaveDocumentVersion<T>(T documentKey, RepositoryKeyInfo repositoryKeyInfo, bool isDelete = false) where T : DocumentKey;
        DocumentHistories SaveDocumentVersion<T>(T documentKey, RepositoryKeyInfo repositoryKeyInfoMain, DocumentHistory newlatest, bool isDeleteNewHistoy) where T : DocumentKey;
        bool HistoryThrowAway(DocumentKey documentKey);

        DocumentHistories UpdateDocumentVersion(DocumentKey documentKey, DocumentHistory newlatest);
    }
}
