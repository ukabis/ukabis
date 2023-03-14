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
    public interface IApiDescriptionService
    {
        IEnumerable<VendorLinkModel> GetVendorLink();
        IEnumerable<SystemLinkModel> GetSystemLink();
        IEnumerable<ApiDescriptionModel> GetApiDescription(bool noChildren, string culture = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false);
        IEnumerable<SchemaDescriptionModel> GetSchemaDescription(string culture = null);
        IEnumerable<CategoryModel> GetCategoryList(string culture = null);
        RegisterStaticApiResponseModel RegisterStaticApi(StaticApiModel requestModel);
        IEnumerable<RegisterStaticApiResponseModel> RegisterStaticApi(IEnumerable<StaticApiModel> staticApiList);
    }
}
