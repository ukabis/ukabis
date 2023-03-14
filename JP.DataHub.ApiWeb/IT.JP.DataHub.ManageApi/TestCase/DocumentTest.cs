using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Resolution;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi.Models.Document;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    //後で見直し
    [TestClass]
    public class DocumentTest : ManageApiTestCase
    {
        private const string registerTitle = "--itRegister--";
        private const string updateTitle = "--itUpdate--";
        private const string detail = "---itDetail---";
        private static IList<string> listTestTitleName = new List<string>() { registerTitle, updateTitle };

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterDocumentModel, UpdateDocumentModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });

        private static IMapper s_mapper => s_lazyMapper.Value;



        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void Document_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDocumentApi>();

            #region テストデータ削除
            var listDocument = client.Request(api.GetDocumentList(AppConfig.AdminVendorId))
                .ToWebApiResponseResult<List<DocumentWithoutFileModel>>();
            listDocument.StatusCode.IsContains(GetExpectStatusCodes);
            var listDelete = listDocument.Result.Where(x => listTestTitleName.Contains(x.Title) == true).Select(x => x.DocumentId).ToList();
            listDelete.ForEach(x => client.Request(api.DeleteDocument(x)).StatusCode.IsContains(DeleteExpectStatusCodes));
            #endregion

            // ポータル用
            // portal, admin,  all, [null] 指定で取得が可能であること。
            var getList = client.Request(api.GetList("portal")).ToWebApiResponseResult<List<DocumentModel>>();
            getList.StatusCode.IsContains(GetExpectStatusCodes);
            var getList1 = client.Request(api.GetList("admin")).ToWebApiResponseResult<List<DocumentModel>>();
            getList1.StatusCode.IsContains(GetExpectStatusCodes);
            var getList2 = client.Request(api.GetList("all")).ToWebApiResponseResult<List<DocumentModel>>();
            getList2.StatusCode.IsContains(GetExpectStatusCodes);
            var getList3 = client.Request(api.GetList(null)).ToWebApiResponseResult<List<DocumentModel>>();
            getList3.StatusCode.IsContains(GetExpectStatusCodes);
            var getAgreementList = client.Request(api.GetAgreementList()).ToWebApiResponseResult<List<DocumentAgreementModel>>();
            getAgreementList.StatusCode.IsContains(GetExpectStatusCodes);
            var getCategoryList = client.Request(api.GetCategoryList()).ToWebApiResponseResult<List<DocumentCategoryModel>>();
            getCategoryList.StatusCode.IsContains(GetExpectStatusCodes);
            if (getAgreementList.Result.Count() == 0 || getCategoryList.Result.Count() == 0)
            {
                // 存在しない環境の場合はテストしない
                return;
            }

            // 【準備】登録・更新に使用するため、規約・カテゴリの一覧から１番目の要素を抜き出す。
            var tmpAgreement = getAgreementList.Result.FirstOrDefault(x => x.IsActive == true);
            var firstAgreementId = tmpAgreement?.AgreementId;
            var firstCategoryId = getCategoryList.Result.First().CategoryId;

            // Manage系
            // 新規登録
            // FKデータは改めてセット
            var regData = s_mapper.Map<RegisterDocumentModel>(registerData);
            regData.CategoryId = firstCategoryId;
            regData.AgreementId = firstAgreementId;
            regData.VendorId = AppConfig.AdminVendorId;
            regData.SystemId = AppConfig.AdminSystemId;
            var resultRegisterDocument = client.Request(api.RegisterDocument(registerData)).ToWebApiResponseResult<DocumentWithoutFileModel>();
            resultRegisterDocument.StatusCode.Is(RegisterSuccessExpectStatusCode);
            var registerId = resultRegisterDocument.Result.DocumentId;

            #region RegisterDocument:FkError系
            //VendorId FkError
            {
                var fkData = registerData;
                fkData.VendorId = Guid.NewGuid().ToString();
                fkData.SystemId = AppConfig.AdminSystemId;
                fkData.AgreementId = firstAgreementId;
                fkData.CategoryId = firstCategoryId;
                var result = client.Request(api.RegisterDocument(fkData)).ToWebApiResponseResult<DocumentWithoutFileModel>();
                result.StatusCode.Is(HttpStatusCode.BadRequest);
            }
            //SystemId FkError
            {
                var fkData = registerData;
                fkData.VendorId = AppConfig.AdminVendorId;
                fkData.SystemId = Guid.NewGuid().ToString();
                fkData.AgreementId = firstAgreementId;
                fkData.CategoryId = firstCategoryId;
                var result = client.Request(api.RegisterDocument(fkData)).ToWebApiResponseResult<DocumentWithoutFileModel>();
                result.StatusCode.Is(HttpStatusCode.BadRequest);
            }
            //AgreementId FkError
            {
                var fkData = registerData;
                fkData.VendorId = AppConfig.AdminVendorId;
                fkData.SystemId = AppConfig.AdminSystemId;
                fkData.AgreementId = Guid.NewGuid().ToString();
                fkData.CategoryId = firstCategoryId;
                var result = client.Request(api.RegisterDocument(fkData)).ToWebApiResponseResult<DocumentWithoutFileModel>();
                result.StatusCode.Is(HttpStatusCode.BadRequest);
            }
            //CategoryId FkError
            {
                var fkData = registerData;
                fkData.VendorId = AppConfig.AdminVendorId;
                fkData.SystemId = AppConfig.AdminSystemId;
                fkData.AgreementId = firstAgreementId;
                fkData.CategoryId = Guid.NewGuid().ToString();
                var result = client.Request(api.RegisterDocument(fkData)).ToWebApiResponseResult<DocumentWithoutFileModel>();
                result.StatusCode.Is(HttpStatusCode.BadRequest);
            }

            #endregion

            // 新規登録したものを取得
            var getRegDataResult = client.Request(api.GetDocument(registerId)).ToWebApiResponseResult<DocumentModel>();
            var getRegData = getRegDataResult.Result;

            // 更新登録
            var updData = updateData;
            updData.DocumentId = registerId;
            //FKデータは改めてセット
            updData.VendorId = AppConfig.AdminVendorId;
            updData.SystemId = AppConfig.AdminSystemId;
            updData.AgreementId = firstAgreementId;
            updData.CategoryId = firstCategoryId;
            var updDocumentResult = client.Request(api.UpdateDocument(updData)).ToWebApiResponseResult<DocumentWithoutFileViewModel>();
            updDocumentResult.StatusCode.Is(RegisterSuccessExpectStatusCode);
            var updateId = updDocumentResult.Result.DocumentId;

            #region UpdateDocument:FkError系
            //VendorId FkError
            {
                var fkData = updateData;
                fkData.VendorId = Guid.NewGuid().ToString();
                fkData.SystemId = AppConfig.AdminSystemId;
                fkData.AgreementId = firstAgreementId;
                fkData.CategoryId = firstCategoryId;
                client.Request(api.UpdateDocument(fkData)).ToWebApiResponseResult<DocumentWithoutFileViewModel>().StatusCode.Is(BadRequestStatusCode);
            }
            //SystemId FkError
            {
                var fkData = updateData;
                fkData.VendorId = AppConfig.AdminVendorId;
                fkData.SystemId = Guid.NewGuid().ToString();
                fkData.AgreementId = firstAgreementId;
                fkData.CategoryId = firstCategoryId;
                client.Request(api.UpdateDocument(fkData)).ToWebApiResponseResult<DocumentWithoutFileViewModel>().StatusCode.Is(BadRequestStatusCode);
            }
            //AgreementId FkError
            {
                var fkData = updateData;
                fkData.VendorId = AppConfig.AdminVendorId;
                fkData.SystemId = AppConfig.AdminSystemId;
                fkData.AgreementId = Guid.NewGuid().ToString();
                fkData.CategoryId = firstCategoryId;
                client.Request(api.UpdateDocument(fkData)).ToWebApiResponseResult<DocumentWithoutFileViewModel>().StatusCode.Is(BadRequestStatusCode);
            }
            //CategoryId FkError
            {
                var fkData = updateData;
                fkData.VendorId = AppConfig.AdminVendorId;
                fkData.SystemId = AppConfig.AdminSystemId;
                fkData.AgreementId = firstAgreementId;
                fkData.CategoryId = Guid.NewGuid().ToString();
                client.Request(api.UpdateDocument(fkData)).ToWebApiResponseResult<DocumentWithoutFileViewModel>().StatusCode.Is(BadRequestStatusCode);
            }

            #endregion


            // 再度更新したものを取得
            var getUpdDataResult = client.Request(api.GetDocument(updateId)).ToWebApiResponseResult<DocumentModel>();
            getUpdDataResult.StatusCode.Is(GetSuccessExpectStatusCode);
            var getUpdData = getUpdDataResult.Result;

            // 新規登録したものを更新登録したもののIDが同じか
            getRegData.DocumentId.Is(getUpdData.DocumentId);
            // 新規登録したものを更新登録したものが異なること（更新したため）
            getRegData.IsNot(getUpdData);

            // リスト取得
            var getDocumentListResult = client.Request(api.GetDocumentList(AppConfig.AdminVendorId)).ToWebApiResponseResult<List<DocumentModel>>();
            getDocumentListResult.StatusCode.Is(GetSuccessExpectStatusCode);
            var getDocumentList = getDocumentListResult.Result;
            // リストの中に新規登録したデータ（登録後、更新したデータ）が存在することを確認
            getDocumentList.Exists(x => x.DocumentId == getUpdData.DocumentId && x.Title == getUpdData.Title).Is(true);

            // 削除
            client.Request(api.DeleteDocument(registerId)).StatusCode.IsContains(DeleteExpectStatusCodes);

            // 削除したものを再度削除(NotFound)
            client.Request(api.DeleteDocument(registerId)).StatusCode.Is(NotFoundStatusCode);
            // 削除したものを取得(NotFound)
            client.Request(api.GetDocument(registerId)).StatusCode.Is(NotFoundStatusCode);
            // 削除したものを更新(NotFound)
            client.Request(api.UpdateDocument(updData)).StatusCode.Is(BadRequestStatusCode);
        }

        [TestMethod]
        public void Document_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var document = UnityCore.Resolve<IDocumentApi>();

            // ポータル用
            // portal, admin,  all, [null] 以外を指定。
            client.Request(document.GetList("aaa")).StatusCode.Is(BadRequestStatusCode);

            // GetAgreementList
            // なし。

            // GetCategoryList
            // なし。

            // Manage系
            // RegisterのValidationError
            RegisterValidationErrorData.ToList().ForEach(x => client.Request(document.RegisterDocument(x)).StatusCode.Is(BadRequestStatusCode));

            // RegisterのNullBody
            client.Request(document.RegisterDocument(null)).StatusCode.Is(BadRequestStatusCode);

            // UpdateのValidationError
            UpdateValidationErrorData.ForEach(x =>
                client.Request(document.UpdateDocument(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // RegisterのNullBody
            client.Request(document.UpdateDocument(null)).StatusCode.Is(BadRequestStatusCode);

            // GetのValidationError
            GetValidationErrorData.ForEach(x =>
                client.Request(document.GetDocument(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // DeleteのValidationError
            DeleteValidationErrorData.ForEach(x =>
                client.Request(document.DeleteDocument(x)).StatusCode.Is(BadRequestStatusCode)
            );

        }


        private enum PublicStatus
        {
            /// <summary>
            /// 連携しない
            /// </summary>
            None = 0,
            /// <summary>
            /// 一覧に公開する
            /// </summary>
            Public,
            /// <summary>
            /// 一覧には非表示
            /// </summary>
            Hidden,
        }

        private RegisterDocumentModel registerData = new RegisterDocumentModel()
        {
            Title = registerTitle,
            Detail = detail,
            CategoryId = Guid.NewGuid().ToString(),
            VendorId = Guid.NewGuid().ToString(),
            SystemId = Guid.NewGuid().ToString(),
            IsEnable = false,   // 無効にしておく
            IsAdminCheck = true,
            IsAdminStop = true,
            AgreementId = null,//指定不要データであるためデフォルトはnullにする。
            Password = null, //指定不要データであるためデフォルトはnull 
            IsPublicPortalStatus = PublicStatus.Public.ToString(),
            IsPublicAdminStatus = PublicStatus.Public.ToString()
        };

        private UpdateDocumentModel updateData
        {
            get
            {
                var result = s_mapper.Map<UpdateDocumentModel>(registerData);
                result.Title = updateTitle;
                return result;
            }
        }

        private RegisterDocumentModel validationBaseModel = new RegisterDocumentModel()
        {
            //DocumentId = Guid.NewGuid().ToString(),
            Title = "hoge_Name",
            Detail = "---itDetail---",
            CategoryId = Guid.NewGuid().ToString(),
            VendorId = Guid.NewGuid().ToString(),
            SystemId = Guid.NewGuid().ToString(),
            IsEnable = false, //無効化しておく
            IsAdminCheck = true,
            IsAdminStop = true,
            AgreementId = null,//指定不要データであるためデフォルトはnullにする。
            Password = null,//指定不要データであるためデフォルトはnull 
            IsPublicPortalStatus = PublicStatus.Public.ToString(),
            IsPublicAdminStatus = PublicStatus.Public.ToString()
        };

        private IEnumerable<RegisterDocumentModel> RegisterValidationErrorData
        {
            get
            {
                // Title がNull
                var x = validationBaseModel.DeepCopy();
                x.Title = null;
                yield return x;
                // Title が100文字オーバー
                x = validationBaseModel.DeepCopy();
                x.Title = new string('a', 101);
                yield return x;
                // Detail がNull
                x = validationBaseModel.DeepCopy();
                x.Detail = null;
                yield return x;
                // Detail が1000文字オーバー
                x = validationBaseModel.DeepCopy();
                x.Detail = new string('a', 1001);
                yield return x;
                // CategoryId がnull
                x = validationBaseModel.DeepCopy();
                x.CategoryId = null;
                yield return x;
                // CategoryId がGuidでない
                x = validationBaseModel.DeepCopy();
                x.CategoryId = "aaa";
                yield return x;
                // VendorId がnull
                x = validationBaseModel.DeepCopy();
                x.VendorId = null;
                yield return x;
                // VendorId がGuidでない
                x = validationBaseModel.DeepCopy();
                x.VendorId = "aaa";
                yield return x;
                // SystemId がnull
                x = validationBaseModel.DeepCopy();
                x.SystemId = null;
                yield return x;
                // SystemId がGuidでない
                x = validationBaseModel.DeepCopy();
                x.SystemId = "aaa";
                yield return x;
                // IsPublicPortalStatus がPublicStatusでない
                x = validationBaseModel.DeepCopy();
                x.IsPublicPortalStatus = "aaa";
                yield return x;
                // IsPublicAdminStatus がPublicStatusでない
                x = validationBaseModel.DeepCopy();
                x.IsPublicAdminStatus = "aaa";
                yield return x;
            }
        }
        /// <summary>
        /// 異常系データ(Update)
        /// </summary>
        public List<UpdateDocumentModel> UpdateValidationErrorData
        {
            get
            {
                // 新規と同じ
                var baseModel = s_mapper.Map<List<UpdateDocumentModel>>(RegisterValidationErrorData.ToList());

                // DocumentId がnull
                var documentIdNullModel = ValidationBaseModel;
                documentIdNullModel.DocumentId = null;

                // DocumentId がGuidでない
                var documentIdNotGuidModel = ValidationBaseModel;
                documentIdNotGuidModel.DocumentId = "aaa";

                baseModel.Add(documentIdNullModel);
                baseModel.Add(documentIdNotGuidModel);

                return baseModel;
            }
        }

        /// <summary>
        /// 異常系データ(Get)
        /// </summary>
        public List<string> GetValidationErrorData
        {
            get
            {
                // DocumentId がNull
                string documentIdNullModel = null;

                // DocumentId がGuidでない
                string documentIdNotGuidModel = "hoge";

                return new List<string>()
                {
                    documentIdNullModel,
                    documentIdNotGuidModel
                };
            }
        }


        /// <summary>
        /// 異常系データ(Delete)
        /// </summary>
        public List<string> DeleteValidationErrorData
        {
            get
            {
                // Getと同じ
                var baseModel = DeepCopy(GetValidationErrorData);

                return baseModel;
            }
        }

        private UpdateDocumentModel ValidationBaseModel = new UpdateDocumentModel()
        {
            DocumentId = Guid.NewGuid().ToString(),
            Title = "hoge_Name",
            Detail = "---itDetail---",
            CategoryId = Guid.NewGuid().ToString(),
            VendorId = Guid.NewGuid().ToString(),
            SystemId = Guid.NewGuid().ToString(),
            IsEnable = false,
            IsAdminCheck = true,
            IsAdminStop = true,
            AgreementId = null,//指定不要データであるためデフォルトはnullにする。
            Password = null,//指定不要データであるためデフォルトはnull 
            IsPublicPortalStatus = PublicStatus.Public.ToString(),
            IsPublicAdminStatus = PublicStatus.Public.ToString()
        };

        public string SmallContentsPath
        {
            get => Path.GetFullPath("TestContents/Admin/tractor_man.png");
        }
        public string LargeContentsPath
        {
            get => Path.GetFullPath("TestContents/Admin/IMG_20171118_122534916.jpg");
        }
    }
}
