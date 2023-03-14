namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    /// <summary>
    /// AttachFileStorageとして利用できるリポジトリのViewModel
    /// </summary>
    public class DynamicApiAttachFileStorageViewModel
    {
        /// <summary>
        /// リポジトリグループID
        /// </summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>
        /// リポジトリグループ名
        /// </summary>
        public string RepositoryGroupName { get; set; }
    }
}
