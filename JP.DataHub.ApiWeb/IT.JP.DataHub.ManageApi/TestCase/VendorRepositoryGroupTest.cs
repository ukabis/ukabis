using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.Vendor;
using IT.JP.DataHub.ManageApi.WebApi.Models.VendorRepositoryGroup;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    [TestClass]
    public partial class VendorRepositoryGroupTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }
        //DomainDataSyncが動作することが必須となる
        /// <summary>
        /// リポジトリグループ-管理画面（ベンダー）の正常系テスト
        /// </summary>
        [TestMethod]
        public void VendorRepositoryGroup_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IVendorRepositoryGroupApi>();
            var repositoryGroupApi = UnityCore.Resolve<IRepositoryGroupApi>();
            var vendorApi = UnityCore.Resolve<IVendorApi>();


            //【準備】テストで使用するRepositoryGroupのデータが存在しないことを確認し、存在するなら消す。
            var repositoryGroupRegObj = RepositoryGroupRegData; // 新規登録するデータ
            repositoryGroupRegObj.RepositoryGroupName = "---itRegister-VendorRepositoryGroup-RepositoryGroup---"; // Repositoryのテスト実行時と判別しやすくするためリネーム

            var repositoryList = client.GetWebApiResponseResult(repositoryGroupApi.GetRepositoryGroupList()).Assert(GetSuccessExpectStatusCode).Result;
            repositoryList?.Where(x => x.RepositoryGroupName == repositoryGroupRegObj.RepositoryGroupName).ToList().ForEach(
                    x => client.GetWebApiResponseResult(repositoryGroupApi.DeleteRepositoryGroup(x.RepositoryGroupId)).Assert(DeleteSuccessStatusCode));

            // 【準備】一覧取得ができること(type)
            var typeList = client.GetWebApiResponseResult(repositoryGroupApi.GetRepositoryGroupTypeList()).Assert(GetSuccessExpectStatusCode).Result;

            // 【準備】タイプの一覧から１番目の要素を抜き出す。
            var firstRepositoryTypeCd = typeList.First().RepositoryTypeCd;

            // 【準備】RepositoryGroup新規登録
            repositoryGroupRegObj.RepositoryTypeCd = firstRepositoryTypeCd;
            var repositoryGroupRegId = client.GetWebApiResponseResult(repositoryGroupApi.RegisterRepositoryGroup(repositoryGroupRegObj)).Assert(RegisterSuccessExpectStatusCode).Result.RepositoryGroupId;

            // 【準備】テストで使用するVendorのデータが存在しないことを確認し、存在するなら消す。
            var vendorRegObj = RegisterVendorData; // 新規登録するデータ
            vendorRegObj.VendorName = "---itRegister-VendorRepositoryGroup-Vendor---"; // Vendorのテスト実行時と判別しやすくするためリネーム

            var vendorList = client.GetWebApiResponseResult(vendorApi.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            vendorList?.Where(x => x.VendorName == vendorRegObj.VendorName).ToList().ForEach(x =>
                    client.GetWebApiResponseResult(vendorApi.Delete(x.VendorId)).Assert(DeleteSuccessStatusCode));

            // 【準備】Vendor新規登録
            var vendorRegId = client.GetWebApiResponseResult(vendorApi.Register(vendorRegObj)).Assert(RegisterSuccessExpectStatusCode).Result.VendorId;

            // ActivateVendorRepositoryGroup（存在しないRepositoryGroupIdでBadRequest）
            var activateObj = ActivateVendorRepositoryGroupData;
            activateObj.RepositoryGroupId = Guid.NewGuid().ToString();
            activateObj.VendorId = vendorRegId;
            client.GetWebApiResponseResult(api.ActivateVendorRepositoryGroup(activateObj)).Assert(BadRequestStatusCode);

            // ActivateVendorRepositoryGroup（存在しないVendorIdでBadRequest）
            activateObj.VendorId = Guid.NewGuid().ToString();
            activateObj.RepositoryGroupId = repositoryGroupRegId;
            client.GetWebApiResponseResult(api.ActivateVendorRepositoryGroup(activateObj)).Assert(BadRequestStatusCode);

            // ActivateVendorRepositoryGroup（有効化）
            activateObj.VendorId = vendorRegId;
            activateObj.RepositoryGroupId = repositoryGroupRegId;
            client.GetWebApiResponseResult(api.ActivateVendorRepositoryGroup(activateObj)).Assert(RegisterSuccessExpectStatusCode);

            // GetVendorRepositoryGroup
            var getRegData = client.GetWebApiResponseResult(api.GetVendorRepositoryGroup(vendorRegId, repositoryGroupRegId)).Assert(GetSuccessExpectStatusCode).Result;
            var verifyRegData = VendorRepositoryGroupInfoData;
            verifyRegData.VendorId = vendorRegId;
            verifyRegData.RepositoryGroupId = repositoryGroupRegId;
            verifyRegData.RepositoryGroupName = repositoryGroupRegObj.RepositoryGroupName;
            getRegData.IsStructuralEqual(verifyRegData);

            // GetVendorRepositoryGroupList
            var list = client.GetWebApiResponseResult(api.GetVendorRepositoryGroupList()).Assert(GetSuccessExpectStatusCode).Result;
            // 無効のVendorは取得できない
            Assert.IsFalse(list.Any(x => x.VendorId == vendorRegId));

            // Vendor毎にGetVendorRepositoryGroupList
            var listByVendor = client.GetWebApiResponseResult(api.GetVendorRepositoryGroupList(vendorRegId)).Assert(GetSuccessExpectStatusCode).Result.First();
            Assert.IsTrue(listByVendor.VendorRepositoryGroupItems.Any(x => x.RepositoryGroupId == repositoryGroupRegId));

            // ActivateVendorRepositoryGroup（無効化）
            activateObj.Active = false;
            client.GetWebApiResponseResult(api.ActivateVendorRepositoryGroup(activateObj)).Assert(RegisterSuccessExpectStatusCode);

            // GetVendorRepositoryGroup（NotFound）
            client.GetWebApiResponseResult(api.GetVendorRepositoryGroup(vendorRegId, repositoryGroupRegId)).Assert(NotFoundStatusCode);

            // GetVendorRepositoryGroupList（無効化後）
            var listAfterInactivation = client.GetWebApiResponseResult(api.GetVendorRepositoryGroupList()).Assert(GetSuccessExpectStatusCode).Result;
            // 無効のVendorは取得できない
            Assert.IsFalse(list.Any(x => x.VendorId == vendorRegId));

            // Vendor毎にGetVendorRepositoryGroup（NotFound）
            var listByVendorAfterInactivation = client.GetWebApiResponseResult(api.GetVendorRepositoryGroupList(vendorRegId)).Assert(GetSuccessExpectStatusCode).Result.First();
            Assert.IsFalse(listByVendorAfterInactivation.VendorRepositoryGroupItems.Any(x => x.RepositoryGroupId == repositoryGroupRegId));

            // ActivateVendorRepositoryGroup（使用中のものを無効化時のBadRequest）
            // 使用中のVendorIdとRepositoryGroupを探索
            var vendorUsedItemContaining = listAfterInactivation.Where(x => x.VendorRepositoryGroupItems.Any(y => y.Used == true)).First();
            var usedVendorId = vendorUsedItemContaining.VendorId;
            var usedRepositoryGroupId = vendorUsedItemContaining.VendorRepositoryGroupItems.Where(x => x.Used == true).First().RepositoryGroupId;
            // 検証
            activateObj.VendorId = usedVendorId;
            activateObj.RepositoryGroupId = usedRepositoryGroupId;
            client.GetWebApiResponseResult(api.ActivateVendorRepositoryGroup(activateObj)).Assert(BadRequestStatusCode);

            // 【後処理】作成したRepositoryGroupとVendorを削除
            client.GetWebApiResponseResult(repositoryGroupApi.DeleteRepositoryGroup(repositoryGroupRegId)).Assert(DeleteSuccessStatusCode);
            client.GetWebApiResponseResult(vendorApi.Delete(vendorRegId)).Assert(DeleteSuccessStatusCode);
        }
        /// <summary>
        /// リポジトリグループ-管理画面（ベンダー）の異常系テスト
        /// </summary>
        [TestMethod]
        public void VendorRepositoryGroup_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IVendorRepositoryGroupApi>();

            // GetVendorRepositoryGroupのValidationError
            ActivateVendorRepositoryGroupValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetVendorRepositoryGroup(x.VendorId, x.RepositoryGroupId)).Assert(BadRequestStatusCode)
            );

            // GetVendorRepositoryGroupのNotFound
            client.GetWebApiResponseResult(api.GetVendorRepositoryGroup(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // GetVendorRepositoryGroupListのNotFound
            // RepositoryGroupテーブルの全レコードを削除する必要があるため、ITでは省略

            // ベンダー毎のGetVendorRepositoryGroupListのNotFound
            client.GetWebApiResponseResult(api.GetVendorRepositoryGroupList(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // ActivateVendorRepositoryGroupのValidationError
            ActivateVendorRepositoryGroupValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.ActivateVendorRepositoryGroup(x)).Assert(BadRequestStatusCode)
            );

            // ActivateVendorRepositoryGroupのNullBody
            client.GetWebApiResponseResult(api.ActivateVendorRepositoryGroup(null)).Assert(BadRequestStatusCode);

            // ActivateVendorRepositoryGroupの存在しないVendor、RepositoryGroupのBadRequest
            // 登録可能なデータからErrorデータを作らねばならず、準備が冗長になるためNormalシナリオ側で実行

            // ActivateVendorRepositoryGroupの使用中のものを無効化時のBadRequest
            // 既に使用中のVendorRepositoryGroupを対象としなければならず、準備が冗長になるためNormalシナリオ側で実行
        }
        #region Data

        public string RegisterVendorName = "------IntegratedTestVendorName------";
        public string UpdateVendorName = "------IntegratedTestUpdateVendorName------";

        /// <summary>
        /// 新規登録正常系データ
        /// </summary>
        public RegisterVendorModel RegisterVendorData
        {
            get => 
            (
                new RegisterVendorModel()
                {
                    VendorName = RegisterVendorName,
                    IsDataOffer = false,
                    IsDataUse = false,
                    IsEnable = false,
                }
            );
        }
        public string RepositoryGroupNameRegData = "---itRegister---";
        public string RepositoryGroupNameUpdData = "---itUpdate---";

        public string RepositoryGroupConnectionStringRegData = "---CONNECTION-STRING-REG---";
        public string RepositoryGroupConnectionStringUpdData1 = "---CONNECTION-STRING-UPD1---";
        public string RepositoryGroupConnectionStringUpdData2 = "---CONNECTION-STRING-UPD2---";
        /// <summary>
        /// ActivateVendorRepositoryGroup正常系データ
        /// </summary>
        public ActivateVendorRepositoryGroupModel ActivateVendorRepositoryGroupData
        {
            get =>
                new ActivateVendorRepositoryGroupModel()
                {
                    VendorId = Guid.Empty.ToString(),
                    RepositoryGroupId = Guid.Empty.ToString(),
                    Active = true
                };
        }

        /// <summary>
        /// GetVendorRepositoryGroup正常系データ
        /// </summary>
        public VendorRepositoryGroupModel VendorRepositoryGroupInfoData
        {
            get =>
                new VendorRepositoryGroupModel()
                {
                    VendorId = Guid.Empty.ToString(),
                    RepositoryGroupId = Guid.Empty.ToString(),
                    RepositoryGroupName = string.Empty,
                    Used = false
                };
        }
        public RepositoryGroupModel RepositoryGroupRegData
        {
            get =>
                new RepositoryGroupModel()
                {
                        //RepositoryGroupId = Guid.Empty.ToString(), 入力不要。
                    RepositoryGroupName = RepositoryGroupNameRegData,
                    RepositoryTypeCd = null, //FK要素なので、タイプの一覧から取得する。
                    SortNo = "0",
                    IsDefault = bool.FalseString,
                    IsEnable = bool.TrueString,
                    PhysicalRepositoryList = new List<PhysicalRepositoryModel>
                    {
                            new PhysicalRepositoryModel
                            {
                                ConnectionString = RepositoryGroupConnectionStringRegData,
                                IsActive = true,
                                IsFull = true
                            }
                    }
                };
        }

        public RepositoryGroupModel RepositoryGroupUpdData
        {
            get =>
                new RepositoryGroupModel()
                {
                        //RepositoryGroupId = Guid.Empty.ToString(), 入力不要。
                    RepositoryGroupName = RepositoryGroupNameUpdData,
                    RepositoryTypeCd = null, //FK要素なので、タイプの一覧から取得する。
                    SortNo = "100",
                    IsDefault = bool.TrueString,
                    IsEnable = bool.FalseString,
                    PhysicalRepositoryList = new List<PhysicalRepositoryModel>
                    {
                            new PhysicalRepositoryModel
                            {
                                ConnectionString = RepositoryGroupConnectionStringUpdData1,
                                IsActive = true,
                                IsFull = true
                            },
                            new PhysicalRepositoryModel
                            {
                                ConnectionString = RepositoryGroupConnectionStringUpdData2,
                                IsActive = true,
                                IsFull = false
                            }
                    }
                };
        }

        #endregion

        #region Validation/Error
        /// <summary>
        /// リポジトリグループ-管理画面（ベンダー）異常系データ
        /// Validation Error
        /// </summary>
        public List<ActivateVendorRepositoryGroupModel> ActivateVendorRepositoryGroupValidationErrorData
        {
            get
            {
                // RepositoryGroupId がnull
                var RepositoryGroupIdNullModel = ActivateVendorRepositoryGroupData;
                RepositoryGroupIdNullModel.RepositoryGroupId = null;

                // RepositoryGroupId がGuidでない
                var RepositoryGroupIdNotGuidModel = ActivateVendorRepositoryGroupData;
                RepositoryGroupIdNotGuidModel.RepositoryGroupId = "aaa";

                // VendorId がnull
                var VendorIdNullModel = ActivateVendorRepositoryGroupData;
                VendorIdNullModel.VendorId = null;

                // VendorId がGuidでない
                var VendorIdNotGuidModel = ActivateVendorRepositoryGroupData;
                VendorIdNotGuidModel.VendorId = "aaa";


                return new List<ActivateVendorRepositoryGroupModel>()
                {
                    RepositoryGroupIdNullModel,
                    RepositoryGroupIdNotGuidModel,
                    VendorIdNullModel,
                    VendorIdNotGuidModel,
                };
            }
        }
        #endregion
    }
}
