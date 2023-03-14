using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Resolution;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models.Document;

namespace IT.JP.DataHub.ManageApi.TestCaseTutorial
{
    [Ignore]
    [TestClass]
    public class _0_Sample : ItTestCaseBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod("_0_Sample.DynamicApiClientのインスタンス作成およびアカウントの指定")]
        public void Func1()
        {
            // test2はアカウント名。test2のアカウント情報を内部で取得している（test2のアカウント情報とか認証情報を持っている）      ※アカウント情報はaccount.jsonに定義あり
            var client = new DynamicApiClient("test2");
        }

        [TestMethod("_0_Sample.DynamicApiClientのインスタンス作成およびアカウントの指定")]
        public void Func2()
        {
            // DynamicApiClientのインスタンスを作成(DI)
            // アカウント情報を別途取得（↑のサンプルと同様）      ※アカウント情報はaccount.jsonに定義あり
            // 取得したアカウント情報をclientに設定
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var auth = UnityCore.Resolve<IAuthenticationInfo>("test1");
            client.SwitchAuthentication(auth);
        }

        [TestMethod("_0_Sample.アカウント情報２つそれぞれでAPIを呼び出し")]            // 名前が指定できるよ
        public void Func3()
        {
            // test1、test2とtest3のアカウント情報を持つDynamicApiClientのインスタンスを作成
            var client1 = new DynamicApiClient("test1");
            var client2 = new DynamicApiClient("test2");
            var client3 = new DynamicApiClient("test3");

            var additionalHeaders = new Dictionary<string, string[]>();
            additionalHeaders.Add("X-Hoge", "0");
            additionalHeaders.Add("X-Peke", new string[] { "1" });

            // ManageVendorリソースのインスタンスを作成（VendorApiのクラスは実装する必要はない。interfaceのみを定義する）
            var vendor = UnityCore.Resolve<IDocumentApi>();
            vendor.AddHeaders.Add("X-Test", "ABC");

            // リソースのURLを変更する仕組み
            // 通常は /Manage/Document だけど一定条件で /Manage/Mongo/Document に変更したい場合は次のようにする(UT的な処置あり）
            var req = vendor.GetCategoryList();
            var org = vendor.ResourceUrl;
            req.ResourceUrl.Is("/Manage/Document");
            vendor.ResourceUrl = "/Manage/Mongo/Document";
            req = vendor.GetCategoryList();
            req.ResourceUrl.Is("/Manage/Mongo/Document");

            // 追加ヘッダーの指定方法
            // ①IVendorApiに依存して追加ヘッダーを指定する（IVendorApiのActionメソッドを呼び出す場合はいつも使われる）
            // vendor.AddHeaders = additionalHeaders;          // 追加ヘッダーを完全に入れ替える
            // ②APIを呼び出すときだけに指定する方法
            // client1.Request(vendor.GetCategoryList(), additionalHeaders)
            // ③IWebApiClientAddHeaderをRegisterTypeすると、そのインプリメントクラスで定義しているヘッダーが無条件に入る
            // RegisterType<IWebApiClientAddHeader, AdditonalHeader>();
            // class AdditonalHeader : IWebApiClientAddHeader
            // {
            //     Dictionary<string, string[]> Header { get; set; } 
            //     public AdditonalHeader
            //     {
            //         Header.Add("ヘッダーキー", new string[] { "ヘッダー値" });
            //     }
            // }
            // AdditonalHeaderクラスのコンストラクタに入れる

            // VendorAPIのGetCategoryListを呼び出す
            // この時は追加ヘッダーは
            // 1. X-Test
            // 2. X-Hoge
            // 3. X-Peke
            // の３つである
            var result1 = client1.Request(vendor.GetCategoryList(), additionalHeaders)
                .ToWebApiResponseResult<List<DocumentCategoryModel>>();
            result1.StatusCode.Is(HttpStatusCode.OK);

            // VendorAPIのGetAgreementListを呼び出す
            // この時は追加ヘッダー
            // 1. X-Test
            // だけになる
            var result2 = client2.Request(vendor.GetAgreementList())
                .ToWebApiResponseResult<List<DocumentAgreementModel>>();
            result2.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod("_0_Sample.シンプルに全農作物コードの取得")]
        public void Func4()
        {
            var client1 = new DynamicApiClient("test1");
            var crop = UnityCore.Resolve<ICropApi>();
            var result = client1.Request(crop.GetList())
              .ToWebApiResponseResult<List<DocumentCategoryModel>>();
        }
    }
}