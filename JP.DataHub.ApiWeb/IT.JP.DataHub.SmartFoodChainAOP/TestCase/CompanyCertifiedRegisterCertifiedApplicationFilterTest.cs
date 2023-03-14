using System.Net;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class CompanyCertifiedRegisterCertifiedApplicationFilterTest : ItTestCaseBase
    {        
        private const string CompanyId = "6a041a07-9d77-4c2a-9495-fc7fe1d38dd1";
        private const string CompanyCertifiedId = "c05eb3bd-2857-41de-9106-265ddc58849d";


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        public void CompanyCertifiedRegisterCertifiedApplicationFilterTest_NormalScenario()
        {
            var client1 = new IntegratedTestClient("test1");
            var client2 = new IntegratedTestClient("test2");
            var company = UnityCore.Resolve<ICompanyApi>();
            var companyCertified = UnityCore.Resolve<ICompanyCertifiedApi>();

            // データ削除
            DeleteTestData();

            // 事業者登録
            client1.GetWebApiResponseResult(company.Register(new CompanyModel()
            {
                CompanyId = CompanyId,
                CompanyName = "テスト用事業者1",
                CompanyNameLang = new List<NameLangModel>
                    {
                        new NameLangModel {LocaleCode = "ja-jp", Name = "テスト用事業者1"},
                        new NameLangModel {LocaleCode = "en-us", Name = "Test1"}
                    },
                IndustoryTypeCode = "wholesaler",
                Address1 = "hoge",
                Address2 = "hoge",
                Address3 = "hoge",
                Fax = "1234567890",
                Images = new List<ImageModel>
                    {
                        new ImageModel
                        {
                            DefaultImageFlag = true, ImageDescription = "hoge",
                            ImagePath = "hoge"
                        }
                    },
                MailAddress = "hoge@example.com",
                ZipCode = "1234567",
                Tel = "1234567890",
                CountryCode = "JP",
                GlnCode = CompanyId,
                GS1CompanyCode = "hoge"
            })).Assert(RegisterSuccessExpectStatusCode);

            // 認証情報登録
            // BadRequest(存在しないCompanyId)
            client2.GetWebApiResponseResult(companyCertified.RegisterCertifiedApplication(new CompanyCertifiedModel()
            {
                CompanyCertifiedId = CompanyCertifiedId,
                CompanyId = Guid.NewGuid().ToString(),
                DataSourceType = "SeedsManagement",
                DataSourceCertificationId = Guid.NewGuid().ToString(),
                CertificationNo = Guid.NewGuid().ToString(),
                CertificationName = Guid.NewGuid().ToString(),
                ExpireDate = "2023-02-14",
                RegisteredDate = "2023-02-14",
                EvidenceFiles = new List<EvidenceFileModel>()
                {
                    new EvidenceFileModel()
                    {
                        FileTypeCode = "etc",
                        FilePath = Guid.NewGuid().ToString()
                    }
                }
            })).AssertErrorCode(BadRequestStatusCode, "E107403");

            // BadRequest(通常のバリデーションエラー)
            client2.GetWebApiResponseResult(companyCertified.RegisterCertifiedApplication(new CompanyCertifiedModel()
            {
                CompanyCertifiedId = CompanyCertifiedId,
                CompanyId = CompanyId,
                DataSourceType = "hogehoge",
                DataSourceCertificationId = Guid.NewGuid().ToString(),
                CertificationNo = Guid.NewGuid().ToString(),
                CertificationName = Guid.NewGuid().ToString(),
                ExpireDate = "2023-02-14",
                RegisteredDate = "2023-02-14",
                EvidenceFiles = new List<EvidenceFileModel>()
                {
                    new EvidenceFileModel()
                    {
                        FileTypeCode = "etc",
                        FilePath = Guid.NewGuid().ToString()
                    }
                }
            })).AssertErrorCode(BadRequestStatusCode, "E10402");

            // 登録成功
            client2.GetWebApiResponseResult(companyCertified.RegisterCertifiedApplication(new CompanyCertifiedModel()
            {
                CompanyCertifiedId = CompanyCertifiedId,
                CompanyId = CompanyId,
                DataSourceType = "SeedsManagement",
                DataSourceCertificationId = Guid.NewGuid().ToString(),
                CertificationNo = Guid.NewGuid().ToString(),
                CertificationName = Guid.NewGuid().ToString(),
                ExpireDate = "2023-02-14",
                RegisteredDate = "2023-02-14",
                EvidenceFiles = new List<EvidenceFileModel>()
                {
                    new EvidenceFileModel()
                    {
                        FileTypeCode = "etc",
                        FilePath = Guid.NewGuid().ToString()
                    }
                }
            })).Assert(RegisterSuccessExpectStatusCode);

            // 登録確認(Companyと同じ領域に登録される)
            client1.GetWebApiResponseResult(companyCertified.Get(CompanyCertifiedId)).Assert(GetSuccessExpectStatusCode);
            client2.GetWebApiResponseResult(companyCertified.Get(CompanyCertifiedId)).Assert(NotFoundStatusCode);

            // データ削除
            DeleteTestData();
        }



        private void DeleteTestData()
        {
            var client = new IntegratedTestClient("test1");
            var company = UnityCore.Resolve<ICompanyApi>();
            var companyCertified = UnityCore.Resolve<ICompanyCertifiedApi>();

            client.GetWebApiResponseResult(company.Delete(CompanyId)).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(companyCertified.Delete(CompanyCertifiedId)).Assert(DeleteExpectStatusCodes);
        }
    }
}
