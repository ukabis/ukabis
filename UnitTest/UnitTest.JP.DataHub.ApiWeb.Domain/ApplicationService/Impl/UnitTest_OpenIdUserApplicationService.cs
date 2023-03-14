using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.ApplicationService.Impl;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.OpenIdUser;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    [TestClass]
    public class UnitTest_OpenIdUserApplicationService : UnitTestBase
    {
        private UnityContainer _container;
        private Mock<IOpenIdUserRepository> _mockRepo;
        private IOpenIdUserApplicationService _testClass;

        // テストパラメーター
        private string _objectId = Guid.NewGuid().ToString();
        private string _objectId2 = Guid.NewGuid().ToString();
        private string _objectId3 = Guid.NewGuid().ToString();
        private string _objectId4 = Guid.NewGuid().ToString();
        private string _systemId = Guid.NewGuid().ToString();
        private string _systemId2 = Guid.NewGuid().ToString();
        private const string _userId = "user1@example.net";
        private const string _userId2 = "user2@example.com";
        private const string _userId3 = "user3@example.com";
        private const string _userId4 = "user4@example.com";
        private OpenIdUser _user;
        private OpenIdUser _user2;
        private OpenIdUser _user3;
        private OpenIdUser _user4;


        public UnitTest_OpenIdUserApplicationService()
        {
            _user = new OpenIdUser(_objectId, _userId, _systemId, "p@ssw0rd", "テストユーザー１", DateTime.UtcNow);
            _user2 = new OpenIdUser(_objectId2, _userId2, _systemId2, "p@ssw0rd2", "テストユーザー２", DateTime.UtcNow);
            _user3 = new OpenIdUser(_objectId3, _userId3, _systemId, "p@ssw0rd3", "テストユーザー３", null);
            _user4 = new OpenIdUser(_objectId4, _userId4, null, "p@ssw0rd4", "テストユーザー４", null);

            // モックの作成
            _mockRepo = new Mock<IOpenIdUserRepository>();
            _mockRepo.Setup(s => s.Get(It.Is<UserId>(x => x.Value == _userId))).ReturnsAsync(_user);
            _mockRepo.Setup(s => s.Get(It.Is<UserId>(x => x.Value == _userId2))).ReturnsAsync(_user2);
            _mockRepo.Setup(s => s.Get(It.Is<UserId>(x => x.Value == _userId4))).ReturnsAsync(_user4);
            _mockRepo.Setup(s => s.GetList(It.IsAny<SystemId>())).ReturnsAsync(new[] { _user });
            _mockRepo.Setup(s => s.Register(It.IsAny<OpenIdUser>())).ReturnsAsync(_user3);

            // Unityの初期化
            _container = new UnityContainer();
            _container.RegisterType<IOpenIdUserApplicationService, OpenIdUserApplicationService>();
            _container.RegisterInstance(_mockRepo.Object);
            UnityCore.UnityContainer = _container;

            // テスト対象のインスタンスを作成
            _testClass = _container.Resolve<IOpenIdUserApplicationService>();
        }

        [TestMethod]
        public async Task Delete()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.Delete(new SystemId(_systemId), new UserId(_userId));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId)));
            _mockRepo.Verify(s => s.Delete(It.Is<ObjectId>(x => x.Value == _objectId)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Deleted);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task Delete_Forbidden()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.Delete(new SystemId(_systemId), new UserId(_userId2));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId2)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Forbidden);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task Delete_NotFound()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.Delete(new SystemId(_systemId), new UserId(_userId3));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId3)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.NotFound);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task Get()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.Get(new SystemId(_systemId), new UserId(_userId));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Selected);
            result.UserInfo.IsStructuralEqual(_user);
        }

        [TestMethod]
        public async Task Get_Forbidden()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.Get(new SystemId(_systemId), new UserId(_userId2));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId2)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Forbidden);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task Get_NotFound()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.Get(new SystemId(_systemId), new UserId(_userId3));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId3)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.NotFound);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task Get_NotFound_NoSystemOfUser()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.Get(new SystemId(_systemId), new UserId(_userId4));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId4)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.NotFound);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task Get_NotFound_NoSystemOfClient()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.Get(null, new UserId(_userId4));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId4)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.NotFound);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task GetFullAccess()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.GetFullAccess(new UserId(_userId2));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId2)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Selected);
            result.UserInfo.IsStructuralEqual(_user2);
        }

        /// <summary>
        /// システムのないユーザーが取得できる
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetFullAccess_NoSystemOfUser()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.GetFullAccess(new UserId(_userId4));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId4)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Selected);
            result.UserInfo.IsStructuralEqual(_user4);
        }

        [TestMethod]
        public async Task GetFullAccess_NoFound()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.GetFullAccess(new UserId(_userId3));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId3)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.NotFound);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task GetList()
        {
            // テスト対象のメソッド実行
            var result = await _testClass.GetList(new SystemId(_systemId));

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.GetList(It.Is<SystemId>(x => x.Value == _systemId)));

            // 結果をチェック
            result.ToArray().IsStructuralEqual(new[] { _user });
        }

        [TestMethod]
        public async Task Register()
        {
            var newUser = new OpenIdUser(null, _userId3, _systemId, "p@ssw0rd3", "テストユーザー３", null);

            // テスト対象のメソッド実行
            var result = await _testClass.Register(newUser);

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId3)));
            _mockRepo.Verify(s => s.Register(It.Is<OpenIdUser>(x => x.UserId.Value == _userId3)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Created);
            result.UserInfo.IsStructuralEqual(_user3);
        }

        [TestMethod]
        public async Task Register_Update()
        {
            var newUser = new OpenIdUser(null, _userId, _systemId, "p@ssw0rd1", "テストユーザー１", null);

            // テスト対象のメソッド実行
            var result = await _testClass.Register(newUser);

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId)));
            _mockRepo.Verify(s => s.Update(It.Is<ObjectId>(x => x.Value == _objectId),
                It.Is<OpenIdUser>(x => x.UserId.Value == _userId)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Updated);
            result.UserInfo.IsNull();
        }

        [TestMethod]
        public async Task Register_Update_Forbidden()
        {
            var newUser = new OpenIdUser(null, _userId2, _systemId, "p@ssw0rd2", "テストユーザー２", null);

            // テスト対象のメソッド実行
            var result = await _testClass.Register(newUser);

            // モックの呼び出しを検証
            _mockRepo.Verify(s => s.Get(It.Is<UserId>(x => x.Value == _userId2)));

            // 結果をチェック
            result.Status.Is(OpenIdUserOperationStatus.Forbidden);
            result.UserInfo.IsNull();
        }
    }
}
