using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    [TestClass]
    public class UnitTest_AttachFileInformation : UnitTestBase
    {
        private string _fileId = Guid.NewGuid().ToString();
        private string _fileName = "testname";
        private string _key = "testkey";
        private string _attachFilestorageId = "AAAAAAA";
        private string _blobUrl = "AAAAAAA/BBBBB";
        private string _contentType = "image/jpeg";
        private long _fileLength = 10000;
        private bool _isDrm = false;
        private string _drmType = "jdrm";
        private string _drmKey = "12345";
        private string _vendorId = Guid.NewGuid().ToString();
        private Dictionary<string, string> _metaList = new Dictionary<string, string>()
            {
                {"key1","value1" },
                {"key2","value2" },
                {"key3","value3" },
            };

        /// CreateFile
        /// 処理概要
        /// 与えられた引数をもとにAttachFileを生成する。
        /// 引数のVendorIDをもとにAttachFileStorageIdを取得する
        /// OtherDomainを経由してCosmosDBにAttachFileの情報を登録する
        [TestMethod]
        public void AttachFileInformation_CreateFile_正常系()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid().ToString();
            var attachStorageId = Guid.NewGuid();

            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, "aaaaaa");
            perRequestDataContainer.SetupProperty(x => x.VendorId, vendorId.ToString());
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);
            HttpResponseMessage httpRequestMessage = new HttpResponseMessage();
            httpRequestMessage.Content = new StreamContent(new System.IO.MemoryStream());

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();

            mockAttachFileInformationRepository.Setup(x => x.RegisterAttachFile(It.IsAny<AttachFileInformation>(), It.IsAny<NotAuthentication>())).Returns(httpRequestMessage);
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileStorageId(It.Is<VendorId>(s => s.Value == vendorId))).Returns(new AttachFileStorageId(attachStorageId.ToString()));

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            Dictionary<MetaKey, MetaValue> mockMeta = new Dictionary<MetaKey, MetaValue>();

            foreach (var meta in _metaList)
            {
                mockMeta.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }

            var result = AttachFileInformation.Create(new FileName(_fileName), new Key(_key), new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm), new DrmType(_drmType), new DrmKey(_drmKey), new Meta(mockMeta));

            result.FileId.IsNotNull();
            result.BlobUrl.Value.IsStructuralEqual($"/{vendorId.ToString()}/{result.FileId.Value.ToString()}/{_fileName}");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AttachFileInformation_CreateFile_異常系_パラメータ異常_VendorIdがNULL()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid().ToString();
            var attachStorageId = Guid.NewGuid();
            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, "aaaaaa");
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);
            HttpResponseMessage httpRequestMessage = new HttpResponseMessage();
            httpRequestMessage.Content = new StreamContent(new System.IO.MemoryStream());

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();

            mockAttachFileInformationRepository.Setup(x => x.RegisterAttachFile(It.IsAny<AttachFileInformation>(), It.IsAny<NotAuthentication>())).Returns(httpRequestMessage);
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileStorageId(It.Is<VendorId>(s => s.Value == vendorId))).Returns(new AttachFileStorageId(attachStorageId.ToString()));

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            Dictionary<MetaKey, MetaValue> mockMeta = new Dictionary<MetaKey, MetaValue>();

            foreach (var meta in _metaList)
            {
                mockMeta.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }
            AttachFileInformation.Create(new FileName(_fileName), new Key(_key), new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm), new DrmType(_drmType), new DrmKey(_drmKey), new Meta(mockMeta));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AttachFileInformation_CreateFile_異常系_パラメータ異常_FileNameがNULL()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid().ToString();
            var attachStorageId = Guid.NewGuid();
            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, "aaaaaa");
            perRequestDataContainer.SetupProperty(x => x.VendorId, vendorId.ToString());
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);

            HttpResponseMessage httpRequestMessage = new HttpResponseMessage();
            httpRequestMessage.Content = new StreamContent(new System.IO.MemoryStream());

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();

            mockAttachFileInformationRepository.Setup(x => x.RegisterAttachFile(It.IsAny<AttachFileInformation>(), It.IsAny<NotAuthentication>())).Returns(httpRequestMessage);
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileStorageId(It.Is<VendorId>(s => s.Value == vendorId))).Returns(new AttachFileStorageId(attachStorageId.ToString()));

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            Dictionary<MetaKey, MetaValue> mockMeta = new Dictionary<MetaKey, MetaValue>();

            foreach (var meta in _metaList)
            {
                mockMeta.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }

            AttachFileInformation.Create(null, new Key(_key), new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm), new DrmType(_drmType), new DrmKey(_drmKey), new Meta(mockMeta));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_CreateFile_異常系_データ異常_attachFilestorageIdが取得できない場合()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid().ToString();
            var attachStorageId = Guid.NewGuid();
            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, "aaaaaa");
            perRequestDataContainer.SetupProperty(x => x.VendorId, vendorId.ToString());
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);
            HttpResponseMessage httpRequestMessage = new HttpResponseMessage();
            httpRequestMessage.Content = new StreamContent(new System.IO.MemoryStream());

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();

            mockAttachFileInformationRepository.Setup(x => x.RegisterAttachFile(It.IsAny<AttachFileInformation>(), It.IsAny<NotAuthentication>())).Returns(httpRequestMessage);
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileStorageId(It.Is<VendorId>(s => s.Value == vendorId))).Returns(new AttachFileStorageId(null));
            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            AttachFileInformation.Create(new FileName(_fileName), new Key(_key), new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm), new DrmType(_drmType), new DrmKey(_drmKey), new Meta(new Dictionary<MetaKey, MetaValue>()));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_CreateFile_異常系_登録失敗_DyanamicAPIでエラーを返却()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid().ToString();
            var attachStorageId = Guid.NewGuid();
            var notAuthentication = (new Random().NextDouble() >= 0.5);
            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, "aaaaaa");
            perRequestDataContainer.SetupProperty(x => x.VendorId, vendorId.ToString());
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);
            HttpResponseMessage httpRequestMessage = new HttpResponseMessage();
            httpRequestMessage.Content = new StreamContent(new System.IO.MemoryStream());

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();

            mockAttachFileInformationRepository.Setup(x => x.RegisterAttachFile(It.IsAny<AttachFileInformation>(), It.Is<NotAuthentication>(param => param.Value == notAuthentication))).Throws(new Exception());
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileStorageId(It.Is<VendorId>(s => s.Value == vendorId))).Returns(new AttachFileStorageId(attachStorageId.ToString()));
            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            var a = AttachFileInformation.Create(new FileName(_fileName), new Key(_key), new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm), new DrmType(_drmType), new DrmKey(_drmKey), new Meta(new Dictionary<MetaKey, MetaValue>()));
            a.Save(new NotAuthentication(notAuthentication));
        }

        /// GetFile
        /// 処理概要
        /// Blobからファイルの取得を行う
        [TestMethod]
        public void AttachFileInformation_GetFile_正常系()
        {
            base.TestInitialize(true);
            List<byte> test = new List<byte>() { 1, 2, 3 };

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.GetFileStream(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>())).Returns(new MemoryStream(test.ToArray()));
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId,
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));
            var result = testClass.GetFile();

            result.StatusCode.IsStructuralEqual(System.Net.HttpStatusCode.OK);
            result.Content.Headers.ContentType.ToString().IsStructuralEqual(_contentType);
            mockAttachFileRepository.Verify(x => x.GetFileStream(paramVendorId, parmFileId, parmFileName, parmAttachFileStorageId));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_GetFile_異常系_ファイル取得失敗()
        {
            base.TestInitialize(true);

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.GetFileStream(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>())).Throws(new Exception());
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId,
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));
            var result = testClass.GetFile();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_GetFile_異常系_ファイルなし()
        {
            base.TestInitialize(true);

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.GetFileStream(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>())).Returns(new MemoryStream());
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId,
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));
            var result = testClass.GetFile();
        }


        /// GetFile
        /// 処理概要
        /// Blobからファイルの取得を行う
        [TestMethod]
        public void AttachFileInformation_GetFileStream_正常系()
        {
            base.TestInitialize(true);
            List<byte> test = new List<byte>() { 1, 2, 3 };

            var mockResult = new MemoryStream(test.ToArray());
            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.GetFileStream(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>())).Returns(mockResult);
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId,
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));
            var result = testClass.GetFileStream();

            result.Value.IsSameReferenceAs(mockResult);
            mockAttachFileRepository.Verify(x => x.GetFileStream(paramVendorId, parmFileId, parmFileName, parmAttachFileStorageId));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_GetFileStream_異常系_ファイル取得失敗()
        {
            base.TestInitialize(true);

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.GetFileStream(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>())).Throws(new Exception());
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId,
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));
            var result = testClass.GetFileStream();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_GetFileStream_異常系_ファイルなし()
        {
            base.TestInitialize(true);

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.GetFileStream(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>())).Returns(new MemoryStream());
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId,
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));
            var result = testClass.GetFileStream();

        }

        /// Restore
        /// 処理概要
        /// 与えられた引数をもとにAttachFileの復元を行う
        /// IDが指定された場合はDyanamicAPIで対象のデータを取得しAttachFileを復元する。
        /// 復元するデータにKeyの設定がされていた場合はKeyのチェックを行う。
        [TestMethod]
        public void AttachFileInformation_Restore_正常系()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid().ToString();
            Dictionary<MetaKey, MetaValue> mockMeta = new Dictionary<MetaKey, MetaValue>();

            foreach (var meta in _metaList)
            {
                mockMeta.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }

            var result = AttachFileInformation.Restore(new FileId(Guid.Parse(_fileId)), new FileName(_fileName),
                new BlobUrl(_blobUrl), new AttachFileStorageId(_attachFilestorageId), new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), new VendorId(vendorId), new Meta(mockMeta), new IsUploaded(true));

            result.FileId.Value.IsStructuralEqual(_fileId);
            result.FileName.Value.IsStructuralEqual(_fileName);
            result.BlobUrl.Value.IsStructuralEqual(_blobUrl);
            result.AttachFileStorageId.Value.IsStructuralEqual(_attachFilestorageId);
            result.Key.Value.IsStructuralEqual(_key);
            result.FileLength.Value.IsStructuralEqual(_fileLength);
            result.ContentType.Value.IsStructuralEqual(_contentType);
            result.IsDrm.Value.IsStructuralEqual(_isDrm);
            result.DrmType.Value.IsStructuralEqual(_drmType);
            result.DrmKey.Value.IsStructuralEqual(_drmKey);
            result.VendorId.Value.IsStructuralEqual(vendorId);
            //result.Meta.IsStructuralEqual(mockMeta);
            foreach (var meta in result.Meta)
            {
                var motoMeta = mockMeta[meta.Key];
                meta.Value.Value.IsStructuralEqual(motoMeta.Value);
            }
        }

        [TestMethod]
        public void AttachFileInformation_Restore_正常系_ID指定()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid().ToString();
            var attachStorageId = Guid.NewGuid();
            var notAuthentication = (new Random().NextDouble() >= 0.5);

            Dictionary<MetaKey, MetaValue> mockMeta = new Dictionary<MetaKey, MetaValue>();

            foreach (var meta in _metaList)
            {
                mockMeta.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }

            var mockDC = new Mock<IAttachFileInformationRepository>();
            var mockData = AttachFileInformation.Restore(new FileId(Guid.Parse(_fileId)), new FileName(_fileName),
                new BlobUrl(_blobUrl), new AttachFileStorageId(_attachFilestorageId), new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), new VendorId(vendorId), new Meta(mockMeta), new IsUploaded(true));
            mockDC.Setup(x => x.GetAttachFileInformation(It.Is<FileId>(s => s.Value == _fileId), It.Is<NotAuthentication>(param => param.Value == notAuthentication))).Returns(mockData);

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockDC.Object);

            var result = AttachFileInformation.Restore(new FileId(Guid.Parse(_fileId)), new NotAuthentication(notAuthentication));

            result.FileId.Value.IsStructuralEqual(_fileId);
            result.FileName.Value.IsStructuralEqual(_fileName);
            result.BlobUrl.Value.IsStructuralEqual(_blobUrl);
            result.AttachFileStorageId.Value.IsStructuralEqual(_attachFilestorageId);
            result.Key.Value.IsStructuralEqual(_key);
            result.FileLength.Value.IsStructuralEqual(_fileLength);
            result.ContentType.Value.IsStructuralEqual(_contentType);
            result.IsDrm.Value.IsStructuralEqual(_isDrm);
            result.DrmType.Value.IsStructuralEqual(_drmType);
            result.DrmKey.Value.IsStructuralEqual(_drmKey);
            result.VendorId.Value.IsStructuralEqual(vendorId);
            foreach (var meta in result.Meta)
            {
                var motoMeta = mockMeta[meta.Key];
                meta.Value.Value.IsStructuralEqual(motoMeta.Value);

            }
        }

        [TestMethod]
        public void AttachFileInformation_Restore_正常系_ID指定_Kye一致()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid().ToString();
            var attachStorageId = Guid.NewGuid();
            var notAuthentication = (new Random().NextDouble() >= 0.5);

            Dictionary<MetaKey, MetaValue> mockMeta = new Dictionary<MetaKey, MetaValue>();

            foreach (var meta in _metaList)
            {
                mockMeta.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }

            var mockDC = new Mock<IAttachFileInformationRepository>();
            var mockData = AttachFileInformation.Restore(new FileId(Guid.Parse(_fileId)), new FileName(_fileName),
                new BlobUrl(_blobUrl), new AttachFileStorageId(_attachFilestorageId), new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), new VendorId(vendorId), new Meta(mockMeta), new IsUploaded(true));
            mockDC.Setup(x => x.GetAttachFileInformation(It.Is<FileId>(s => s.Value == _fileId), It.Is<NotAuthentication>(param => param.Value == notAuthentication))).Returns(mockData);

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockDC.Object);

            var result = AttachFileInformation.Restore(new FileId(Guid.Parse(_fileId)), new Key(_key), new NotAuthentication(notAuthentication));

            result.FileId.Value.IsStructuralEqual(_fileId);
            result.FileName.Value.IsStructuralEqual(_fileName);
            result.BlobUrl.Value.IsStructuralEqual(_blobUrl);
            result.AttachFileStorageId.Value.IsStructuralEqual(_attachFilestorageId);
            result.Key.Value.IsStructuralEqual(_key);
            result.FileLength.Value.IsStructuralEqual(_fileLength);
            result.ContentType.Value.IsStructuralEqual(_contentType);
            result.IsDrm.Value.IsStructuralEqual(_isDrm);
            result.DrmType.Value.IsStructuralEqual(_drmType);
            result.DrmKey.Value.IsStructuralEqual(_drmKey);
            result.VendorId.Value.IsStructuralEqual(vendorId);
            foreach (var meta in result.Meta)
            {
                var motoMeta = mockMeta[meta.Key];
                meta.Value.Value.IsStructuralEqual(motoMeta.Value);

            }
        }

        [TestMethod]
        public void AttachFileInformation_Restore_正常系_ID指定_Kye不一致()
        {
            base.TestInitialize(true);

            var vendorId = Guid.NewGuid();
            var attachStorageId = Guid.NewGuid();
            var notAuthentication = (new Random().NextDouble() >= 0.5);

            Dictionary<MetaKey, MetaValue> mockMeta = new Dictionary<MetaKey, MetaValue>();

            foreach (var meta in _metaList)
            {
                mockMeta.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }

            var mockDC = new Mock<IAttachFileInformationRepository>();
            var mockData = AttachFileInformation.Restore(new FileId(Guid.Parse(_fileId)), new FileName(_fileName),
                new BlobUrl(_blobUrl), new AttachFileStorageId(_attachFilestorageId), new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), new VendorId(vendorId), new Meta(mockMeta), new IsUploaded(true));
            mockDC.Setup(x => x.GetAttachFileInformation(It.Is<FileId>(s => s.Value == _fileId), It.Is<NotAuthentication>(param => param.Value == notAuthentication))).Returns(mockData);

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockDC.Object);

            ApiException result = new ApiException(null);
            try
            {
                AttachFileInformation.Restore(new FileId(Guid.Parse(_fileId)), new Key(""), new NotAuthentication(notAuthentication));
            }
            catch (ApiException ex)
            {
                result = ex;
            }
            result.HttpResponseMessage.StatusCode.IsStructuralEqual(System.Net.HttpStatusCode.Forbidden);
        }

        /// Upload
        /// 処理概要
        /// 与えられた引数をもとにファイルのアップロードを行う
        /// isEndStream=Falseの場合はローカルストレージにファイル追加保存する
        /// isEndStream=Trueの場合はローカルストレージ追加後Blobにアップロードする
        [TestMethod]
        public void AttachFileInformation_Upload_正常系_ローカルストレージにアップロード()
        {
            base.TestInitialize(true);

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.TempFileUpload(It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<InputStream>(), It.IsAny<IsAppendStream>(), It.IsAny<AppendPosition>())).Returns(new FilePath("aa"));
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmInputStream = new InputStream(new MemoryStream());
            var parmIsEndStream = new IsEndStream(false);
            var parmIsAppendStream = new IsAppendStream(false);
            var parmAppendPosition = new AppendPosition(0);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), new AttachFileStorageId(_attachFilestorageId), new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), new VendorId(Guid.Parse(_vendorId)),
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));

            testClass.Upload(parmInputStream, parmIsEndStream, parmIsAppendStream, parmAppendPosition);

            mockAttachFileRepository.Verify(x => x.TempFileUpload(parmFileId, parmFileName, parmInputStream, parmIsAppendStream, parmAppendPosition));
            mockAttachFileRepository.Verify(x => x.Upload(It.IsAny<FilePath>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<VendorId>(), It.IsAny<ContentType>(), It.IsAny<AttachFileStorageId>()), Times.Never);
        }

        [TestMethod]
        public void AttachFileInformation_Upload_正常系_Blobにアップロード()
        {
            base.TestInitialize(true);
            var localPath = new FilePath("aaa");

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.TempFileUpload(It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<InputStream>(), It.IsAny<IsAppendStream>(), It.IsAny<AppendPosition>())).Returns(localPath);
            mockAttachFileRepository.Setup(x => x.Upload(It.IsAny<FilePath>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<VendorId>(), It.IsAny<ContentType>(), It.IsAny<AttachFileStorageId>()));

            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            HttpResponseMessage httpRequestMessage = new HttpResponseMessage();
            httpRequestMessage.Content = new StreamContent(new System.IO.MemoryStream());
            var mockDC = new Mock<IAttachFileInformationRepository>();
            //var mockData = AttachFileInformation.Restore(new FileId(Guid.Parse(fileId)), new FileName(fileName),
            //    new BlobUrl(blobUrl), new AttachFileStorageId(attachFilestorageId), new Key(key),
            //    new ContentType(contentType), new FileLength(fileLength), new IsDrm(isDrm),
            //    new DrmType(drmType), new DrmKey(drmKey), new VendorId(vendorId), new Meta(mockMeta), new IsUploaded(true));
            mockDC.Setup(x => x.RegisterAttachFile(It.IsAny<AttachFileInformation>(), It.IsAny<NotAuthentication>())).Returns(httpRequestMessage);

            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockDC.Object);

            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmInputStream = new InputStream(new MemoryStream());
            var parmIsEndStream = new IsEndStream(true);
            var parmIsAppendStream = new IsAppendStream(false);
            var parmAppendPosition = new AppendPosition(0);
            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);
            var parmContentType = new ContentType(_contentType);
            var parmBlobUrl = new BlobUrl(_contentType);


            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                parmBlobUrl, parmAttachFileStorageId, new Key(_key),
                parmContentType, new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId,
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(false));

            testClass.Upload(parmInputStream, parmIsEndStream, parmIsAppendStream, parmAppendPosition);

            mockAttachFileRepository.Verify(x => x.TempFileUpload(parmFileId, parmFileName, parmInputStream, parmIsAppendStream, parmAppendPosition));
            mockAttachFileRepository.Verify(x => x.Upload(localPath, parmFileId, parmFileName, paramVendorId, parmContentType, parmAttachFileStorageId));

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_Upload_異常系_ローカルストレージにアップロード失敗()
        {
            base.TestInitialize(true);

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.TempFileUpload(It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<InputStream>(), It.IsAny<IsAppendStream>(), It.IsAny<AppendPosition>())).Throws(new Exception());
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmInputStream = new InputStream(new MemoryStream());
            var parmIsEndStream = new IsEndStream(false);
            var parmIsAppendStream = new IsAppendStream(false);
            var parmAppendPosition = new AppendPosition(0);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), new AttachFileStorageId(_attachFilestorageId), new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), new VendorId(Guid.Parse(_vendorId)),
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));

            testClass.Upload(parmInputStream, parmIsEndStream, parmIsAppendStream, parmAppendPosition);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_Upload_異常系_Blobにアップロード失敗()
        {
            base.TestInitialize(true);
            var localPath = new FilePath("aaa");

            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.TempFileUpload(It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<InputStream>(), It.IsAny<IsAppendStream>(), It.IsAny<AppendPosition>())).Returns(localPath);
            mockAttachFileRepository.Setup(x => x.Upload(
                It.IsAny<FilePath>(), It.IsAny<FileId>(), It.IsAny<FileName>(),
                It.IsAny<VendorId>(), It.IsAny<ContentType>(), It.IsAny<AttachFileStorageId>())).Throws(new Exception());
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmInputStream = new InputStream(new MemoryStream());
            var parmIsEndStream = new IsEndStream(true);
            var parmIsAppendStream = new IsAppendStream(false);
            var parmAppendPosition = new AppendPosition(0);
            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);
            var parmContentType = new ContentType(_contentType);
            var parmBlobUrl = new BlobUrl(_contentType);


            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                parmBlobUrl, parmAttachFileStorageId, new Key(_key),
                parmContentType, new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId,
                new Meta(new Dictionary<MetaKey, MetaValue>()), new IsUploaded(true));

            testClass.Upload(parmInputStream, parmIsEndStream, parmIsAppendStream, parmAppendPosition);

        }

        /// Delete
        /// 処理概要
        /// CosmosDBとBlobからAttachFileの削除を行う。
        [TestMethod]
        public void AttachFileInformation_Delete_正常系()
        {
            base.TestInitialize(true);

            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmInputStream = new InputStream(new MemoryStream());
            var parmIsEndStream = new IsEndStream(true);
            var parmIsAppendStream = new IsAppendStream(false);
            var parmAppendPosition = new AppendPosition(0);
            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);
            var parmContentType = new ContentType(_contentType);
            var parmBlobUrl = new BlobUrl(_contentType);
            var parmNotAuthentication = new NotAuthentication(new Random().NextDouble() >= 0.5);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId, null, new IsUploaded(true));


            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.DeleteFile(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>()));
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.DeleteAttachFile(It.Is<FileId>(s => s.Value == _fileId), It.Is<NotAuthentication>(s => s.Value == parmNotAuthentication.Value))).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);


            var result = testClass.DeleteFile(parmNotAuthentication);
            mockAttachFileInformationRepository.Verify(x => x.DeleteAttachFile(parmFileId, parmNotAuthentication));
            mockAttachFileRepository.Verify(x => x.DeleteFile(paramVendorId, parmFileId, parmFileName, parmAttachFileStorageId));
            result.StatusCode.IsStructuralEqual(System.Net.HttpStatusCode.OK);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_Delete_異常系_CosmosDB削除失敗()
        {
            base.TestInitialize(true);

            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmInputStream = new InputStream(new MemoryStream());
            var parmIsEndStream = new IsEndStream(true);
            var parmIsAppendStream = new IsAppendStream(false);
            var parmAppendPosition = new AppendPosition(0);
            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);
            var parmContentType = new ContentType(_contentType);
            var parmBlobUrl = new BlobUrl(_contentType);
            var parmNotAuthentication = new NotAuthentication(new Random().NextDouble() >= 0.5);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId, null, new IsUploaded(true));


            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.DeleteAttachFile(It.Is<FileId>(s => s.Value == _fileId), It.Is<NotAuthentication>(s => s.Value == parmNotAuthentication.Value))).Throws(new Exception());
            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);

            var result = testClass.DeleteFile(parmNotAuthentication);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_Delete_異常系_Blob削除失敗()
        {
            base.TestInitialize(true);

            var parmFileId = new FileId(Guid.Parse(_fileId));
            var parmFileName = new FileName(_fileName);
            var parmInputStream = new InputStream(new MemoryStream());
            var parmIsEndStream = new IsEndStream(true);
            var parmIsAppendStream = new IsAppendStream(false);
            var parmAppendPosition = new AppendPosition(0);
            var paramVendorId = new VendorId(Guid.Parse(_vendorId));
            var parmAttachFileStorageId = new AttachFileStorageId(_attachFilestorageId);
            var parmContentType = new ContentType(_contentType);
            var parmBlobUrl = new BlobUrl(_contentType);
            var parmNotAuthentication = new NotAuthentication(new Random().NextDouble() >= 0.5);

            var testClass = AttachFileInformation.Restore(parmFileId, parmFileName,
                new BlobUrl(_blobUrl), parmAttachFileStorageId, new Key(_key),
                new ContentType(_contentType), new FileLength(_fileLength), new IsDrm(_isDrm),
                new DrmType(_drmType), new DrmKey(_drmKey), paramVendorId, null, new IsUploaded(true));


            var mockAttachFileRepository = new Mock<IAttachFileRepository>();
            mockAttachFileRepository.Setup(x => x.DeleteFile(It.IsAny<VendorId>(), It.IsAny<FileId>(), It.IsAny<FileName>(), It.IsAny<AttachFileStorageId>())).Throws(new Exception());
            UnityContainer.RegisterInstance<IAttachFileRepository>(mockAttachFileRepository.Object);

            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.DeleteAttachFile(It.Is<FileId>(s => s.Value == _fileId), It.Is<NotAuthentication>(s => s.Value == parmNotAuthentication.Value))).Returns(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);

            var result = testClass.DeleteFile(parmNotAuthentication);
        }

        /// SearchByMeta
        /// 処理概要
        /// 引数のメタの情報からAttachFileを検索してAttachFileを復元する
        /// 検索処理はOtherDomainを経由してDyanamicAPIで行う
        [TestMethod]

        public void AttachFileInformation_SearchByMeta_正常系()
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
            var dresult = AttachFileInformation.SearchByMeta(meta);
            mockAttachFileInformationRepository.Verify(x => x.GetAttachFileInformationSearchByMeta(meta));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileInformation_SearchByMeta_異常系_DynamicAPIエラー()
        {
            base.TestInitialize(true);

            var metaKey = new MetaKey("aa");
            var metaValue = new MetaValue("bb");
            var d = new Dictionary<MetaKey, MetaValue>();
            d.Add(metaKey, metaValue);
            var meta = new Meta(d);
            var mockAttachFileInformationRepository = new Mock<IAttachFileInformationRepository>();
            mockAttachFileInformationRepository.Setup(x => x.GetAttachFileInformationSearchByMeta(It.IsAny<Meta>())).Throws(new Exception());
            UnityContainer.RegisterInstance<IAttachFileInformationRepository>(mockAttachFileInformationRepository.Object);
            var result = AttachFileInformation.SearchByMeta(meta);
        }
    }
}
