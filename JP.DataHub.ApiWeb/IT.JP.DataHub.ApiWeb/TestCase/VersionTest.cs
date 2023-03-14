using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class VersionTest : ApiWebItTestCase
    {
        #region TestData

        private class VersionTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                ConversionSquareMeters = 10
            };

            public VersionTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl, true) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [DataTestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void VersionTest_NormalScenario(Repository repository)
        {
            // 現在のカレントバージョンとバージョン情報の詳細を取得
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IVersionApi>();

            var current = client.GetWebApiResponseResult(api.GetCurrentVersion()).Assert(GetSuccessExpectStatusCode).Result;
            var version = client.GetWebApiResponseResult(api.GetVersionInfo()).Assert(GetSuccessExpectStatusCode).Result;

            // currentがバージョン情報の詳細と合っているか？
            version.currentversion.Is(current.CurrentVersion);
            version.documentversions.Where(x => x.is_current == true).Count().Is(1);
            version.documentversions.Where(x => x.is_current == true).Select(x => x.version).FirstOrDefault().Is(current.CurrentVersion);

            // RegisterVersionを取得するが、まだ始めていない（登録モード）ので、0を確認
            var currentregister = client.GetWebApiResponseResult(api.GetRegisterVersion()).Assert(GetSuccessExpectStatusCode).Result;
            currentregister.RegisterVersion.Is(0);

            // 登録バージョンを作成
            var register = client.GetWebApiResponseResult(api.CreateRegisterVersion()).Assert(RegisterSuccessExpectStatusCode).Result;
            register.RegisterVersion.Is(current.CurrentVersion + 1);

            // 登録バージョンを作ったけど、カレントバージョンには変化なし（登録用バージョンの履歴が作られていること）
            version = client.GetWebApiResponseResult(api.GetVersionInfo()).Assert(GetSuccessExpectStatusCode).Result;
            version.currentversion.Is(current.CurrentVersion);
            version.documentversions.Where(x => x.is_current == true).Count().Is(1);
            version.documentversions.Where(x => x.is_current == false).Select(x => x.version).Max().Is(current.CurrentVersion + 1);

            // 登録バージョンをFIX
            version = client.GetWebApiResponseResult(api.CompleteRegisterVersion()).Assert(GetSuccessExpectStatusCode).Result;
            version.currentversion.Is(0);
            // 登録バージョンをFIXした後にバージョン情報は正しいか？（最初から見ると１つ進んでいる）
            version = client.GetWebApiResponseResult(api.GetVersionInfo()).Assert(GetSuccessExpectStatusCode).Result;
            version.documentversions.Where(x => x.is_current == true).Count().Is(1);
            version.documentversions.Where(x => x.is_current == true).Select(x => x.version).Max().Is(current.CurrentVersion + 1);

            // いきなりNewVersionにする
            var newversion = client.GetWebApiResponseResult(api.SetNewVersion()).Assert(RegisterSuccessExpectStatusCode).Result;
            newversion.CurrentVersion.Is(current.CurrentVersion + 2);
            // その場合でも、バージョン情報は正しいか？（最初から見ると２つ進んでいる）
            version = client.GetWebApiResponseResult(api.GetVersionInfo()).Assert(GetSuccessExpectStatusCode).Result;
            version.documentversions.Where(x => x.is_current == true).Count().Is(1);
            version.documentversions.Where(x => x.is_current == true).Select(x => x.version).Max().Is(current.CurrentVersion + 2);
        }

        [DataTestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void VersionTest_Version_Data(Repository repository)
        {
            // 現在のバージョン番号取得
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IVersionApi>();
            var testData = new VersionTestData(repository, api.ResourceUrl);

            var first_version = client.GetWebApiResponseResult(api.GetCurrentVersion()).Assert(GetSuccessExpectStatusCode).Result;

            // レコード削除
            client.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteExpectStatusCodes);

            // レコードを１件登録し、レコード数を取得
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            var first_count = client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result;
            first_count.Count.Is(1);

            // 次のバージョン作成
            var nextversion = client.GetWebApiResponseResult(api.SetNewVersion()).Assert(RegisterSuccessExpectStatusCode).Result;
            nextversion.CurrentVersion.Is(first_version.CurrentVersion + 1);

            // 次のバージョンに、レコードを2件登録
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.Register(testData.Data2)).Assert(RegisterSuccessExpectStatusCode);

            var second_count = client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result;
            second_count.Count.Is(2);

            // 最初のバージョンを指定して件数を取得
            api.AddHeaders.Add(HeaderConst.X_Version, first_version.CurrentVersion.ToString());

            var count = client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result;
            count.Count.Is(first_count.Count);

            // カレントバージョンを指定して件数を取得
            api.AddHeaders.Remove(HeaderConst.X_Version);
            api.AddHeaders.Add(HeaderConst.XVendor, nextversion.CurrentVersion.ToString());
            count = client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result;
            count.Count.Is(second_count.Count);
            api.AddHeaders.Remove(HeaderConst.X_Version);

            // 登録バージョンを作成
            var register = client.GetWebApiResponseResult(api.CreateRegisterVersion()).Assert(RegisterSuccessExpectStatusCode).Result;
            register.RegisterVersion.Is(first_version.CurrentVersion + 2);

            // 登録バージョンに、レコードを1件登録
            api.AddHeaders.Add(HeaderConst.X_Version, register.RegisterVersion.ToString());
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 登録バージョンを指定して件数を取得
            count = client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result;
            count.Count.Is(1);
            api.AddHeaders.Remove(HeaderConst.X_Version);

            // カレントバージョンを指定して件数を取得
            api.AddHeaders.Add(HeaderConst.XVendor, nextversion.CurrentVersion.ToString());
            count = client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result;
            count.Count.Is(second_count.Count);
            api.AddHeaders.Remove(HeaderConst.X_Version);

            // 登録バージョンをFIX
            client.GetWebApiResponseResult(api.CompleteRegisterVersion()).Assert(GetSuccessExpectStatusCode);

            // バージョン指定なし(=カレントバージョン)を指定して件数を取得
            count = client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result;
            count.Count.Is(1);
        }
    }
}
