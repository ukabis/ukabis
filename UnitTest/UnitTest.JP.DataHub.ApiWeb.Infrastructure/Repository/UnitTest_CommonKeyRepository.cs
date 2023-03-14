using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EasyCaching.Core;
using EasyCaching.InMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_CommonKeyRepository : UnitTestBase
    {
        private ICommonKeyRepository _testClass;
        private string _cacheKey = typeof(CommonKey).Namespace + ":" + nameof(CommonKey);
        private string _systemId = Guid.NewGuid().ToString();
        private CommonKey _commonKey;


        [TestInitialize]
        public override void TestInitialize()
        {
            _commonKey = CommonKey.CreateNewCommonKey(_systemId, null, CreateCommonKeyParameters());

            base.TestInitialize(true);

            UnityContainer.RegisterType<ICommonKeyRepository, CommonKeyRepository>();

            var cachingProvider = new DefaultInMemoryCachingProvider(
                "DynamicApi", 
                new[] 
                { 
                    new InMemoryCaching("DynamicApi", new InMemoryCachingOptions()
                    {
                        ExpirationScanFrequency = 1
                    }) 
                }, 
                new InMemoryOptions()
                {
                    MaxRdSecond = 0
                }, 
                null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>();

            // テスト対象のインスタンスを作成
            _testClass = UnityContainer.Resolve<ICommonKeyRepository>();
        }

        [TestMethod]
        public void Get_NotFound()
        {
            // テスト対象のメソッド実行
            var result = _testClass.Get(new SystemId(_systemId), new CommonKeyId(Guid.NewGuid().ToString()));

            // 結果をチェック
            result.IsNull();
        }

        [TestMethod]
        public async Task Get_Expired()
        {
            var commonKey2 = CommonKey.CreateNewCommonKey(_systemId, DateTime.UtcNow.AddSeconds(10), CreateCommonKeyParameters());

            // テスト対象のメソッド実行
            _testClass.Register(commonKey2);

            await Task.Delay(8000);
            var result = _testClass.Get(commonKey2.SystemId, commonKey2.CommonKeyId);
            // 結果をチェック
            result.IsStructuralEqual(commonKey2);

            await Task.Delay(3000);
            result = _testClass.Get(commonKey2.SystemId, commonKey2.CommonKeyId);
            // 結果をチェック
            result.IsNull();
        }

        [TestMethod]
        public void RegisterGet()
        {
            // テスト対象のメソッド実行
            _testClass.Register(_commonKey);
            var result = _testClass.Get(_commonKey.SystemId, _commonKey.CommonKeyId);

            // 結果をチェック
            result.IsStructuralEqual(_commonKey);
        }

        private CommonKeyParameters CreateCommonKeyParameters()
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                csp.GenerateKey();
                return new CommonKeyParameters(csp.Key, null, 128, CipherMode.CBC, PaddingMode.PKCS7);
            }
        }
    }
}
