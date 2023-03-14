using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ApplicationService.Impl;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    [TestClass]
    public class UnitTest_AttachFileApplicationService : UnitTestBase
    {

        [TestMethod]
        public void AttachFileApplicationService_CreateFile()
        {
            base.TestInitialize(true);

            string fileId = Guid.NewGuid().ToString();
            string fileName = "testname";
            string key = "testkey";
            string attachFilestorageId = "AAAAAAA";
            string contentType = "image/jpeg";
            long fileLength = 10000;
            bool isDrm = false;
            string drmType = "jdrm";
            string drmKey = "12345";
            string vendorId = Guid.NewGuid().ToString();
            Dictionary<string, string> metaList = new Dictionary<string, string>()
            {
                {"key1","value1" },
                {"key2","value2" },
                {"key3","value3" },
            };
            bool notAuthentication = (new Random().NextDouble() >= 0.5);

            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, "aaaaaa");
            perRequestDataContainer.SetupProperty(x => x.VendorId, vendorId);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);


            HttpResponseMessage httpRequestMessage = new HttpResponseMessage();
            httpRequestMessage.Content = new StreamContent(new System.IO.MemoryStream());

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.RegisterAttachFile(It.IsAny<AttachFileInformation>(), It.Is<NotAuthentication>(param => param.Value == notAuthentication))).Returns(httpRequestMessage);
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileStorageId(It.IsAny<VendorId>())).Returns(new AttachFileStorageId(attachFilestorageId.ToString()));
            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);

            Dictionary<MetaKey, MetaValue> mockMeta = new Dictionary<MetaKey, MetaValue>();
            foreach (var meta in metaList)
            {
                mockMeta.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }

            var testClass = new AttachFileApplicationService();
            var result = testClass.CreateFile(
                new FileName(fileName),
                new Key(key),
                new ContentType(contentType),
                new FileLength(fileLength),
                new IsDrm(isDrm),
                new DrmType(drmType),
                new DrmKey(drmKey),
                new Meta(mockMeta),
                new NotAuthentication(notAuthentication));

            result.IsNotNull();
        }

        [TestMethod]
        public void AttachFileApplicationService_UploadFile()
        {
            base.TestInitialize(true);

            string fileId = Guid.NewGuid().ToString();
            string fileName = "testname";
            string key = "testkey";
            string attachFilestorageId = "AAAAAAA";
            string blobUrl = "AAAAAAA/BBBBB";
            string contentType = "image/jpeg";
            long fileLength = 10000;
            bool isDrm = false;
            string drmType = "jdrm";
            string drmKey = "12345";
            bool notAuthentication = (new Random().NextDouble() >= 0.5);
            Guid vendorId = Guid.NewGuid();
            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.TempFileUpload(It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<InputStream>(), It.IsAny<IsAppendStream>(), It.IsAny<AppendPosition>())).Returns(new FilePath("aa"));
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var mockData = AttachFileInformation.Restore(new FileId(Guid.Parse(fileId)), new FileName(fileName),
                new BlobUrl(blobUrl), new AttachFileStorageId(attachFilestorageId), new Key(key),
                new ContentType(contentType), new FileLength(fileLength), new IsDrm(isDrm),
                new DrmType(drmType), new DrmKey(drmKey), new VendorId(vendorId), new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));


            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileInformation(It.IsAny<FileId>(), It.Is<NotAuthentication>(param => param.Value == notAuthentication))).Returns(mockData);

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            var parmFileId = new FileId(Guid.Parse(fileId));
            var parmFileName = new FileName(fileName);
            var parmInputStream = new InputStream(new MemoryStream());
            var parmIsEndStream = new IsEndStream(false);
            var parmIsAppendStream = new IsAppendStream(false);
            var parmAppendPosition = new AppendPosition(0);
            var parmNotAuthentication = new NotAuthentication(notAuthentication);
            var testClass = new AttachFileApplicationService();
            testClass.UploadFile(
                parmFileId,
                parmInputStream,
                parmIsEndStream,
                parmIsAppendStream,
                parmAppendPosition,
                parmNotAuthentication);
        }

        [TestMethod]
        public void AttachFileApplicationService_GetFile()
        {
            base.TestInitialize(true);

            string fileId = Guid.NewGuid().ToString();
            string fileName = "testname";
            string key = "testkey";
            string attachFilestorageId = "AAAAAAA";
            string blobUrl = "AAAAAAA/BBBBB";
            string contentType = "image/jpeg";
            long fileLength = 10000;
            bool isDrm = false;
            string drmType = "jdrm";
            string drmKey = "12345";
            bool notAuthentication = (new Random().NextDouble() >= 0.5);
            Guid vendorId = Guid.NewGuid();
            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            List<byte> test = new List<byte>() { 1, 2, 3 };
            mockAttachFileRepository.Setup(x => x.GetFileStream(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>())).Returns(new MemoryStream(test.ToArray()));
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var mockData = AttachFileInformation.Restore(new FileId(Guid.Parse(fileId)), new FileName(fileName),
                new BlobUrl(blobUrl), new AttachFileStorageId(attachFilestorageId), new Key(key),
                new ContentType(contentType), new FileLength(fileLength), new IsDrm(isDrm),
                new DrmType(drmType), new DrmKey(drmKey), new VendorId(vendorId), new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));


            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileInformation(It.IsAny<FileId>(), It.Is<NotAuthentication>(param => param.Value == notAuthentication))).Returns(mockData);

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            var parmFileId = new FileId(Guid.Parse(fileId));
            var parmKey = new Key(key);
            var parmNotAuthentication = new NotAuthentication(notAuthentication);
            var testClass = new AttachFileApplicationService();
            var result = testClass.GetFile(parmFileId, parmKey, parmNotAuthentication);
            result.StatusCode.IsStructuralEqual(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void AttachFileApplicationService_GetFileMeta()
        {
            base.TestInitialize(true);

            string fileId = Guid.NewGuid().ToString();
            string fileName = "testname";
            string key = "testkey";
            string attachFilestorageId = "AAAAAAA";
            string blobUrl = "AAAAAAA/BBBBB";
            string contentType = "image/jpeg";
            long fileLength = 10000;
            bool isDrm = false;
            string drmType = "jdrm";
            string drmKey = "12345";
            Guid vendorId = Guid.NewGuid();

            var mockData = AttachFileInformation.Restore(new FileId(Guid.Parse(fileId)), new FileName(fileName),
                new BlobUrl(blobUrl), new AttachFileStorageId(attachFilestorageId), new Key(key),
                new ContentType(contentType), new FileLength(fileLength), new IsDrm(isDrm),
                new DrmType(drmType), new DrmKey(drmKey), new VendorId(vendorId), new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));


            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileInformation(It.IsAny<FileId>(), It.Is<NotAuthentication>(param => param.Value == true))).Returns(mockData);

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            var parmFileId = new FileId(Guid.Parse(fileId));
            var parmKey = new Key(key);
            var testClass = new AttachFileApplicationService();
            var result = testClass.GetFileMeta(parmFileId, parmKey);
            result.IsStructuralEqual(mockData);
        }

        [TestMethod]
        public void AttachFileApplicationService_DeleteFile()
        {
            base.TestInitialize(true);

            string fileId = Guid.NewGuid().ToString();
            string fileName = "testname";
            string key = "testkey";
            string attachFilestorageId = "AAAAAAA";
            string blobUrl = "AAAAAAA/BBBBB";
            string contentType = "image/jpeg";
            long fileLength = 10000;
            bool isDrm = false;
            string drmType = "jdrm";
            string drmKey = "12345";
            bool notAuthentication = (new Random().NextDouble() >= 0.5);
            Guid vendorId = Guid.NewGuid();

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.DeleteFile(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>()));
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.DeleteAttachFile(It.Is<FileId>(s => s.Value == fileId), It.Is<NotAuthentication>(s => s.Value == notAuthentication))).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK));

            var mockData = AttachFileInformation.Restore(new FileId(Guid.Parse(fileId)), new FileName(fileName),
                new BlobUrl(blobUrl), new AttachFileStorageId(attachFilestorageId), new Key(key),
                new ContentType(contentType), new FileLength(fileLength), new IsDrm(isDrm),
                new DrmType(drmType), new DrmKey(drmKey), new VendorId(vendorId), new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));


            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileInformation(It.IsAny<FileId>(), It.Is<NotAuthentication>(param => param.Value == notAuthentication))).Returns(mockData);

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            var parmFileId = new FileId(Guid.Parse(fileId));
            var parmKey = new Key(key);
            var parmNotAuthentication = new NotAuthentication(notAuthentication);
            var testClass = new AttachFileApplicationService();
            var result = testClass.DeleteFile(parmFileId, parmKey, parmNotAuthentication);
            result.StatusCode.IsStructuralEqual(System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        public void AttachFileApplicationService_SearchByMeta()
        {
            base.TestInitialize(true);

            var metaKey = new MetaKey("aa");
            var metaValue = new MetaValue("bb");
            var d = new Dictionary<MetaKey, MetaValue>();
            d.Add(metaKey, metaValue);
            var meta = new Meta(d);
            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileInformationSearchByMeta(It.IsAny<Meta>())).Returns(new List<AttachFileInformation>() { });
            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            var testClass = new AttachFileApplicationService();
            var result = testClass.SearchByMeta(meta);
        }
    }
}
