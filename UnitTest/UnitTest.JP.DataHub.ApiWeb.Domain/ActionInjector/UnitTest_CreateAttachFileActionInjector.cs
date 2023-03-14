using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.HashAlgorithm;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ActionInjector;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ActionInjector
{
    [TestClass]
    public class UnitTest_CreateAttachFileActionInjector : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private Mock<IExternalAttachFileRepository> _mockExternalAttachFileRepository;


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");

            //UploadOK/BlockList
            UnityContainer.RegisterInstance<bool>("IsEnableUploadContentCheck", false);
            UnityContainer.RegisterInstance<bool>("IsPriorityHigh_OKList", false);
            UnityContainer.RegisterInstance<bool>("IsUploadOk_NoExtensionFile", true);

            string okContentTypeList = null;
            string okExtensionList = null;
            string blockContentTypeList = null;
            string blockExtensionList = null;
            UnityContainer.RegisterInstance<List<string>>("UploadOK_ContentTypeList", GetListFromConfigString(okContentTypeList));
            UnityContainer.RegisterInstance<List<string>>("UploadOK_ExtensionList", GetListFromConfigString(okExtensionList));
            UnityContainer.RegisterInstance<List<string>>("BlockContentTypeList", GetListFromConfigString(blockContentTypeList));
            UnityContainer.RegisterInstance<List<string>>("BlockExtensionList", GetListFromConfigString(blockExtensionList));
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<IDataContainer>(new PerRequestDataContainer());

            var mockRepository = new Mock<IDynamicApiRepository>();
            mockRepository.Setup(x => x.GetSchemaModelById(It.IsAny<DataSchemaId>())).Returns(new DataSchema(""));
            UnityContainer.RegisterInstance(mockRepository.Object);
        }

        private static List<string> GetListFromConfigString(string configValue)
        {
            var ret = new List<string>();
            if (configValue != null)
            {
                var cfg = configValue.Split(',');
                foreach (var c1 in cfg)
                {
                    var c = c1.Replace(" ", "");
                    c = c.ToLower();
                    ret.Add(c);
                }
            }
            return ret;
        }

        [TestMethod]
        public void Execute_正常()
        {
            var vendorId = Guid.NewGuid().ToString();
            var fileName = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = vendorId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction(null, fileName);

            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });

            var contents = JToken.Parse(action.Contents.ReadToString());
            var regFileId = contents.Value<string>("FileId");
            var regPath = contents.Value<string>("FilePath");
            regPath.Is($"{vendorId}/{regFileId}/{HashCalculation.ComputeHashString(Encoding.Unicode.GetBytes(fileName), HashAlgorithmType.Sha256)}");

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.Created);
            var resultJtoken = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            (resultJtoken.Value<string>("FileId")).Is(regFileId);
        }

        [TestMethod]
        public void Execute_正常_Vendorなし()
        {
            var vendorId = Configuration.GetValue<string>("AppConfig:VendorSystemAuthenticationDefaultVendorId");
            var fileName = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = null;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction(null, fileName);

            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });

            var contents = JToken.Parse(action.Contents.ReadToString());
            var regFileId = contents.Value<string>("FileId");
            var regPath = contents.Value<string>("FilePath");
            regPath.Is($"{vendorId}/{regFileId}/{HashCalculation.ComputeHashString(Encoding.Unicode.GetBytes(fileName), HashAlgorithmType.Sha256)}");

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.Created);
            var resultJtoken = JToken.Parse(result.Content.ReadAsStringAsync().Result);

            (resultJtoken.Value<string>("FileId")).Is(regFileId);
        }

        [TestMethod]
        public void Execute_正常_外部ファイル()
        {
            var vendorId = Guid.NewGuid().ToString();
            var fileName = Guid.NewGuid().ToString();
            var dataSourceType = ExternalAttachFileInfomation.SupportedDataSourceTypes[0];
            var credentials = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var filePath = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = vendorId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var inputContents = GetInfoJson(null, fileName).ToJson();
            inputContents[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            inputContents[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)] = new ExternalAttachFileInfomation(dataSourceType, credentials, filePath).ToJson();
            var action = CreateRegistDataActionWithContents(null, fileName, inputContents.ToString());

            SetupExternalAttachFile(dataSourceType, true);
            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });

            var regContents = JToken.Parse(action.Contents.ReadToString());
            var regFileId = regContents.Value<string>(nameof(DynamicApiAttachFileInformation.FileId));
            regContents.Value<string>(nameof(DynamicApiAttachFileInformation.FilePath)).IsNull();

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.Created);
            var resultJtoken = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            (resultJtoken.Value<string>("FileId")).Is(regFileId);
        }

        [TestMethod]
        public void Execute_異常_Json違反()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction();
            action.Contents = new Contents("hogehoge");
            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });
            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void Execute_異常_外部ファイル_バリデーションエラー()
        {
            var vendorId = Guid.NewGuid().ToString();
            var fileName = Guid.NewGuid().ToString();
            var dataSourceType = ExternalAttachFileInfomation.SupportedDataSourceTypes[0];
            var credentials = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var filePath = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = vendorId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var inputContents = GetInfoJson(null, fileName).ToJson();
            inputContents[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            inputContents[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)] = new ExternalAttachFileInfomation(dataSourceType, credentials, filePath).ToJson();
            var action = CreateRegistDataActionWithContents(null, fileName, inputContents.ToString());

            var errorCode = ErrorCodeMessage.Code.E20414;
            SetupExternalAttachFile(dataSourceType, false, errorCode);
            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var resultJtoken = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            resultJtoken["error_code"].Value<string>().Is(errorCode.ToString());

            ErrorCodeMessage.Code? code;
            _mockExternalAttachFileRepository.Verify(x => x.Validate(It.IsAny<ExternalAttachFileInfomation>(), out code), Times.Once);
        }

        [TestMethod]
        public void Execute_異常_外部ファイル_ファイル情報なし()
        {
            var vendorId = Guid.NewGuid().ToString();
            var fileName = Guid.NewGuid().ToString();
            var dataSourceType = ExternalAttachFileInfomation.SupportedDataSourceTypes[0];
            var credentials = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var filePath = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = vendorId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var inputContents = GetInfoJson(null, fileName).ToJson();
            inputContents[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            var action = CreateRegistDataActionWithContents(null, fileName, inputContents.ToString());

            SetupExternalAttachFile(dataSourceType, true);
            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var resultJtoken = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            resultJtoken["error_code"].Value<string>().Is(ErrorCodeMessage.Code.E20413.ToString());
        }

        [TestMethod]
        public void Execute_異常_外部ファイル_データソース種別不正()
        {
            var vendorId = Guid.NewGuid().ToString();
            var fileName = Guid.NewGuid().ToString();
            var dataSourceType = Guid.NewGuid().ToString();
            var credentials = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var filePath = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = vendorId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var inputContents = GetInfoJson(null, fileName).ToJson();
            inputContents[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            inputContents[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)] = new ExternalAttachFileInfomation(dataSourceType, credentials, filePath).ToJson();
            var action = CreateRegistDataActionWithContents(null, fileName, inputContents.ToString());

            SetupExternalAttachFile(dataSourceType, true);
            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var resultJtoken = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            resultJtoken["error_code"].Value<string>().Is(ErrorCodeMessage.Code.E20414.ToString());

            ErrorCodeMessage.Code? code;
            _mockExternalAttachFileRepository.Verify(x => x.Validate(It.IsAny<ExternalAttachFileInfomation>(), out code), Times.Never);
        }

        [TestMethod]
        public void Execute_異常_外部ファイル_履歴ON()
        {
            var vendorId = Guid.NewGuid().ToString();
            var fileName = Guid.NewGuid().ToString();
            var dataSourceType = ExternalAttachFileInfomation.SupportedDataSourceTypes[0];
            var credentials = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var filePath = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = vendorId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var inputContents = GetInfoJson(null, fileName).ToJson();
            inputContents[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            inputContents[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)] = new ExternalAttachFileInfomation(dataSourceType, credentials, filePath).ToJson();
            var action = CreateRegistDataActionWithContents(null, fileName, inputContents.ToString());
            action.IsDocumentHistory = new IsDocumentHistory(true);

            SetupExternalAttachFile(dataSourceType, true);
            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var resultJtoken = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            resultJtoken["error_code"].Value<string>().Is(ErrorCodeMessage.Code.E20418.ToString());
        }

        [TestMethod]
        public void Execute_異常_ファイル名不正()
        {
            var vendorId = Guid.NewGuid().ToString();
            var credentials = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var filePath = Guid.NewGuid().ToString();

            var invalidChars = Path.GetInvalidFileNameChars();
            var fileName = $"hoge{invalidChars[0]}.txt";

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = vendorId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction(null, fileName);
            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var resultJtoken = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            resultJtoken["error_code"].Value<string>().Is(ErrorCodeMessage.Code.E20417.ToString());
        }

        [TestMethod]
        public void ExecuteAction_異常_設定無効()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });
            var contents = JToken.Parse(action.Contents.ReadToString());
            var regFileId = contents.Value<string>("FileId");
            var regPath = contents.Value<string>("FilePath");
            var result = target.ReturnValue as HttpResponseMessage;

            result.StatusCode.Is(HttpStatusCode.NotImplemented);
        }


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private static string FileId = "D261DF4B-0305-46AD-9FF5-12026BEE3F89";

        private RegistAction CreateRegistDataAction(string contenttype = null, string filename = null)
        {
            return CreateRegistDataActionWithContents(contenttype, filename, GetInfoJson(contenttype: contenttype, filename: filename));
        }

        private RegistAction CreateRegistDataActionWithContents(string contenttype = null, string filename = null, string contents = null)
        {
            RegistAction action = UnityCore.Resolve<RegistAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{FileId}");
            action.ActionType = new ActionTypeVO(ActionType.Regist);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.RequestSchema = new DataSchema(null);
            action.Contents = new Contents(contents);
            action.IsEnableAttachFile = new IsEnableAttachFile(true);

            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>())).Returns(new RegisterOnceResult(""));

            return action;
        }

        private string GetInfoJson(string key = null, string contenttype = null, string filename = null)
        {
            DynamicApiAttachFileInformation dynamicApiAttachFileInformation = new DynamicApiAttachFileInformation(FileId, filename == null ? "filename" : filename, contenttype == null ? "application/octet-stream" : contenttype, 100, "hoge/fuga/piyo.jpeg", key, false, null, null, true);

            var json = JToken.Parse(JsonConvert.SerializeObject(dynamicApiAttachFileInformation));
            if (json["FileName"].ToString() == "REMOVE")
            {
                json = json.RemoveField("FileName");
            }
            else if (json["FileName"].ToString() == "NULL")
            {
                json["FileName"] = null;
            }
            else if (json["FileName"].ToString() == "EMPTY")
            {
                json["FileName"] = string.Empty;
            }
            if (json["ContentType"].ToString() == "REMOVE")
            {
                json = json.RemoveField("ContentType");
            }
            else if (json["ContentType"].ToString() == "NULL")
            {
                json["ContentType"] = null;
            }
            else if (json["ContentType"].ToString() == "EMPTY")
            {
                json["ContentType"] = string.Empty;
            }

            var fi = json.SelectToken("FileId");
            fi.Parent.Remove();
            //json["FileId"] = FileId;
            return json.ToString();
        }

        private void SetupExternalAttachFile(string dataSourceType, bool isValid, ErrorCodeMessage.Code? code = null)
        {
            _mockExternalAttachFileRepository = new Mock<IExternalAttachFileRepository>();
            _mockExternalAttachFileRepository.Setup(x => x.Validate(It.IsAny<ExternalAttachFileInfomation>(), out code)).Returns(isValid);
            UnityContainer.RegisterInstance(dataSourceType, _mockExternalAttachFileRepository.Object);
        }



        public static object[] OKリストで_ブロック_OKチェック_コンテントタイプ = new[]
        {
            //大文字小文字混在
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "Text/Html", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/plane", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "TexT/PLane", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "application/PdF", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/Json",  UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //ワイルドカード効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/Jsoooooon", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JS*", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //中間一致しないか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/JsonHoge", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //正規表現効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/pde", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/pda", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
        };

        public static object[] OKリストで_ブロック_OK_チェック_拡張子 = new[]
        {
            //大文字小文字混在
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.txt", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.TxT", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.pdf", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.PDf", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.JPg", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.JPEg", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //ワイルドカード効いているか
             new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.vbeeeeeeees", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG,vb*", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //正規表現効いているか
             new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.vbs", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG,vb[s|e]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
             new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.vbe", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG,vb[s|e]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //拡張子無し：拡張子無しブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //拡張子無し：拡張子無しOK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = ".", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //拡張子無し：ピリオド多数
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.fuga.hogehoge.fuga.pdf", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //拡張子無し：ピリオド多数:NGのやつ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.fuga.hogehoge.fuga.exe.pdf.exe", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //中間一致しないか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.hogePnGhoge", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.pdfa", UploadOK_ExtensionList = "PdF,JPeg,jPG,PNG", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
       };

        public static object[] OKリストで_ブロックOKチェック_コンテントタイプと拡張子組み合わせ = new[]
        {
            //OKのコンテントタイプリスト、拡張子リストは、正規表現を入れる
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.hogegegefuga", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k]", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,*piyo", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.fugapiyo", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k]", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,*piyo", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/pnk", RequestFileName = "hoge.jpe", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k]", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,*piyo", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //コンテントタイプがOKで、拡張子NG
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.ho", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k]", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,*piyo", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがNGで、拡張子OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/hoge", RequestFileName = "hoge.jpg", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k]", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,*piyo", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがOKで、拡張子無し：ブロック → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k]", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,*piyo", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがNGで、拡張子無し：ブロック → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/hoge", RequestFileName = "hoge", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k]", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,*piyo", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがOKで、拡張子無し：OK → OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k]", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,*piyo", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
        };

        public static object[] ブロックリストで_ブロック_OKチェック_コンテントタイプ = new[]
        {
            //大文字小文字混在
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "Text/javascRIPT", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //OKのやつ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/plane", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "TexT/PLane", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //ワイルドカード効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "Text/javascRIPThoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //正規表現効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "application/pdE", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "application/pDa", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //中間一致しないか確認
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTmlA", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
        };

        public static object[] ブロックリストで_ブロック_OKチェック_拡張子 = new[]
        {
            //大文字小文字混在
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.eXe", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //OKのもの
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.PdF", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //正規表現効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.vbS", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.vBe", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.cooOOooom", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //拡張子無し:拡張子無しはどうするか:false ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = ".", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //拡張子無し:拡張子無しはどうするか:true OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //ピリオド多数
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.fuga.hogehoge.fuga.exe", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //ピリオド多数:OKのやつ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.fuga.hogehoge.fuga.exe.pdf", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //中間一致しないか確認
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", RequestFileName = "hoge.HogeEXeHoge", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
        };

        public static object[] ブロックリストでブロック_OKチェック_コンテントタイプと拡張子組み合わせ = new[]
        {
            //ブロックしないやつ:OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.jpg", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //コンテントタイプjpeg(ブロックしない)と、ブロックするファイルタイプ:ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.exe", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプhtml(ブロックする)と、ブロックしないファイルタイプ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "TexT/HTmL", RequestFileName = "hoge.jpEg", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプhtml(ブロックする)と、ブロックするファイルタイプ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "TExt/HtMl", RequestFileName = "hoge.Exe", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックしないやつで、拡張子無し：NG → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックで、拡張子無し：NG → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/JavaScripT", RequestFileName = "hoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックで、拡張子無し：OK → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/JavaScripT", RequestFileName = "hoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックしないやつで、拡張子無し：OK → OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
        };

        public static object[] OKリストとブロックリスト_コンテントタイプと拡張子組み合わせ = new[]
        {
            //OKリスト、ブロックリストには同じものを入れる(text/plane, txt) フラグで変わるかチェック
            //OKリストにしかないもの
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.jpg", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //ブロックリストにしかない
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.exe", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがOKリストにあって、拡張子がブロックリストにある → ブロックにあるので、ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.exe", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックリストにあって、拡張子がOKリストにある → ブロックにあるので、ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", RequestFileName = "hoge.jpg", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがOK、ブロックどちらにもある、拡張子がOKにある :  OKリスト優先 → OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/plane", RequestFileName = "hoge.jpg", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = true, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //コンテントタイプがOK、ブロックどちらにもある、ブロックリスト優先 :  ブロックリスト優先 → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/plane", RequestFileName = "hoge.jpg", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411" } },
            //拡張子がOK、ブロックどちらにもある、コンテントタイプがOKにある : OKリスト優先 → OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "application/pdf", RequestFileName = "hoge.txt", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = true, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //拡張子がOK、ブロックどちらにもある、コンテントタイプがOKにある : ブロックリスト優先 → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "application/pdf", RequestFileName = "hoge.txt", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411" } },
            //コンテントタイプがOKリストに無く、拡張子がブロックリストにもない : ブロックリスト優先 → ブロックリストに無いから、OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/hoge", RequestFileName = "hoge.hoge", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //コンテントタイプがOKリストに無く、拡張子がブロックリストにもない : OKリスト優先 → OKリストに無いから、ブロック（OKされていない）
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/hoge", RequestFileName = "hoge.hoge", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = true, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
        };

        public static object[] このコンテントタイプは基本OKだけどこの拡張子だけはダメ = new[]
        {
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.jpg", UploadOK_ContentTypeList = "image/*", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.exe", UploadOK_ContentTypeList = "image/*", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411" } },
        };

        public static object[] このコンテントタイプは基本NGだけどこの拡張子だけはOK = new[]
        {
            //基本、ブロックの方が強いので、この場合は、OKListを優先にする
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.jpg", UploadBlock_ContentTypeList = "image/*", UploadOK_ExtensionList = "jpg", IsPriorityHigh_OKList = true, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created}},
            //ブロックリスト優先にすると、ブロックされる
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.jpg", UploadBlock_ContentTypeList = "image/*", UploadOK_ExtensionList = "jpg", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //これは元からダメ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.exe", UploadBlock_ContentTypeList = "image/*", UploadOK_ExtensionList = "jpg", IsPriorityHigh_OKList = true, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
        };

        public static object[] FileNameとContentTypeが項目ごと無い_項目null_空値 = new[]
        {
            //チェックしないだけなので、後続に流れる（UTだとMockでOKになるが、ITだと、JsonValidationが掛かってBadRequestになる）
            //REMOVEとすると、項目消される。
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "REMOVE", RequestFileName = "REMOVE", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created } },
            //NULL は、'項目':null
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "NULL", RequestFileName = "NULL", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created } },
            //EMPTY は、'項目':''
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "EMPTY", RequestFileName = "EMPTY", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadOK_ExtensionList = "PdF,JP[e|g],PNG,hoge.*.fuga,txt", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m,txt", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.Created } },
        };


        [TestMethod]
        [TestCaseSource(nameof(OKリストで_ブロック_OKチェック_コンテントタイプ))]
        [TestCaseSource(nameof(OKリストで_ブロック_OK_チェック_拡張子))]
        [TestCaseSource(nameof(OKリストで_ブロックOKチェック_コンテントタイプと拡張子組み合わせ))]
        [TestCaseSource(nameof(ブロックリストで_ブロック_OKチェック_コンテントタイプ))]
        [TestCaseSource(nameof(ブロックリストで_ブロック_OKチェック_拡張子))]
        [TestCaseSource(nameof(ブロックリストでブロック_OKチェック_コンテントタイプと拡張子組み合わせ))]
        [TestCaseSource(nameof(OKリストとブロックリスト_コンテントタイプと拡張子組み合わせ))]
        [TestCaseSource(nameof(このコンテントタイプは基本OKだけどこの拡張子だけはダメ))]
        public void CreateAttachFile_正常系_コンテントOKリスト_ブロックリストによる制限のチェック()
        {
            TestContext.Run((AttachFileOKBlockTestData testSource) => TestExecute(testSource));
        }

        [TestMethod]
        [TestCaseSource(nameof(FileNameとContentTypeが項目ごと無い_項目null_空値))]
        public void CreateAttachFile_異常系_FileNameとContentTypeが項目ごと無い_項目null_空値()
        {
            TestContext.Run((AttachFileOKBlockTestData testSource) => TestExecute(testSource));
        }

        private void TestExecute(AttachFileOKBlockTestData testSource)
        {
            UnityContainer.RegisterInstance<bool>("IsEnableUploadContentCheck", testSource.IsEnableUploadContentCheck);
            UnityContainer.RegisterInstance<bool>("IsPriorityHigh_OKList", testSource.IsPriorityHigh_OKList);
            UnityContainer.RegisterInstance<bool>("IsUploadOk_NoExtensionFile", testSource.IsUploadOk_NoExtensionFile);
            UnityContainer.RegisterInstance<List<string>>("UploadOK_ContentTypeList", new List<string>());
            UnityContainer.RegisterInstance<List<string>>("UploadOK_ExtensionList", new List<string>());
            UnityContainer.RegisterInstance<List<string>>("BlockContentTypeList", new List<string>());
            UnityContainer.RegisterInstance<List<string>>("BlockExtensionList", new List<string>());

            if (!string.IsNullOrEmpty(testSource.UploadOK_ContentTypeList))
            {
                UnityContainer.RegisterInstance<List<string>>("UploadOK_ContentTypeList", GetListFromConfigString(testSource.UploadOK_ContentTypeList));
            }
            if (!string.IsNullOrEmpty(testSource.UploadOK_ExtensionList))
            {
                UnityContainer.RegisterInstance<List<string>>("UploadOK_ExtensionList", GetListFromConfigString(testSource.UploadOK_ExtensionList));
            }
            if (!string.IsNullOrEmpty(testSource.UploadBlock_ContentTypeList))
            {
                UnityContainer.RegisterInstance<List<string>>("BlockContentTypeList", GetListFromConfigString(testSource.UploadBlock_ContentTypeList));
            }
            if (!string.IsNullOrEmpty(testSource.UploadBlock_ExtensionList))
            {
                UnityContainer.RegisterInstance<List<string>>("BlockExtensionList", GetListFromConfigString(testSource.UploadBlock_ExtensionList));
            }

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction(contenttype: testSource.RequestContentType, filename: testSource.RequestFileName);

            var target = UnityCore.Resolve<CreateAttachFileActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);

            System.Diagnostics.Debug.Print(JsonConvert.SerializeObject(testSource));

            //処理実行
            target.Execute(() => { });

            var result = target.ReturnValue as HttpResponseMessage;
            System.Diagnostics.Debug.Print($"result:{result.StatusCode} expect:{testSource.ExpectStatusCode}");

            //チェック
            result.StatusCode.Is(testSource.ExpectStatusCode);
            if (!result.IsSuccessStatusCode)
            {
                result.Content.ReadAsStringAsync().Result.Contains(testSource.ExpectErrorCodeWheError).IsTrue();
            }
        }

        public class AttachFileOKBlockTestData
        {
            public string RequestContentType { get; set; }
            public string RequestFileName { get; set; }
            public bool IsEnableUploadContentCheck { get; set; } = true;
            public string UploadOK_ContentTypeList { get; set; }
            public string UploadOK_ExtensionList { get; set; }
            public string UploadBlock_ContentTypeList { get; set; }
            public string UploadBlock_ExtensionList { get; set; }
            public bool IsPriorityHigh_OKList { get; set; }
            public bool IsUploadOk_NoExtensionFile { get; set; }
            public HttpStatusCode ExpectStatusCode { get; set; }
            public string ExpectErrorCodeWheError { get; set; }
        }

    }
}
