using System;
using System.Collections.Generic;
using EasyCaching.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Infrastructure.Database.AttachFile;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_AttachFileInformationRepository : UnitTestBase
    {
        public static string CACHE_KEY_ATTACH_FILE_STORAGE_ID = "AttachFileStorageId";

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        /// GetAttachFileStorageId
        /// 処理概要
        ///  AttachFileStorageから指定されたVendorのStorageのIDを取得する
        [TestMethod]
        public void AttachFileInformationRepository_GetAttachFileStorageId_正常系_キャッシュヒット()
        {
            var cacheExpireTime = TimeSpan.Parse("0:30:00");
            var vendorId = new VendorId(Guid.NewGuid());
            var cacheAttachFileBlobStorageId = new AttachFileStorageId(Guid.NewGuid().ToString());
            var dbAttachFileBlobStorageId = new AttachFileStorageId(Guid.NewGuid().ToString());
            var key = CacheManager.CreateKey(CACHE_KEY_ATTACH_FILE_STORAGE_ID, vendorId.Value);

            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<DB_AttachFileStorage>(key, It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns(new DB_AttachFileStorage() { attachfile_storage_id = Guid.Parse(cacheAttachFileBlobStorageId.Value) });
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var testClass = UnityContainer.Resolve<IAttachFileInformationRepository>();
            var result = testClass.GetAttachFileStorageId(vendorId);
            result.Value.IsStructuralEqual(cacheAttachFileBlobStorageId.Value);
        }

        [TestMethod]
        public void AttachFileInformationRepository_GetAttachFileStorageId_正常系_キャッシュミスヒット()
        {
            var vendorId = new VendorId(Guid.NewGuid());
            var dbAttachFileBlobStorageId = new AttachFileStorageId(Guid.NewGuid().ToString());

            var mockDb = new Mock<IJPDataHubDbConnection>();
            mockDb.Setup(x => x.Query<DB_AttachFileStorage>(
                It.IsAny<string>(), It.IsAny<object>()
                )).Returns(new List<DB_AttachFileStorage>() { new DB_AttachFileStorage() { attachfile_storage_id = Guid.Parse(dbAttachFileBlobStorageId.Value) } });
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("AttachFile", mockDb.Object);

            var providerName = "DynamicApi";
            var mockProvider = new Mock<IEasyCachingProvider>();
            mockProvider.Setup(x => x.Name).Returns(providerName);
            mockProvider.Setup(x => x.Set(It.IsAny<String>(), It.IsAny<object>(), It.IsAny<TimeSpan>()));
            mockProvider.Setup(x => x.Get<object>(It.IsAny<String>())).Returns(new CacheValue<object>(null, false));
            UnityContainer.RegisterInstance<IEasyCachingProvider>(providerName, mockProvider.Object);

            var sql = @"
SELECT
    a.attachfile_storage_id
FROM
AttachFileStorage a INNER JOIN VendorAttachfilestorage va ON a.attachfile_storage_id = va.attachfile_storage_id 
WHERE
va.vendor_id = @vendor_id
and a.is_active = 1 
and va.is_active = 1
and va.is_current = 1
and a.is_full = 0";

            var testClass = UnityContainer.Resolve<IAttachFileInformationRepository>();
            var result = testClass.GetAttachFileStorageId(vendorId);

            mockDb.Verify(x => x.Query<DB_AttachFileStorage>(
                It.Is<string>(y => y.Trim() == sql.Trim()),
                It.Is<object>(o => o.ToString().ToLower() == ((new { vendor_id = vendorId.Value })).ToString().ToLower())),
                Times.Once());
            result.IsStructuralEqual(dbAttachFileBlobStorageId);
        }
    }
}
