namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class SecondaryRepositoryViewModel
    {
        /// <summary>
        /// SecondaryRepositoryMapId
        /// </summary>
        public string SecondaryRepositoryMapId { get; set; }

        /// <summary>
        /// RepositoryGroupId
        /// </summary>
        public string RepositoryGroupId { get; set; }

        /// <summary>
        /// RepositoryGroupName
        /// </summary>
        public string RepositoryGroupName { get; set; }

        /// <summary>
        /// IsPrimary
        /// </summary>
        public bool IsPrimary { get; set; }
    }
}
