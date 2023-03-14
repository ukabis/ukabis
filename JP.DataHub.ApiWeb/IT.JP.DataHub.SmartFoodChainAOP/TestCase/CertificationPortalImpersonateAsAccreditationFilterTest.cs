using System.Net;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    /// <summary>
    /// 以下2つのFilterのテスト
    /// CertificationPortalImpersonateAsAccreditationFilter
    /// CertificationPortalImpersonateAsApplicantFilter
    /// </summary>
    [TestClass]
    public class CertificationPortalImpersonateAsAccreditationFilterTest : ApiWebItTestCase
    {
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
        public void CertificationPortalImpersonateAsAccreditationFilter_NormalScenario()
        {
            // 申請者
            var clientApplicant = new IntegratedTestClient(AppConfig.Account);
            // 認定機関
            var clientAuthority = new IntegratedTestClient("test2");
            // 関係ないユーザー
            var clientTest3 = new IntegratedTestClient("test3");

            var certificationApplyApi = UnityCore.Resolve<ICertificationApplyApi>();
            var certificationIssueHistoryApi = UnityCore.Resolve<ICertificationIssueHistoryApi>();
            var temporaryCertificationApi = UnityCore.Resolve<ITemporaryCertificationApi>();
            var certificationQuestionnaireApi = UnityCore.Resolve<ICertificationQuestionnaireApi>();
            var certificationNameApi = UnityCore.Resolve<ICertificationNameApi>();
            var parentCertificationNameApi = UnityCore.Resolve<IParentCertificationNameApi>();
            var certificationAuthorityApi = UnityCore.Resolve<ICertificationAuthorityApi>();
            var accreditationApi = UnityCore.Resolve<IAccreditationApi>();
            var accreditationCertificationMapApi = UnityCore.Resolve<IAccreditationCertificationMapApi>();
            
            const string testCertificationAuthorityId = "728AFAE8-C342-48FD-A1D9-6262B04EFB9C";
            const string testParentCertificationNameId = "56FDCE43-CD50-410A-B65A-D0AC81FB3147";
            const string testCertificationNameId = "135C770F-78F4-4386-BF10-BFCD3CC8878F";
            const string testCertificationQuestionnaireId = "64B0C549-E333-4E0E-B4B1-ABD217C0A3D0";
            const string testCertificationApplyId = "D7B590BC-CF14-4DC6-8A1E-F78822B73492";
            const string testTemporaryCertificationId = "E6D6F787-94B0-4C31-BB2B-5BC76B08DB8B";
            const string testCertificationIssueHistoryId = "D570698B-6BF1-4EA1-B416-674AF69EB236";
            const string testAccreditationId = "108099DC-002B-4426-AB75-2B12E46E4202";
            const string testAccreditationCertificationMapId = "03CFDE74-989C-4C5F-92CE-FAAC9A679861";
            var openId = clientApplicant.GetOpenId();

            // テスト用のマスタ系のデータを登録
            var registerCertificationAuthorityData = new CertificationAuthorityModel
            { 
                CertificationAuthorityId = testCertificationAuthorityId,
                CertificationAuthorityName = "テスト用",
            };
            clientApplicant.GetWebApiResponseResult(certificationAuthorityApi.Register(registerCertificationAuthorityData)).Assert(RegisterSuccessExpectStatusCode);
            
            var registerParentCertificationNameData = new ParentCertificationNameModel
            { 
                ParentCertificationNameId = testParentCertificationNameId,
                ParentCertificationName = "テスト用",
                IsPublic = true,
                CertificationAuthorityId = testCertificationAuthorityId,
                Url = "",
                CountryCode = "",
                OrderNo = 999,
                IsEnable = true,
            };
            clientApplicant.GetWebApiResponseResult(parentCertificationNameApi.Register(registerParentCertificationNameData)).Assert(RegisterSuccessExpectStatusCode);

            var registerCertificationNameData = new CertificationNameModel
            { 
                CertificationNameId = testCertificationNameId,
                CertificationName = "テスト用",
                ParentCertificationNameId = testParentCertificationNameId,
                IsPublic = true,
                CertificationAuthorityId = testCertificationAuthorityId,
                Url = "",
                CountryCode = "",
                OrderNo = 999,
                IsEnable = true,
            };
            clientApplicant.GetWebApiResponseResult(certificationNameApi.Register(registerCertificationNameData)).Assert(RegisterSuccessExpectStatusCode);

            var registerCertificationQuestionnaireData = new CertificationQuestionnaireModel
            { 
                CertificationQuestionnaireId = testCertificationQuestionnaireId,
                CertificationNameId = testCertificationNameId,
                AccreditationId = testAccreditationId,
                Version = new CertificationQuestionnaire_VersionModel()
                {
                    VersionNo = 1,
                    PublicEndDate = "2050-01-01"
                },
                IsActive = true,
                AutomaticJudgment = new CertificationQuestionnaire_AutomaticJudgmentModel()
                {
                    IsAutomaticJudgment = true,
                    PassScore = 80
                },
                Indivisual = null,
                IsAutoPublic = true,
                AddtionalCompanyItem = null,
                AddtionalOfficeItem = null,
                Expiration = new CertificationQuestionnaire_ExpirationModel()
                {
                    Day = 1,
                    Month = 1,
                    Week = 1,
                    Year = 10
                },
                IsTemporaryCertificate = true,
                ApplicantUnitCode = new List<string>{"cmp", "ofc"},
                PageStructure = null,
                TemporaryCertificationDataId = null
            };
            clientAuthority.GetWebApiResponseResult(certificationQuestionnaireApi.Register(registerCertificationQuestionnaireData)).Assert(RegisterSuccessExpectStatusCode);
            
            var accreditationData = new AccreditationModel
            { 
                AccreditationId = testAccreditationId,
                AccreditationName = "テスト用",
                CompanyName = "テスト用",
                CompanyNameKana = "テスト用",
                Url = "",
                DepartmentName = "テスト用",
                DepartmentNameKana = "テスト用",
                ManagerName = "テスト用",
                ManagerNameKana = "テスト用",
                ManagerMailAddress = "テスト用",
                ZipCode = "1234567",
                Address = "テスト用",
                Tel = "0312341234",
                Fax = "0312341234",
                GroupId = "",
            };
            clientAuthority.GetWebApiResponseResult(accreditationApi.Register(accreditationData)).Assert(RegisterSuccessExpectStatusCode);

            var accreditationCertificationMapData = new AccreditationCertificationMapModel
            { 
                AccreditationCertificationMapId = testAccreditationCertificationMapId,
                CertificationNameId = testCertificationNameId,
                AccreditationId = testAccreditationId,
            };
            clientAuthority.GetWebApiResponseResult(accreditationCertificationMapApi.Register(accreditationCertificationMapData)).Assert(RegisterSuccessExpectStatusCode);

            // 申請する
            var registerCertificationApplyData = new CertificationApplyModel
            { 
                CertificationApplyId = testCertificationApplyId,
                CertificationQuestionnaireId = testCertificationQuestionnaireId,
                SubmitOpenId = openId,
                EntryDate = new DateTime(2022, 10, 01),
                Apply = new CertificationApply_ApplyModel()
                {
                    CertificationApplyTypeCode = "cmp",
                },
                IsAutoPublic = true,
                CertificationApplyStatusCode = "apl",
                IsAutomaticJudgment = true,
                TotalScore = 100,
                ThresholdScore = 100,
                IsPass = true,
                DisqualificationReason = null,
                JudgementDate = new DateTime(2022, 10, 01),
                IsTemporaryCertificationIssue = false,
                IsCertificationIssue = false
            };
            clientApplicant.GetWebApiResponseResult(certificationApplyApi.Register(registerCertificationApplyData)).Assert(RegisterSuccessExpectStatusCode);

            // 申請者が認証側にも登録
            clientApplicant.GetWebApiResponseResult(certificationApplyApi.RegisterAsAccreditation(registerCertificationApplyData, testCertificationApplyId)).Assert(RegisterSuccessExpectStatusCode);

            // 認定機関が取得できるか確認
            clientAuthority.GetWebApiResponseResult(certificationApplyApi.Get(testCertificationApplyId)).Assert(GetSuccessExpectStatusCode, registerCertificationApplyData);
          
            // 申請者が更新
            registerCertificationApplyData.CertificationApplyStatusCode = "jrq";
            clientApplicant.GetWebApiResponseResult(certificationApplyApi.Update(testCertificationApplyId, registerCertificationApplyData)).Assert(UpdateSuccessExpectStatusCode);
            clientApplicant.GetWebApiResponseResult(certificationApplyApi.UpdateAsAccreditation(testCertificationApplyId, registerCertificationApplyData)).Assert(UpdateSuccessExpectStatusCode);

            // 認定機関が更新確認
            var result = clientAuthority.GetWebApiResponseResult(certificationApplyApi.Get(testCertificationApplyId)).Assert(GetSuccessExpectStatusCode);
            result.Result.CertificationApplyStatusCode.Is("jrq");

            // 認定機関が更新
            registerCertificationApplyData.CertificationApplyStatusCode = "pas";
            clientAuthority.GetWebApiResponseResult(certificationApplyApi.Update(testCertificationApplyId, registerCertificationApplyData)).Assert(UpdateSuccessExpectStatusCode);
            clientAuthority.GetWebApiResponseResult(certificationApplyApi.UpdateAsApplicant(testCertificationApplyId, registerCertificationApplyData)).Assert(UpdateSuccessExpectStatusCode);

            // 申請者が更新確認
            result = clientApplicant.GetWebApiResponseResult(certificationApplyApi.Get(testCertificationApplyId)).Assert(GetSuccessExpectStatusCode);
            result.Result.CertificationApplyStatusCode.Is("pas");
            
            // 申請者が仮証明書を登録
            var registerTemporaryCertificationData = new TemporaryCertificationModel
            { 
                TemporaryCertificationId = testTemporaryCertificationId,
                CertificationQuestionnaireId = testCertificationQuestionnaireId,
                FilePath = "",
            };
            clientApplicant.GetWebApiResponseResult(temporaryCertificationApi.RegisterAsAccreditation(registerTemporaryCertificationData, testCertificationApplyId)).Assert(RegisterSuccessExpectStatusCode);

            // 認定機関が取得
            clientAuthority.GetWebApiResponseResult(temporaryCertificationApi.Get(testTemporaryCertificationId)).Assert(GetSuccessExpectStatusCode, registerTemporaryCertificationData);

            // 申請者が履歴を登録
            var registerCertificationIssueHistoryData = new CertificationIssueHistoryModel
            { 
                CertificationIssueHistoryId = testCertificationIssueHistoryId,
                CertificationApplyId = testCertificationApplyId,
                IssueDate = new DateTime(2022, 10, 01),
                IsTemporary = false,
                TemporaryCertificationId = testTemporaryCertificationId,
            };
            clientApplicant.GetWebApiResponseResult(certificationIssueHistoryApi.RegisterAsAccreditation(registerCertificationIssueHistoryData, testCertificationApplyId)).Assert(RegisterSuccessExpectStatusCode);

            // 認定機関が取得
            clientAuthority.GetWebApiResponseResult(certificationIssueHistoryApi.Get(testCertificationIssueHistoryId)).Assert(GetSuccessExpectStatusCode, registerCertificationIssueHistoryData);

            // 関係ないユーザーからは更新できないことを確認
            clientTest3.GetWebApiResponseResult(certificationApplyApi.RegisterAsAccreditation(registerCertificationApplyData, testCertificationApplyId)).AssertErrorCode(HttpStatusCode.NotFound, "E107402");
            clientTest3.GetWebApiResponseResult(certificationApplyApi.UpdateAsAccreditation(testCertificationApplyId, registerCertificationApplyData)).AssertErrorCode(HttpStatusCode.NotFound, "E107402");
            clientTest3.GetWebApiResponseResult(certificationApplyApi.UpdateAsApplicant(testCertificationApplyId, registerCertificationApplyData)).AssertErrorCode(HttpStatusCode.NotFound, "E107402");
            clientTest3.GetWebApiResponseResult(temporaryCertificationApi.RegisterAsAccreditation(registerTemporaryCertificationData, testCertificationApplyId)).AssertErrorCode(HttpStatusCode.NotFound, "E107402");
            clientTest3.GetWebApiResponseResult(certificationIssueHistoryApi.RegisterAsAccreditation(registerCertificationIssueHistoryData, testCertificationApplyId)).AssertErrorCode(HttpStatusCode.NotFound, "E107402");
        }
    }
}
