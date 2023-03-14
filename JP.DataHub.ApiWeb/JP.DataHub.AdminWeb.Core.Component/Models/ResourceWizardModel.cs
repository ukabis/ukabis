using JP.DataHub.AdminWeb.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Core.Component.Models.Api
{
    public class ResourceWizardModel
    {

        public ResourceWizardModel()
        {
            RepositoryKeySource = new List<string>();
            MethodList = new()
            {
                new MethodRegisterSelectModel(){MethodName ="Get/{キー}",Description="単一のデータを取得する", Create=true},
                new MethodRegisterSelectModel(){MethodName ="GetAll",Description="全てのデータを取得する",Create=true},
                new MethodRegisterSelectModel(){MethodName ="Register",Description="単一のデータを登録する",Create=true},
                new MethodRegisterSelectModel(){MethodName ="RegisterList",Description="複数のデータを登録する",Create=true},
                new MethodRegisterSelectModel(){MethodName ="Update/{キー}",Description="単一のデータを更新する",Create=true},
                new MethodRegisterSelectModel(){MethodName ="Exists/{キー}",Description="データの存在を確認する",Create=true},
                new MethodRegisterSelectModel(){MethodName ="Delete/{キー}",Description="単一のデータを削除する",Create=true},
                new MethodRegisterSelectModel(){MethodName ="DeleteAll",Description="全てのデータを削除する",Create=true}
            };

            DefaultMethodList = new()
            {
                new MethodRegisterSelectModel(){MethodName ="SetNewVersion",Description="新しいバージョンを作成し、現在のバージョンを設定する"},
                new MethodRegisterSelectModel(){MethodName ="GetCount",Description="レコード数を取得する"},
                new MethodRegisterSelectModel(){MethodName ="GetCurrentVersion",Description="現在のバージョンを取得する"},
                new MethodRegisterSelectModel(){MethodName ="CreateRegisterVersion",Description="登録用バージョンを作成する"},
                new MethodRegisterSelectModel(){MethodName ="CompleteRegisterVersion",Description="登録用バージョンを現在のバージョンに設定する"},
                new MethodRegisterSelectModel(){MethodName ="GetRegisterVersion",Description="登録用バージョンを取得する"},
                new MethodRegisterSelectModel(){MethodName ="GetVersionInfo",Description="バージョン情報を表示する"},
                new MethodRegisterSelectModel(){MethodName ="OData",Description="ODataでリクエストするためのAPI"},
                new MethodRegisterSelectModel(){MethodName ="ODataDelete",Description="ODataで削除するためのAPI"},
                new MethodRegisterSelectModel(){MethodName ="ODataPatch",Description="ODataで更新するためのAPI"},
                new MethodRegisterSelectModel(){MethodName ="AdaptResourceSchema",Description="APIにモデルを適用する"},
                new MethodRegisterSelectModel(){MethodName ="GetResourceSchema",Description="レスポンスモデルを取得する"}
            };
            Url = @"/API/Individual/";
            DataModel = @"{
  ""description"": """",
  ""type"": ""object"",
  ""properties"": {
     項目を指定してください。
     例：「
     ""SampleCode"": {
       ""title"": ""サンプルコード"",
       ""type"": ""string""}
     」
  },
  ""required"": [
  必須項目がある場合は指定してください。
  ]
}";
        }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// API名
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Api説明
        /// </summary>
        public string ApiDescription { get; set; }

        /// <summary>
        /// モデル名
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// データモデル
        /// </summary>
        public string DataModel { get; set; }

        /// <summary>
        /// リポジトリキーリスト
        /// </summary>
        public List<string> RepositoryKeySource { get; set; }

        /// <summary>
        /// リポジトリキー
        /// </summary>
        public string RepositoryKey { get; set; }

        /// <summary>
        /// リポジトリグループ
        /// </summary>
        public IList<RepositoryGroupModel> RepositoryGroup { get; set; } = new List<RepositoryGroupModel>();

        /// <summary>
        /// リポジトリグループID
        /// </summary>
        public string RepositoryGroupId => RepositoryGroup.FirstOrDefault()?.RepositoryGroupId;

        /// <summary>
        /// リポジトリグループ名
        /// </summary>
        public string RepositoryGroupName => RepositoryGroup.FirstOrDefault()?.RepositoryGroupName;

        /// <summary>
        /// メソッドリスト
        /// </summary>
        public List<MethodRegisterSelectModel> MethodList { get; private set; }

        /// <summary>
        /// デフォルトメソッドリスト
        /// </summary>
        public List<MethodRegisterSelectModel> DefaultMethodList { get; private set; }

        public class MethodRegisterSelectModel
        {
            public string MethodName { get; set; }
            public string Description { get; set; }
            public bool Create { get; set; }
        }
    }
}
