using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_RegistDataAction : UnitTestBase
    {
        private string _versionKey1 = Guid.NewGuid().ToString();

        private Mock<INewDynamicApiDataStoreRepository> _mockRepository;
        private RegisterParam _registerOnceParam;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            //HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost", ""), new HttpResponse(new StringWriter()));
            UnityContainer.RegisterType<IHttpContextAccessor, HttpContextAccessor>();

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>("multiThread", perRequestDataContainer);

            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);
            UnityContainer.RegisterInstance<bool>("Return.JsonValidator.ErrorDetail", true);
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
        }

        private HttpResponseMessage ExecuteAction_リポジトリ1つ_登録_Common(
            Func<RegistAction, RegistAction> setRegistActionFunc,
            string requestContents = null,
            bool isRequestContentsIsArray = false,
            bool isHistoryTest = false,
            bool isConflictError = false,
            int conflictErrorOccurIndex = 0,
            string designationId = null,
            bool isBlockChainTest = false,
            string designationRepokey = null,
            bool isSkipReference = false
        )
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.IsSkipJsonFormatProtect = isSkipReference;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = string.IsNullOrEmpty(designationId) ? Guid.NewGuid().ToString() : designationId;
            var registerId = "id1";
            var contents = "";
            if (string.IsNullOrEmpty(requestContents))
            {
                contents = $"{{'id': '{registerId}','field1': 'value1','hoge':'val1','_id':'{id}','_Owner_Id':'{id}','_Reguser_Id':'{id}','_Regdate':'{{{{*}}}}','_Upduser_Id':'{id}','_Upddate':'{{{{*}}}}'}}";
            }
            else
            {
                contents = requestContents;
            }

            var contentsObject = JToken.Parse(contents);
            var expectId = new JObject() { ["id"] = registerId }.ToString();
            _mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            _mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(() =>
            {
                return new DocumentDataId(registerId, null, null);
            });
            DocumentDataId documentDataId;
            _mockRepository.Setup(x => x.KeyManagement.IsIdValid(It.IsAny<JToken>(), It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), out documentDataId)).Returns(() =>
            {
                return false;
            });
            _mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            var action = CreateRegistDataAction(id, isHistoryTest, isRequestContentsIsArray, designationRepokey: designationRepokey);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                _mockRepository.Object
            });
            action.Contents = new Contents(contents);
            setRegistActionFunc(action);

            _mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((para) =>
                {
                    if (contentsObject is JArray)
                    {
                        var targetId = para.Json["id"];
                        var array = contentsObject as JArray;
                        var j = array.Where(x => x["id"].ToString() == targetId.ToString()).FirstOrDefault();
                        para.Json.ToString().ToJson().Is(j);
                    }
                    else
                    {
                        para.Json.ToString().ToJson().Is(contentsObject.ToString().ToJson());
                    }
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                })
                .Returns(new RegisterOnceResult(registerId));

            int iExecCnt = 0;
            int iReferenceExecCnt = 0;

            var mockDocVerion = new Mock<IDocumentVersionRepository>();
            if (isConflictError == false)
            {
                mockDocVerion.Setup(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<bool>())).Returns(() =>
                {
                    var ret = new DocumentDbDocumentVersions();
                    ret.Id = "hogeId";
                    ret.Etag = "hogeTag";
                    ret.Partitionkey = "hogePart";
                    ret.RegDate = DateTime.Now;
                    ret.RegUserId = "hogeUser";
                    ret.Type = "hogeType";
                    ret.UpdDate = DateTime.Now;
                    ret.UpdUserId = "hogeUser";
                    ret.DocumentVersions = new List<DocumentDbDocumentVersion>()
                    {
                        new DocumentDbDocumentVersion(){ VersionKey = _versionKey1, CreateDate = DateTime.Now.ToString()  }
                    };
                    var mappingConfig = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<DocumentHistory, DocumentDbDocumentVersion>().ReverseMap();
                        cfg.CreateMap<DocumentHistories, DocumentDbDocumentVersions>().ReverseMap();
                    });
                    IMapper m = new Mapper(mappingConfig);

                    return m.Map<DocumentHistories>(ret);
                });

                if (isRequestContentsIsArray)
                {
                    var checkList = contentsObject.ToList();
                    _mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(() =>
                    {
                        var regId = checkList[iReferenceExecCnt]["id"].ToString();
                        iReferenceExecCnt++;
                        return new DocumentDataId(regId, null, null);
                    });

                    _mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                        .Callback<RegisterParam>((para) =>
                        {
                            _registerOnceParam = para;
                            if (contentsObject is JArray)
                            {
                                var targetId = para.Json["id"];
                                var array = contentsObject as JArray;
                                var j = array.Where(x => x["id"].ToString() == targetId.ToString()).FirstOrDefault();
                                para.Json.ToString().ToJson().Is(j);
                            }
                            else
                            {
                                para.Json.ToString().ToJson().Is(contentsObject.ToString().ToJson());
                            }
                            UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                        }).Returns(() => {
                            iExecCnt++;
                            if (isConflictError && conflictErrorOccurIndex == -1)
                            {
                                throw new ConflictException("hoge");
                            }
                            else if (isConflictError && (iExecCnt - 1) == conflictErrorOccurIndex)
                            {
                                throw new ConflictException("hoge");
                            }

                            return new RegisterOnceResult(registerId);
                        });

                }
            }
            else
            {
                if (isRequestContentsIsArray)
                {
                    var checkList = contentsObject.ToList();
                    _mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(() =>
                    {
                        var regId = checkList[iReferenceExecCnt]["id"].ToString();
                        iReferenceExecCnt++;
                        return new DocumentDataId(regId, null, null);
                    });


                    _mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                        .Callback<RegisterParam>((para) =>
                        {
                            if (contentsObject is JArray)
                            {
                                var targetId = para.Json["id"];
                                var array = contentsObject as JArray;
                                var j = array.Where(x => x["id"].ToString() == targetId.ToString()).FirstOrDefault();
                                para.Json.ToString().ToJson().Is(j);
                            }
                            else
                            {
                                para.Json.ToString().ToJson().Is(contentsObject.ToString().ToJson());
                            }
                            UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                        }).Returns(() =>
                        {
                            iExecCnt++;
                            if (isConflictError && conflictErrorOccurIndex == -1)
                            {
                                throw new ConflictException("hoge");
                            }
                            else if (isConflictError && (iExecCnt - 1) == conflictErrorOccurIndex)
                            {
                                throw new ConflictException("hoge");
                            }

                            return new RegisterOnceResult(registerId);
                        });
                }
                else
                {
                    _mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                        .Callback<RegisterParam>((para) =>
                        {
                            if (contentsObject is JArray)
                            {
                                var targetId = para.Json["id"];
                                var array = contentsObject as JArray;
                                var j = array.Where(x => x["id"].ToString() == targetId.ToString()).FirstOrDefault();
                                para.Json.ToString().ToJson().Is(j);
                            }
                            else
                            {
                                para.Json.ToString().ToJson().Is(contentsObject.ToString().ToJson());
                            }
                            UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                        })
                        .Returns(() =>
                        {
                            throw new ConflictException("hoge");
                        });
                }

                mockDocVerion.Setup(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<bool>())).Returns(() =>
                {
                    var ret = new DocumentDbDocumentVersions();
                    ret.Id = "hogeId";
                    ret.Etag = "hogeTag";
                    ret.Partitionkey = "hogePart";
                    ret.RegDate = DateTime.Now;
                    ret.RegUserId = "hogeUser";
                    ret.Type = "hogeType";
                    ret.UpdDate = DateTime.Now;
                    ret.UpdUserId = "hogeUser";
                    var mappingConfig = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<DocumentHistory, DocumentDbDocumentVersion>().ReverseMap();
                        cfg.CreateMap<DocumentHistories, DocumentDbDocumentVersions>().ReverseMap();
                    });
                    IMapper m = new Mapper(mappingConfig);
                    return m.Map<DocumentHistories>(ret);
                });
            }

            if (isBlockChainTest)
            {
                action.IsEnableBlockchain = new IsEnableBlockchain(true);
                var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
                mockBlockchainEventhub.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, JToken, RepositoryType, string>((hashid, token, type, versionKey) =>
                {
                    blockchainEventList.Add(new KeyValuePair<JToken, string>(token, versionKey));
                });
                blockchainEventList = new List<KeyValuePair<JToken, string>>();

                UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);

            }
            UnityContainer.RegisterInstance<IDocumentVersionRepository>(mockDocVerion.Object);

            _mockRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());

            var result = action.ExecuteAction();
            if (result.IsSuccessStatusCode)
            {
                if (isConflictError == false && isRequestContentsIsArray == false)
                {
                    result.Content.ReadAsStringAsync().Result.Is(expectId);
                }

                if (isRequestContentsIsArray)
                {
                    var checkList = contentsObject.ToList();
                    // 引数はCallbackで判定
                    _mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(checkList.Count));

                }
                else
                {
                    // 引数はCallbackで判定
                    _mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
                }
            }

            return result;
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_登録_Post()
        {
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                });
            result.StatusCode.Is(HttpStatusCode.Created);
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_登録_Historyありチェック_単数データ()
        {
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }, isHistoryTest: true);
            result.StatusCode.Is(HttpStatusCode.Created);

            var header = result.Headers.Single(x => x.Key == "X-DocumentHistory");
            header.IsNotNull();
            var documentHistory = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(header.Value.Single()).FirstOrDefault();
            documentHistory.documents.Single(x => x.versionKey == _versionKey1).IsNotNull();
        }


        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_登録_Historyありチェック_単数データ_blockchainあり()
        {
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }, isHistoryTest: true, isBlockChainTest: true);
            result.StatusCode.Is(HttpStatusCode.Created);

            var header = result.Headers.Single(x => x.Key == "X-DocumentHistory");
            header.IsNotNull();
            var documentHistory = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(header.Value.Single()).FirstOrDefault();
            documentHistory.documents.Single(x => x.versionKey == _versionKey1).IsNotNull();
            var bcevent = blockchainEventList.Single();
            bcevent.IsNotNull();
            bcevent.Value.Is(_versionKey1);
        }

        [TestMethod]
        public void ExecuteAction_OccuredConflictException_History無しチェック_単数_登録データなし_blockchainあり()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: true
                , requestContents: contents
                , isRequestContentsIsArray: true
                , conflictErrorOccurIndex: 0
                , designationId: designationId
                , isBlockChainTest: true
                );

            result.StatusCode.Is(HttpStatusCode.Conflict);
            //ヘッダチェック
            result.Headers.Count(x => x.Key == "X-DocumentHistory").Is(0);
            //blockchainのイベントチェック
            blockchainEventList.Any().IsFalse();
        }


        [TestMethod]
        public void ExecuteAction_OccuredConflictException_History無しチェック_単数データ()
        {
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }, isHistoryTest: true, isConflictError: true);
            result.StatusCode.Is(HttpStatusCode.Conflict);
            //ヘッダチェック
            var header = result.Headers.Count(x => x.Key == "X-DocumentHistory");
            //履歴ヘッダが返って来ていないこと
            header.Is(0);
        }



        [TestMethod]
        public void ExecuteAction_HistoryチェックNormal_３件データ_別ドキュメント()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId2','field': 'value2','hoge':'val2','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId3','field': 'value3','hoge':'val3','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId);

            result.StatusCode.Is(HttpStatusCode.Created);
            //ヘッダチェック
            result.Headers.Count(x => x.Key == "X-DocumentHistory").Is(1);

            //履歴ヘッダの中身チェック
            var header = result.Headers.GetValues("X-DocumentHistory").First().ToJson().ToList();
            //ヘッダは、自身の更新のみなので1件
            header.Count.Is(1);
            //ただし、documentsは3件
            header[0]["documents"].Count().Is(3);

            //各々のドキュメントの履歴があること
            header[0]["documents"][0]["documentKey"].ToString().Is("hogeId1");
            header[0]["documents"][1]["documentKey"].ToString().Is("hogeId2");
            header[0]["documents"][2]["documentKey"].ToString().Is("hogeId3");
        }

        [TestMethod]
        public void ExecuteAction_Historyチェック_３件データ_同一ドキュメント_IDが同じ()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId1','field': 'value2','hoge':'val2','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId1','field': 'value3','hoge':'val3','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId);

            //BadRequestであること
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_Historyチェック_３件データ_同一ドキュメント_リポジトリキーが同じ()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'hoge': 'hogeId1','field': 'value','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'hoge': 'hogeId1','field': 'value2','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'hoge': 'hogeId1','field': 'value3','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId);

            //BadRequestであること
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }


        [TestMethod]
        public void ExecuteAction_Historyチェック_３件データ_同一ドキュメント_リポジトリキー複数_同じ()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'hoge': 'hogeId1','field': 'value','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'hoge': 'hogeId1','field': 'value','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'hoge': 'hogeId1','field': 'value','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId
                , designationRepokey: "/API/Private/QueryTest/{hoge}/{field}");

            //BadRequestであること
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_Historyチェック_４件データ_同一ID_RepositoryKey混合ドキュメント_リポジトリキー複数_同じ()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'hoge': 'hogeId1','field': 'value1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'hoge': 'hogeId1','field': 'value2','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id':'hoge1','hoge': 'hogeId1','field': 'value','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id':'hoge1','hoge': 'hogeId2','field': 'value','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId
                , designationRepokey: "/API/Private/QueryTest/{hoge}/{field}");

            //BadRequestであること
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_OccuredConflictException_History無しチェック_３件データ_初回Conflict()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId2','field': 'value2','hoge':'val2','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId3','field': 'value3','hoge':'val3','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: true
                , requestContents: contents
                , isRequestContentsIsArray: true
                , conflictErrorOccurIndex: 0
                , designationId: designationId);

            result.StatusCode.Is(HttpStatusCode.Conflict);
            //ヘッダチェック
            result.Headers.Count(x => x.Key == "X-DocumentHistory").Is(1);

            //履歴ヘッダの中身チェック
            var header = result.Headers.GetValues("X-DocumentHistory").First().ToJson().ToList();
            //hogeId1 の履歴ヘッダは無いこと
            foreach (var h in header[0]["documents"])
            {
                h["documentKey"].ToString().IsNot("hogeId1");
            }
        }

        [TestMethod]
        public void ExecuteAction_OccuredConflictException_History無しチェック_３件データ_２件目Conflict()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId2','field': 'value2','hoge':'val2','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId3','field': 'value3','hoge':'val3','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: true
                , requestContents: contents
                , isRequestContentsIsArray: true
                , conflictErrorOccurIndex: 1
                , designationId: designationId);

            result.StatusCode.Is(HttpStatusCode.Conflict);
            //ヘッダチェック
            result.Headers.Count(x => x.Key == "X-DocumentHistory").Is(1);

            //履歴ヘッダの中身チェック
            var header = result.Headers.GetValues("X-DocumentHistory").First().ToJson().ToList();
            //hogeId2 の履歴ヘッダは無いこと
            foreach (var h in header[0]["documents"])
            {

                h["documentKey"].ToString().IsNot("hogeId2");
            }
        }

        [TestMethod]
        public void ExecuteAction_OccuredConflictException_History無しチェック_３件データ_３件目Conflict()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId2','field': 'value2','hoge':'val2','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId3','field': 'value3','hoge':'val3','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: true
                , requestContents: contents
                , isRequestContentsIsArray: true
                , conflictErrorOccurIndex: 2
                , designationId: designationId);

            result.StatusCode.Is(HttpStatusCode.Conflict);
            //ヘッダチェック
            result.Headers.Count(x => x.Key == "X-DocumentHistory").Is(1);

            //履歴ヘッダの中身チェック
            var header = result.Headers.GetValues("X-DocumentHistory").First().ToJson().ToList();
            //hogeId3 の履歴ヘッダは無いこと
            foreach (var h in header[0]["documents"])
            {
                h["documentKey"].ToString().IsNot("hogeId3");
            }
        }

        [TestMethod]
        public void ExecuteAction_OccuredConflictException_History無しチェック_３件データ_全件Conflict()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId2','field': 'value2','hoge':'val2','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}},
                               {{'id': 'hogeId3','field': 'value3','hoge':'val3','_id':'{designationId}','_Owner_Id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: true
                , requestContents: contents
                , isRequestContentsIsArray: true
                , conflictErrorOccurIndex: -1
                , designationId: designationId);

            result.StatusCode.Is(HttpStatusCode.Conflict);
            //履歴ヘッダは無いこと
            result.Headers.Count(x => x.Key == "X-DocumentHistory").Is(0);
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_Format_Error()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());

            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("String '2020-01-32' does not validate against format 'date'.(code:23)");
        }
        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_Format複数指定_JSchemaDefined項目あり_Format項目以外でエラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32a'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date;xxxx;yyyy'
        }
    }
}
";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("String '2020-01-32a' exceeds maximum length of 10.(code:4)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_Format複数指定_JSchemaDefined項目あり_Format項目でエラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date;xxxx;yyyy'
        }
    }
}
";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());

            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("String '2020-01-32' does not validate against format 'date'. Path ''.(code:26)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_Format複数指定_JSchemaDefined項目なし_Format以外の項目でエラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32a'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'xxxx;yyyy'
        }
    }
}
";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("String '2020-01-32a' exceeds maximum length of 10.(code:4)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_Format複数指定_JSchemaDefined項目の判定テスト用()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'? 🔰 ?'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'host-name;ip-address;hostname;date-time;date;time;utc-millisec;regex;color;style;phone;uri;uri-reference;uri-template;json-pointer;ipv6;ipv4;email'
        }
    }
}
";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(18 - 2);
            chk[0].ToString().Is("String '? 🔰 ?' does not validate against format 'host-name'. Path ''.(code:26)");
            chk[1].ToString().Is("String '? 🔰 ?' does not validate against format 'ip-address'. Path ''.(code:26)");
            chk[2].ToString().Is("String '? 🔰 ?' does not validate against format 'hostname'. Path ''.(code:26)");
            chk[3].ToString().Is("String '? 🔰 ?' does not validate against format 'date-time'. Path ''.(code:26)");
            chk[4].ToString().Is("String '? 🔰 ?' does not validate against format 'date'. Path ''.(code:26)");
            chk[5].ToString().Is("String '? 🔰 ?' does not validate against format 'time'. Path ''.(code:26)");
            chk[6].ToString().Is("String '? 🔰 ?' does not validate against format 'utc-millisec'. Path ''.(code:26)");
            chk[7].ToString().Is("String '? 🔰 ?' does not validate against format 'regex'. Path ''.(code:26)");
            chk[8].ToString().Is("String '? 🔰 ?' does not validate against format 'color'. Path ''.(code:26)");
            // style: JsonSchemaにvalidation処理がない
            // phone: JsonSchemaにvalidation処理がない
            chk[9].ToString().Is("String '? 🔰 ?' does not validate against format 'uri'. Path ''.(code:26)");
            chk[10].ToString().Is("String '? 🔰 ?' does not validate against format 'uri-reference'. Path ''.(code:26)");
            chk[11].ToString().Is("String '? 🔰 ?' does not validate against format 'uri-template'. Path ''.(code:26)");
            chk[12].ToString().Is("String '? 🔰 ?' does not validate against format 'json-pointer'. Path ''.(code:26)");
            chk[13].ToString().Is("String '? 🔰 ?' does not validate against format 'ipv6'. Path ''.(code:26)");
            chk[14].ToString().Is("String '? 🔰 ?' does not validate against format 'ipv4'. Path ''.(code:26)");
            chk[15].ToString().Is("String '? 🔰 ?' does not validate against format 'email'. Path ''.(code:26)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_1件()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':10000, 'a2':'hoge', 'a3':'fuga'}}";
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(4);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = msg["errors"]["a2"].ToList();
            chk.Count.Is(2);
            chk[0].ToString().Is("String 'hoge' exceeds maximum length of 1.(code:4)");
            chk[1].ToString().Is("Value \"hoge\" is not defined in enum.(code:17)");
            chk = msg["errors"]["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = msg["errors"]["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ExecuteAction_Error_Body空()
        {
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";

            var designationId = new Guid().ToString();
            var contents = "";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , IsRequestBodyNull: true);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10404.ToString(), actualMessage["error_code"]);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10404.ToString());
        }

        [TestMethod]
        public void ExecuteAction_Error_RequestBodyが空Json()
        {
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";

            var designationId = new Guid().ToString();
            var contents = "{}";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , IsRequestBodyNull: true);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10404.ToString());

        }

        [TestMethod]
        public void ExecuteAction_Error_RequestBodyのjson不正()
        {
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";

            var designationId = new Guid().ToString();
            var contents = "{'hoge':'null}";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , IsRequestBodyNull: true);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            //json形式で返って来ていること
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10403.ToString());

        }


        [TestMethod]
        public void ExecuteAction_Error_RequestBodyが空配列()
        {
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";

            var designationId = new Guid().ToString();
            var contents = "[]";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , IsRequestBodyNull: true);

            result.StatusCode.Is(HttpStatusCode.Created);
        }

        [TestMethod]
        public void ExecuteAction_Error_RegisterでRequestBodyがArray()
        {
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";

            var designationId = new Guid().ToString();
            var contents = "[{'a1':'hoge','a2':'hoge2'}]";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var chk = msg["errors"]["RootInvalid"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected Object but got Array.(code:18)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_1件_XML()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':10000, 'a2':'hoge', 'a3':'fuga'}}";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 1,
              'enum': [ 'hoghogehogheo', null],
              'type': 'string'
            }
          },
          'required':['a0','a4']
        }";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , contenttype: "application/xml");

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+xml
            result.Content.Headers.ContentType.MediaType.Is("application/problem+xml");
            var msg = result.Content.ReadAsStringAsync().Result.StringToXml();
            msg.Element("error_code").Value.Is(ErrorCodeMessage.Code.E10402.ToString());
            var chk = msg.Element("errors").Elements("a1").ToList();
            chk.Count.Is(1);
            chk[0].Value.Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = msg.Element("errors").Elements("a2").ToList();
            chk.Count.Is(2);
            chk[0].Value.Is("String 'hoge' exceeds maximum length of 1.(code:4)");
            chk[1].Value.Is(@"Value ""hoge"" is not defined in enum.(code:17)");

            chk = msg.Element("errors").Elements("a3").ToList();
            chk.Count.Is(1);
            chk[0].Value.Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");

            chk = msg.Element("errors").Elements("a0_x002C_a4").ToList();
            chk.Count.Is(1);
            chk[0].Value.Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_複数件_初回エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a1':10000, 'a2': 1000, 'a3':'fuga'}},
                                        {{'a0':2,'a1':'20000', 'a2':'hoge2','a4':2}},
                                        {{'a0':3,'a1':'30000', 'a2':'hoge3','a4':3}},
                                       ]";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , isRequestContentsIsArray: true
                , designationRepokey: "/API/Private/QueryTest/{a1}");

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(4);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = msg["errors"]["a2"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            chk = msg["errors"]["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = msg["errors"]["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_複数件_２件目エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a0':1,'a1':'10000', 'a2':'hoge1','a4':1}},
                                        {{'a1':200, 'a2':220, 'a3':'fuga'}},
                                        {{'a0':3,'a1':'30000', 'a2':'hoge3','a4':3}},
                                       ]";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , isRequestContentsIsArray: true
                , designationRepokey: "/API/Private/QueryTest/{a1}");

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(4);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = msg["errors"]["a2"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            chk = msg["errors"]["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = msg["errors"]["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_複数件_３件目エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a0':1,'a1':'10000', 'a2':'hoge1','a4':1}},
                                        {{'a0':2,'a1':'20000', 'a2':'hoge2','a4':2}},
                                        {{'a1':3000, 'a2':3300, 'a3':'fuga3'}},
                                       ]";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , isRequestContentsIsArray: true
                , designationRepokey: "/API/Private/QueryTest/{a1}");

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(4);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = msg["errors"]["a2"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            chk = msg["errors"]["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = msg["errors"]["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ExecuteAction_JsonValidationErrorTest_複数件_全部エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a1':10000, 'a2':1100,'a3':'fuga1'}},
                                        {{'a1':20000, 'a2':2200,'a3':'fuga2'}},
                                        {{'a1':30000, 'a2':3300,'a3':'fuga3'}},
                                       ]";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var result = ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , requestContents: contents
                , model: model
                , isRequestContentsIsArray: true);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg.Type.Is(JTokenType.Array);
            msg.Count().Is(3);
            //それぞれチェック

            msg[0]["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg[0]["errors"];
            messages.Count().Is(4);
            messages["a1"].Children().Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Children().Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Children().Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Children().Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");

            messages = msg[1]["errors"];
            messages.Count().Is(4);
            messages["a1"].Children().Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Children().Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Children().Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Children().Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");

            messages = msg[2]["errors"];
            messages.Count().Is(4);
            messages["a1"].Children().Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Children().Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Children().Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Children().Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");



        }

        private HttpResponseMessage ExecuteAction_BadRequest用_リポジトリ1つ_登録_Common(
            Func<RegistAction, RegistAction> setRegistActionFunc,
            string requestContents = null,
            string model = null,
            bool isRequestContentsIsArray = false,
            string contenttype = null,
            bool IsRequestBodyNull = false,
            string designationRepokey = null
        )
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var registerId = "id1";
            var contents = "";
            if (string.IsNullOrEmpty(requestContents) && IsRequestBodyNull == false)
            {
                contents = $"{{'id': '{registerId}','field1': 'value1','hoge':'val1','_id':'{id}','_Owner_Id':'{id}','_Reguser_Id':'{id}','_Regdate':'{{{{*}}}}'}}";
            }
            else
            {
                contents = requestContents;
            }

            JToken contentsObject = null;
            if (!string.IsNullOrEmpty(contents))
            {
                try
                {
                    contentsObject = JToken.Parse(contents);
                }
                catch
                {
                    //パースエラーの場合は、contentsObject はnull設定
                }
            }

            var expectId = new JObject() { ["id"] = registerId }.ToString();
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(() =>
            {
                return new DocumentDataId(registerId, null, null);
            });
            DocumentDataId documentDataId;
            mockRepository.Setup(x => x.KeyManagement.IsIdValid(It.IsAny<JToken>(), It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), out documentDataId)).Returns(() =>
            {
                return false;
            });



            var action = CreateRegistDataAction(id, false, isRequestContentsIsArray, model, contenttype, designationRepokey);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });
            action.Contents = new Contents(contents);
            setRegistActionFunc(action);

            mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((para) =>
                {
                    if (contentsObject is JArray)
                    {
                        var targetId = para.Json["id"];
                        var array = contentsObject as JArray;
                        var j = array.Where(x => x["id"].ToString() == targetId.ToString()).FirstOrDefault();
                        para.Json.ToString().ToJson().Is(j);
                    }
                    else if (contentsObject != null)
                    {
                        para.Json.ToString().ToJson().Is(contentsObject.ToString().ToJson());
                    }
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                })
                .Returns(new RegisterOnceResult(registerId));



            var result = action.ExecuteAction();
            // 引数はCallbackで判定
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(0));

            return result;
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_登録_Put()
        {
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT);
                    return action;
                });
            result.StatusCode.Is(HttpStatusCode.OK);
        }

        private Mock<INewDynamicApiDataStoreRepository> CreateINewDynamicApiDataStoreRepositoryMock(string registerId, JToken contentsObject,
            RegisterParam registerParam, IPerRequestDataContainer perRequestDataContainer, Exception exception = null, DocumentDataId documentDataId = null)
        {
            List<JsonDocument> jsonDocuments = new List<JsonDocument>();

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            if (documentDataId == null)
            {
                documentDataId = new DocumentDataId(registerId, null, null);
            }
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(() =>
            {
                return documentDataId;
            });

            DocumentDataId documentDataId2;
            mockRepository.Setup(x => x.KeyManagement.IsIdValid(It.IsAny<JToken>(), It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), out documentDataId2)).Returns(() =>
            {
                return false;
            });

            if (exception == null)
            {
                mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                    .Callback<RegisterParam>((para) =>
                    {
                        if (contentsObject != null)
                        {
                            if (contentsObject is JArray)
                            {
                                var targetId = para.Json["id"];
                                var array = contentsObject as JArray;
                                var j = array.Where(x => x["id"].ToString() == targetId.ToString()).FirstOrDefault();
                                para.Json.ToString().ToJson().Is(j);
                            }
                            else
                            {
                                para.Json.ToString().ToJson().Is(contentsObject.ToString().ToJson());
                            }
                        }
                        if (perRequestDataContainer != null)
                            UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                    })
                    .Returns(new RegisterOnceResult(documentDataId.Id));
            }
            else
            {
                mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((para) =>
                {
                    if (contentsObject != null)
                    {
                        if (contentsObject is JArray)
                        {
                            var targetId = para.Json["id"];
                            var array = contentsObject as JArray;
                            var j = array.Where(x => x["id"].ToString() == targetId.ToString()).FirstOrDefault();
                            para.Json.ToString().ToJson().Is(j);
                        }
                        else
                        {
                            para.Json.ToString().ToJson().Is(contentsObject.ToString().ToJson());
                        }
                    }
                    if (perRequestDataContainer != null)
                        UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                })
                .Throws(exception);
            }
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            return mockRepository;
        }



        [TestMethod]
        public void ExecuteAction_スキーマ_arrayではない_OK()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var registerId = "id1";
            var contents = $"{{'id': '{registerId}','field1': 'value1','hoge': 'huga','_id':'{id}','_Owner_Id':'{id}','_Reguser_Id':'{id}','_Regdate':'{{{{*}}}}','_Upduser_Id':'{id}','_Upddate':'{{{{*}}}}'}}";
            var contentsObject = JToken.Parse(contents);
            var expectId = new JObject() { ["id"] = registerId }.ToString();

            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(contents);
            action.RequestSchema = new DataSchema(@"
        {
          ""type"": ""object"",
          ""properties"": {
            ""field1"": {
              ""type"": ""string"",
            }
          }
        }");
            action.OpenId = new OpenId(id);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(registerId, contentsObject, null, perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });


            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.Is(expectId);
            result.StatusCode.Is(HttpStatusCode.Created);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_スキーマ_arrayではない_NG()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var contents = @"
        {
            ""field1"": ""value1""
        }";
            var tempobj = Newtonsoft.Json.JsonConvert.DeserializeObject(contents, new Newtonsoft.Json.JsonSerializerSettings { FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Decimal });
            var contentsObject = JToken.FromObject(tempobj);

            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(contents);
            var schema = @"
        {
          ""type"": ""object"",
          ""properties"": {
            ""field1"": {
              ""type"": ""number"",
            }
          }
        }";
            action.RequestSchema = new DataSchema(schema);
            IList<ValidationError> expectError;
            contentsObject.IsValid(
                JSchema.Parse(schema, new JSchemaReaderSettings
                {
                    Validators = new List<IJsonValidator> { new JsonFormatCustomValidator() }
                }), out expectError);
            var errorString = string.Join("", expectError.Select(error => $@"""errors"":{{""{error.Path}"":[""{error.Message}(code:{(int)error.ErrorType})""]}}"));

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock("id", contentsObject, null, perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.Contains(errorString).Is(true);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_スキーマ_array_OK()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var registerId = "id1";
            var contents = $"[{{'id': '{registerId}','field1': 'value1','hoge': 'huga','_id':'{id}','_Owner_Id':'{id}','_Reguser_Id':'{id}','_Regdate':'{{{{*}}}}','_Upduser_Id':'{id}','_Upddate':'{{{{*}}}}'}}]";
            var contentsObject = JToken.Parse(contents);
            var expectId = new JArray { new JObject() { ["id"] = registerId } }.ToString();

            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(contents);
            action.RequestSchema = new DataSchema(@"
        {
          ""type"": ""object"",
          ""properties"": {
            ""field1"": {
              ""type"": ""string"",
            }
          }
        }");
            action.PostDataType = new PostDataType("array");
            action.OpenId = new OpenId(id);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(registerId, contentsObject, null, perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });


            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.Is(expectId);
            result.StatusCode.Is(HttpStatusCode.Created);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_スキーマ_array_NG()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var contents = @"
        [{
            ""field1"": ""value1""
        }]";
            var tempobj = Newtonsoft.Json.JsonConvert.DeserializeObject(contents, new Newtonsoft.Json.JsonSerializerSettings { FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Decimal });
            var contentsObject = JToken.FromObject(tempobj);

            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(contents);
            action.PostDataType = new PostDataType("array");
            var schema = @"
        {
          ""type"": ""object"",
          ""properties"": {
            ""field1"": {
              ""type"": ""number"",
            }
          }
        }";
            action.RequestSchema = new DataSchema(schema);
            IList<ValidationError> expectError;
            contentsObject[0].IsValid(
                JSchema.Parse(schema, new JSchemaReaderSettings
                {
                    Validators = new List<IJsonValidator> { new JsonFormatCustomValidator() }
                }), out expectError);
            var errorString = string.Join("", expectError.Select(error => $"array 0 Lineno={error.LineNumber} Position={error.LinePosition} Path={error.Path} ErrorMessage={error.Message} "));
            action.Initialize();

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock("id", contentsObject, null, perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });


            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.IndexOf(expectError[0].Message).IsNot(-1);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_NotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.Contents = new Contents("{ 'hoge':''}");

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock("id", null, null, perRequestDataContainer, new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_OK()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var registerId1 = "hoge1";
            var registerId2 = "hoge2";

            var id = Guid.NewGuid().ToString();
            var contents = @"
        {
            ""hoge"": ""value1""
        }";
            var contentsObject = JToken.Parse(contents);
            var expectId = new JObject() { ["id"] = registerId1 }.ToString();

            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(contents);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(registerId1, null, null, perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(registerId2, null, null, perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.Is(expectId);
            result.StatusCode.Is(HttpStatusCode.Created);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つNotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var registerId = "id1";

            var id = Guid.NewGuid().ToString();
            var contents = "{ 'hoge': 'value1' }";
            var contentsObject = JToken.Parse(contents);
            var expectId = new JObject() { ["id"] = registerId }.ToString();

            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(contents);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(registerId, null, null, perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(registerId, null, null, perRequestDataContainer, new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });
            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.Is(expectId);
            result.StatusCode.Is(HttpStatusCode.Created);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_2つNotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var contents = @"
        {
            ""hoge"": ""value1""
        }";
            var contentsObject = JToken.Parse(contents);

            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(contents);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock("id", null, null, perRequestDataContainer, new NotImplementedException());
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock("id", null, null, perRequestDataContainer, new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリキー無し_Post()
        {
            var id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.RepositoryKey = new RepositoryKey(null);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void ExecuteAction_リポジトリキー無し_Put()
        {
            var id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.RepositoryKey = new RepositoryKey(null);
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void ExecuteAction_Get()
        {
            var id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void ExecuteAction_Delete()
        {
            var id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void ExecuteAction_Patch()
        {
            var id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void ExecuteAction_AutomaticId_id()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var registerId = id;
            var registerIdFull = $"hogehoge~{id}";
            var contents = $"{{'field1': 'value1','_id':'{id}','_Owner_Id':'{id}','_Reguser_Id':'{id}','_Regdate':'{{{{*}}}}','_Upduser_Id':'{id}','_Upddate':'{{{{*}}}}','id':'{registerIdFull}'}}";
            var contentsObject = JToken.Parse(contents);
            var expectId = new JObject() { ["id"] = registerIdFull }.ToString();

            var action = CreateRegistDataAction(id);
            action.IsAutomaticId = new IsAutomaticId(true);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest");
            action.Contents = new Contents(contents);
            action.RequestSchema = new DataSchema(@"
        {
          ""type"": ""object"",
          ""properties"": {
            ""field1"": {
              ""type"": ""string"",
            }
          }
        }");
            action.OpenId = new OpenId(id);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock("id", contentsObject, null, perRequestDataContainer, null, new DocumentDataId(registerIdFull, null, registerId));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.Is(expectId);
            result.StatusCode.Is(HttpStatusCode.Created);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_AutomaticId_key()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var registerId = id;
            var registerIdFull = $"hogehoge~{id}";
            var contents = $"{{'field1': 'value1','_id':'{id}','_Owner_Id':'{id}','_Reguser_Id':'{id}','_Regdate':'{{{{*}}}}','_Upduser_Id':'{id}','_Upddate':'{{{{*}}}}','id':'{registerIdFull}','key':'{id}'}}";
            var contentsObject = JToken.Parse(contents);
            var expectId = new JObject() { ["id"] = registerIdFull }.ToString();

            var action = CreateRegistDataAction(id);
            action.IsAutomaticId = new IsAutomaticId(true);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{key}");
            action.Contents = new Contents(contents);
            action.RequestSchema = new DataSchema(@"
        {
          ""type"": ""object"",
          ""properties"": {
            ""field1"": {
              ""type"": ""string"",
            }
          }
        }");
            action.OpenId = new OpenId(id);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock("id", contentsObject, null, perRequestDataContainer, null, new DocumentDataId(registerIdFull, null, registerId));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.Is(expectId);
            result.StatusCode.Is(HttpStatusCode.Created);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_AutomaticId_key3()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var registerId = id;
            var registerIdFull = $"hogehoge~{id}";
            var contents = $"{{'field1': 'value1','_id':'{id}','_Owner_Id':'{id}','_Reguser_Id':'{id}','_Regdate':'{{{{*}}}}','_Upduser_Id':'{id}','_Upddate':'{{{{*}}}}','id':'{registerIdFull}'}}";
            var contentsObject = JToken.Parse(contents);
            var expectId = new JObject() { ["id"] = registerIdFull }.ToString();

            var action = CreateRegistDataAction(id);
            action.IsAutomaticId = new IsAutomaticId(true);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{key1}/{key2}");
            action.Contents = new Contents(contents);
            action.RequestSchema = new DataSchema(@"
        {
          ""type"": ""object"",
          ""properties"": {
            ""field1"": {
              ""type"": ""string"",
            }
          }
        }");
            action.OpenId = new OpenId(id);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock("id", contentsObject, null, perRequestDataContainer, null, new DocumentDataId(registerIdFull, null, registerId));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.Content.ReadAsStringAsync().Result.Is(expectId);
            result.StatusCode.Is(HttpStatusCode.Created);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_MailTemplateあり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);

            // テスト対象のインスタンスを設定
            string id = Guid.NewGuid().ToString();

            var action = CreateRegistDataAction(id);
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            action.Contents = new Contents(JsonConvert.SerializeObject(new { key = "value" }));
            action.HasMailTemplate = new HasMailTemplate(true);

            var DynamicApiDataStoreRepository = CreateINewDynamicApiDataStoreRepositoryMock(id, null, null, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        DynamicApiDataStoreRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Register(It.Is<IDynamicApiAction>(a => a == action),
                It.Is<JToken>(j => j.Value<string>("key") == "value")), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Webhookあり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);
            // テスト対象のインスタンスを設定
            string id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            action.Contents = new Contents(JsonConvert.SerializeObject(new { key = "value" }));
            action.HasWebhook = new HasWebhook(true);

            var DynamicApiDataStoreRepository = CreateINewDynamicApiDataStoreRepositoryMock(id, null, null, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        DynamicApiDataStoreRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Register(It.Is<IDynamicApiAction>(a => a == action),
                It.Is<JToken>(j => j.Value<string>("key") == "value")), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Webhookあり_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", false);

            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);
            // テスト対象のインスタンスを設定
            string id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            action.Contents = new Contents(JsonConvert.SerializeObject(new { key = "value" }));
            action.HasWebhook = new HasWebhook(true);

            var DynamicApiDataStoreRepository = CreateINewDynamicApiDataStoreRepositoryMock(id, null, null, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        DynamicApiDataStoreRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Register(It.IsAny<IDynamicApiAction>(), It.IsAny<JToken>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_Webhookあり_MailTemplateあり_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", false);

            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);
            // テスト対象のインスタンスを設定
            string id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            action.Contents = new Contents(JsonConvert.SerializeObject(new { key = "value" }));
            action.HasWebhook = new HasWebhook(true);
            action.HasMailTemplate = new HasMailTemplate(true);

            var DynamicApiDataStoreRepository = CreateINewDynamicApiDataStoreRepositoryMock(id, null, null, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        DynamicApiDataStoreRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Register(It.IsAny<IDynamicApiAction>(), It.IsAny<JToken>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_Webhookなし_MailTemplateあり_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", false);

            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);
            // テスト対象のインスタンスを設定
            string id = Guid.NewGuid().ToString();
            var action = CreateRegistDataAction(id);
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            action.Contents = new Contents(JsonConvert.SerializeObject(new { key = "value" }));
            action.HasWebhook = new HasWebhook(false);
            action.HasMailTemplate = new HasMailTemplate(true);

            var DynamicApiDataStoreRepository = CreateINewDynamicApiDataStoreRepositoryMock(id, null, null, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        DynamicApiDataStoreRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Register(It.IsAny<IDynamicApiAction>(), It.IsAny<JToken>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_Blockchainあり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            // モックを作成
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, JToken, RepositoryType, string>((hashid, token, type, versionKey) =>
                {
                    blockchainEventList.Add(new KeyValuePair<JToken, string>(token, versionKey));
                });
            blockchainEventList = new List<KeyValuePair<JToken, string>>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);

            var registerData = new List<JToken>();
            // テスト対象のインスタンスを設定
            string id = Guid.NewGuid().ToString();
            var DynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            DynamicApiDataStoreRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>()))
                .Returns(new DocumentDataId(id, null, null))
                .Callback<RegisterParam, IResourceVersionRepository, IPerRequestDataContainer>((para, rv, rc) =>
                {
                    registerData.Add(para.Json);
                });
            DocumentDataId documentDataIdd;
            DynamicApiDataStoreRepository.Setup(x => x.KeyManagement.IsIdValid(It.IsAny<JToken>(), It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), out documentDataIdd)).Returns(() =>
            {
                return false;
            });
            DynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));


            var action = CreateRegistDataAction(id);
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            action.Contents = new Contents(JsonConvert.SerializeObject(new { key = "value" }));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                       DynamicApiDataStoreRepository.Object
                    });
            action.IsEnableBlockchain = new IsEnableBlockchain(true);



            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockBlockchainEventhub.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()), Times.Once);
            blockchainEventList.Count().Is(1);
            blockchainEventList.Single().Key.Is(registerData.Single());
            blockchainEventList.Single().Value.IsNull();
        }

        [TestMethod]
        public void ExecuteAction_AttachFileBase64()
        {
            string id = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(new DocumentDataId(id, null, null));
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            // テスト対象のインスタンスを設定
            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal), "hoge1"));
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.Created);
            string path = $"attachfilebase64/{id}/file";
            string fullPath = $"https://fuga.blob.core.windows.net/{VendorId.ToString()}/attachfilebase64/{id}/file";
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal)), It.Is<string>(c => c.Equals(path))), Times.Exactly(1));
            mockRepository.Verify(x => x.RegisterOnce(It.Is<RegisterParam>(a => a.Json.Value<string>("file").Equals(CreateBase64Registed(fullPath)))), Times.Exactly(1));

        }

        [TestMethod]
        public void ExecuteAction_AttachFileBase64_複数ファイル()
        {
            string id = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(new DocumentDataId(id, null, null));

            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(CreateBase64AttachFileJson2(CreateBase64Query(base64StringNormal), CreateBase64Query(base64StringNormal2)));
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.Created);
            string path = $"attachfilebase64/{id}/file";
            string path2 = $"attachfilebase64/{id}/file2";
            string fullPath = $"https://fuga.blob.core.windows.net/{VendorId.ToString()}/attachfilebase64/{id}/file";
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal)), It.Is<string>(c => c.Equals(path))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal2)), It.Is<string>(c => c.Equals(path2))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.DeleteFilestoBase64(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals($"attachfilebase64/{id}"))), Times.Exactly(1));

            mockRepository.Verify(x => x.RegisterOnce(It.Is<RegisterParam>(a => a.Json.Value<string>("file").Equals(CreateBase64Registed(fullPath)))), Times.Exactly(1));

        }

        [TestMethod]
        public void ExecuteAction_AttachFileBase64_Array()
        {
            string id = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(new DocumentDataId(id, null, null));
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            var action = CreateRegistDataAction(id);
            var json = $"[{CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal), "hoge1")},{CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal2), "hoge2")}]";

            action.Contents = new Contents(json);
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.PostDataType = new PostDataType("array");
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            var msg = result.Content.ReadAsStringAsync().Result;
            result.StatusCode.Is(HttpStatusCode.Created);

            string path = $"attachfilebase64/{id}/file";
            string fullPath = $"https://fuga.blob.core.windows.net/{VendorId.ToString()}/attachfilebase64/{id}/file";

            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal)), It.Is<string>(c => c.Equals(path))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal2)), It.Is<string>(c => c.Equals(path))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.DeleteFilestoBase64(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals($"attachfilebase64/{id}"))), Times.Exactly(2));

            mockRepository.Verify(x => x.RegisterOnce(It.Is<RegisterParam>(a => a.Json.Value<string>("file").Equals(CreateBase64Registed(fullPath)))), Times.Exactly(2));
        }

        [TestMethod]
        public void ExecuteAction_AttachFileBase64_添付ファイル設定なし()
        {
            string id = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(new DocumentDataId(id, null, null));
            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal), "hoge1"));
            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Contains(@"$Base64 cannot be used unless baldness is attachfile enabled").Is(true);
        }

        [TestMethod]
        public void ExecuteAction_AttachFileBase64_制限サイズ以上()
        {
            string id = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            UnityContainer.RegisterInstance<int>("MaxBase64AttachFileContentLength", 18863);
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(new DocumentDataId(id, null, null));
            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal), "hoge1"));
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Contains("Request base64 size is too large").Is(true);
        }

        [TestMethod]
        public void ExecuteAction_AttachFileBase64_制限サイズ以内()
        {
            string id = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            UnityContainer.RegisterInstance<int>("MaxBase64AttachFileContentLength", 18864);
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(new DocumentDataId(id, null, null));
            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal), "hoge1"));
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.Created);
        }

        [TestMethod]
        public void ExecuteAction_AttachFileBase64_Base64不正()
        {
            string id = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(new DocumentDataId(id, null, null));
            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(CreateBase64AttachFileJson(CreateBase64Query(base64StringBroken), "hoge1"));
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();

            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Contains("can not parse base64").Is(true);
        }

        [TestMethod]
        public void ExecuteAction_AttachFileBase64_Base64空()
        {
            string id = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(new DocumentDataId(id, null, null));
            var action = CreateRegistDataAction(id);
            action.Contents = new Contents(CreateBase64AttachFileJson(CreateBase64Query(" "), "hoge1"));
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Contains("can not parse base64").Is(true);
        }

        [TestMethod]
        public void ExecuteAction_contentsにKey項目なし_NG()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var id = Guid.NewGuid().ToString();
            var registerId = id;
            var registerIdFull = $"hogehoge~{id}";
            var contents = $"{{'field1': 'value1','_id':'{id}','_Owner_Id':'{id}','_Reguser_Id':'{id}','_Regdate':'{{{{*}}}}','_Upduser_Id':'{id}','_Upddate':'{{{{*}}}}','id':'{registerIdFull}'}}";
            var contentsObject = JToken.Parse(contents);
            var expectId = new JObject() { ["id"] = registerIdFull }.ToString();

            var action = CreateRegistDataAction(id);
            action.IsAutomaticId = new IsAutomaticId(false);
            action.Contents = new Contents(contents);
            action.RequestSchema = new DataSchema(@"
        {
          ""type"": ""object"",
          ""properties"": {
            ""field1"": {
              ""type"": ""string"",
            },
            ""hoge"": {
              ""type"": ""string"",
            }
          }
        }");
            action.OpenId = new OpenId(id);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock("id", contentsObject, null, perRequestDataContainer, null, new DocumentDataId(registerIdFull, null, registerId));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["detail"].ToString().Is("hoge key not found");
        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId);

            result.StatusCode.Is(HttpStatusCode.Created);
            result.Headers.Contains("X-DocumentHistory").IsFalse();
            mockHistoryEvacuationDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Never);
        }

        [TestMethod]
        public void EnableJsonDocumentHistory_Enable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", false);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", false);
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId);

            result.StatusCode.Is(HttpStatusCode.Created);
            result.Headers.Contains("X-DocumentHistory").IsTrue();
            _mockRepository.Verify(x => x.QueryOnce(It.IsAny<QueryParam>()), Times.Once());
        }

        [TestMethod]
        public void EnableJsonDocumentReference_Enable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", false);
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , isSkipReference: false
                , designationId: designationId);

            result.StatusCode.Is(HttpStatusCode.Created);
            result.Headers.Contains("X-DocumentHistory").IsFalse();
            _mockRepository.Verify(x => x.QueryOnce(It.IsAny<QueryParam>()), Times.Once());
        }

        /// <summary>
        /// KeepRegDateのみ有効な場合は最初に１回目のExecuteでデータが作られ（RegDateがセットされる）、２階目のExecuteでUpdDateは更新されるが、RegDateは変化しない
        /// </summary>
        [TestMethod]
        public void KeepRegDate_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", false);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var registerOnceResut = contents.Substring(1, contents.Length - 2);
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId);
            result.StatusCode.Is(HttpStatusCode.Created);
            var json1 = _registerOnceParam.Json;

            // 1秒待たないとjson1._Regdateとjson2._Regdateが同じになってしまうため
            System.Threading.Thread.Sleep(1000);

            // 2回目でRegDateは変化していること
            designationId = new Guid().ToString();
            result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    _mockRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>()))
                        .Returns(() =>
                        {
                            return new JsonDocument(json1);
                        });
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , designationId: designationId);
            var json2 = _registerOnceParam.Json;
            result.StatusCode.Is(HttpStatusCode.Created);
            json1["_Regdate"].Is(json2["_Regdate"]);
            (json1["_Upddate"] == json2["_Upddate"]).IsFalse();
            _mockRepository.Verify(x => x.QueryOnce(It.IsAny<QueryParam>()), Times.Once());
        }

        [TestMethod]
        public void RegisterInPreQuery_None()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", false);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", false);
            var designationId = new Guid().ToString();
            var contents = $@"[{{'id': 'hogeId1','field': 'value','hoge':'val1','_id':'{designationId}','_Upduser_Id':'{designationId}','_Upddate':'{{{{*}}}}','_Owner_Id':'{designationId}','_Reguser_Id':'{designationId}','_Regdate':'{{{{*}}}}'}}]";
            var registerOnceResut = contents.Substring(1, contents.Length - 2);
            var jatmp = JArray.Parse(contents);
            var cont = jatmp.ToString();
            var result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , isSkipReference: true
                , designationId: designationId);
            result.StatusCode.Is(HttpStatusCode.Created);
            var json1 = _registerOnceParam.Json;

            // 1秒待たないとjson1._Regdateとjson2._Regdateが同じになってしまうため
            System.Threading.Thread.Sleep(1000);

            // 2回目でRegDateは変化していること
            designationId = new Guid().ToString();
            result = ExecuteAction_リポジトリ1つ_登録_Common(
                action =>
                {
                    action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
                    _mockRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>()))
                        .Returns(() =>
                        {
                            return new JsonDocument(json1);
                        });
                    return action;
                }
                , isHistoryTest: true
                , isConflictError: false
                , requestContents: contents
                , isRequestContentsIsArray: true
                , isSkipReference: true
                , designationId: designationId);
            var json2 = _registerOnceParam.Json;
            result.StatusCode.Is(HttpStatusCode.Created);
            // RegdateやUpddateは更新されてしまう（Keep RegDateにしていないから）
            (json1["_Regdate"] == json2["_Regdate"]).IsFalse();
            (json1["_Upddate"] == json2["_Upddate"]).IsFalse();
            _mockRepository.Verify(x => x.QueryOnce(It.IsAny<QueryParam>()), Times.Never);
        }

        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private Mock<INewDynamicApiDataStoreRepository> mockHistoryEvacuationDataStoreRepository;

        private RegistAction CreateRegistDataAction(string id, bool isHistoryTest = false, bool isRequestContentsIsArray = false, string definitionModel = null, string contenttype = null, string designationRepokey = null)
        {
            RegistAction action = UnityCore.Resolve<RegistAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/QueryTest");
            action.RepositoryKey = new RepositoryKey(string.IsNullOrEmpty(designationRepokey) ? "/API/Private/QueryTest/{hoge}" : designationRepokey);
            action.ActionType = new ActionTypeVO(ActionType.Regist);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.IsVendor = new IsVendor(false);
            action.IsPerson = new IsPerson(false);
            action.OpenId = new OpenId(id);
            action.IsOpenIdAuthentication = new IsOpenIdAuthentication(false);
            action.IsAutomaticId = new IsAutomaticId(false);
            action.ProviderSystemId = new SystemId(SystemId.ToString());
            action.ProviderVendorId = new VendorId(VendorId.ToString());

            action.RequestSchema = definitionModel == null ? new DataSchema(null) : new DataSchema(definitionModel);
            action.Accept = string.IsNullOrEmpty(contenttype) ? new Accept("*/*") : new Accept(contenttype);
            action.AttachFileBlobRepositoryInfo = new RepositoryInfo("afb", new Dictionary<string, bool>() { { "DefaultEndpointsProtocol=https;AccountName=fuga;AccountKey=hoge;EndpointSuffix=core.windows.net", false } });
            action.IsEnableBlockchain = new IsEnableBlockchain(false);
            action.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>() { new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "connectionstring", false } }) });

            if (isHistoryTest)
            {
                action.IsDocumentHistory = new IsDocumentHistory(true);
                mockHistoryEvacuationDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
                action.HistoryEvacuationDataStoreRepository = mockHistoryEvacuationDataStoreRepository.Object;
            }
            if (isRequestContentsIsArray)
            {
                action.PostDataType = new PostDataType("array");
            }

            return action;
        }

        private Mock<IDynamicApiAttachFileRepository> SetUpBase64AttachFileRepositoryMock()
        {
            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.IsAny<string>(), It.IsAny<string>()));
            mockBlobRepository.Setup(x => x.DeleteFilestoBase64(It.IsAny<VendorId>(), It.IsAny<string>()));
            return mockBlobRepository;
        }

        private static string base64StringNormal = "";
        private static string base64StringNormal2 = "";
        private static string base64StringBroken = "";
        private List<KeyValuePair<JToken, string>> blockchainEventList;

        private static string CreateBase64Query(string value)
        {
            return $"$Base64({value})";
        }
        private static string CreateBase64Registed(string value)
        {
            return $"$Base64Reference({value})";
        }
        private static string CreateBase64AttachFileJson(string base64attachfile, string id)
        {
            return $@"{{
'hoge':'{id}',
'AreaUnitCode': 'あああああああああああああああああああああああああああ',
'file':'{base64attachfile}'
}}";
        }
        private static string CreateBase64AttachFileJson2(string base64attachfile, string base64attachfile2)
        {
            return $@"{{
'hoge':'hoge',
'AreaUnitCode': 'あああああああああああああああああああああああああああ',
'file':'{base64attachfile}',
'file2':'{base64attachfile2}'
}}";
        }

    }
}
