using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiResourceHeaderModel
    {

        /// <summary>
        /// ベンダー名システム名リスト
        /// </summary>
        public IEnumerable<VendorNameSystemNameModel> VendorNameSystemNameList { get; set; }

        /// <summary>
        /// データスキーマリスト
        /// </summary>
        public IEnumerable<SchemaModel> DataSchemaList { get; set; }

        /// <summary>
        /// コントローラカテゴリーリスト
        /// </summary>
        public List<ControllerCategoryInfomationModel> ControllerCategoryInfomationList { get; set; }

        /// <summary>
        /// コントローラコモンIPフィルタグループリスト
        /// </summary>
        public List<ApiResourceCommonIpFilterGroupModel> ControllerCommonIpFilterGroupList { get; set; }

        /// <summary>
        /// コントローラ
        /// </summary>
        public ApiResourceInformationModel Controller { get; set; }

        /// <summary>
        /// タグリスト
        /// </summary>
        public IEnumerable<TagInfoModel> TagInfoList { get; set; }

        /// <summary>
        /// コントローラタグリスト
        /// </summary>
        public List<ApiResourceTagInfoModel> ControllerTagInfoList { get; set; }

        /// <summary>
        /// フィールドリスト
        /// </summary>
        public IEnumerable<ApiResourceFieldQueryModel> FieldInfoList { get; set; }

        /// <summary>
        /// コントローラフィールドリスト
        /// </summary>
        public List<ApiResourceFieldInfoModel> ControllerFieldInfoList { get; set; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public IEnumerable<OpenIdCaViewModel> OpenIdCaList { get; set; }

        /// <summary>
        /// リポジトリグループリスト
        /// </summary>
        public IEnumerable<RepositoryGroupModel> RepositoryGroupList { get; set; }

        /// <summary>
        /// 添付ファイル設定
        /// </summary>
        public ApiResourceAttachFileSettingsModel AttachFileSettings { get; set; }

        /// <summary>
        /// 履歴ドキュメント設定
        /// </summary>
        public DocumentHistorySettingsModel DocumentHistorySettings { get; set; }

        /// <summary>
        /// 新規登録か(画面制御用)
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 保存された(画面制御用)
        /// </summary>
        public bool IsCreatedOrRenamed { get; set; } = false;

        /// <summary>
        /// AdaptResourceSchemaを呼び出し可能か(画面制御用)。
        /// AdaptResourceSchemaはPOSTメソッドが1つ以上存在する場合のみ呼び出し可能。
        /// </summary>
        public bool EnableAdaptResourceSchema { get; set; } = false;
    }
}
