namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    /// <summary>
    /// カテゴリーのViewModel
    /// </summary>
    public class DynamicApiCategoryViewModel
    {
        public string ControllerCategoryId { get; set; }

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public bool IsActive { get; set; }
    }
}
