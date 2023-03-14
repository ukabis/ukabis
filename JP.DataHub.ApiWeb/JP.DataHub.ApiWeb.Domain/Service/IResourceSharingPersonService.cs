using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ApiWeb.Domain.Service.Model;

namespace JP.DataHub.ApiWeb.Domain.Service
{
    [TransactionScope]
    public interface IResourceSharingPersonService
    {
        /// <summary>
        /// 指定されたIDの個人リソースシェアリングルールを削除します。
        /// </summary>
        /// <param name="id">個人リソースシェアリングルールID</param>
        /// <returns>個人リソースシェアリングルールID</returns>
        void Delete(string id);

        /// <summary>
        /// 指定されたIDの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="id">個人リソースシェアリングルールID</param>
        /// <returns>個人リソースシェアリングルール</returns>
        ResourceSharingPersonRuleModel Get(string id);

        /// <summary>
        /// 指定されたリソースパスの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">リソースパス</param>
        /// <returns>個人リソースシェアリングルール</returns>
        IEnumerable<ResourceSharingPersonRuleModel> GetList(string path);

        /// <summary>
        /// 指定されたリソースパスの指定されたユーザIDからの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">リソースパス</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>個人リソースシェアリングルール</returns>
        IEnumerable<ResourceSharingPersonRuleModel> GetFromList(string path, string userId);

        /// <summary>
        /// 指定されたリソースパスの指定されたユーザIDへの個人リソースシェアリングルールを取得します。
        /// </summary>
        /// <param name="path">リソースパス</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>個人リソースシェアリングルール</returns>
        IEnumerable<ResourceSharingPersonRuleModel> GetToList(string path, string userId);

        /// <summary>
        /// 個人リソースシェアリングルールを登録します。
        /// </summary>
        /// <param name="model">個人リソースシェアリングルール</param>
        /// <returns>個人リソースシェアリングルール</returns>
        ResourceSharingPersonRuleModel Register(ResourceSharingPersonRuleModel model);

        /// <summary>
        /// 個人リソースシェアリングルールを更新します。
        /// </summary>
        /// <param name="model">個人リソースシェアリングルール</param>
        /// <returns>個人リソースシェアリングルール</returns>
        ResourceSharingPersonRuleModel Update(ResourceSharingPersonRuleModel model);
    }
}
