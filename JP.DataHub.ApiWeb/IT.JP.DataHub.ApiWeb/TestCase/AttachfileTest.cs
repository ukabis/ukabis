using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class AttachfileTest : ApiWebItTestCase
    {
        #region TestData

        private class AttachfileTestData : TestDataBase
        {
            public AutoKey3DataModel Data0 = new AutoKey3DataModel()
            {
                key1 = "key-1",
                key2 = "key-2",
                key3 = "key-3"
            };
            public RegisterResponseModel Data0RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}"
            };
            public List<AutoKey3DataModel> Data0Get = new List<AutoKey3DataModel>()
            {
                new AutoKey3DataModel()
                {
                    id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    key1 = "key-1",
                    key2 = "key-2",
                    key3 = "key-3"
                }
            };

            public CreateAttachFileRequestModel Data1 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };

            public GetAttachFileResponseModel Data1MetaExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                isDrm = false,
                isUploaded = true,
                metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                {
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                        metaValue = WILDCARD
                    },
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };
            public GetAttachFileResponseModel Data1MetaExExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                isDrm = false,
                isUploaded = true,
                metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                {
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                        metaValue = WILDCARD
                    },
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = false,
                _Version = 1,
                _partitionkey = $"API~IntegratedTest~Attachfile~{WILDCARD}",
                _Type = "API~IntegratedTest~Attachfile",
                _Owner_Id = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };

            public List<GetAttachFileResponseModel> Data1MetaExpect2 = new List<GetAttachFileResponseModel>()
            {
                new GetAttachFileResponseModel()
                {
                    fileId = WILDCARD,
                    fileName = "tenputest.jpg",
                    contentType = "image/jpeg",
                    fileLength = 1000,
                    isDrm = false,
                    isUploaded = true,
                    metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                    {
                        new GetAttachFileResponseModel.MetaInfo()
                        {
                            metaKey = "TestKey",
                            metaValue = WILDCARD
                        },
                        new GetAttachFileResponseModel.MetaInfo()
                        {
                            metaKey = "Key2",
                            metaValue = "KeyValue2"
                        }
                    }
                }
            };
            public List<GetAttachFileResponseModel> Data1MetaExExpect2 = new List<GetAttachFileResponseModel>()
            {
                new GetAttachFileResponseModel()
                {
                    fileId = WILDCARD,
                    fileName = "tenputest.jpg",
                    contentType = "image/jpeg",
                    fileLength = 1000,
                    isDrm = false,
                    isUploaded = true,
                    metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                    {
                        new GetAttachFileResponseModel.MetaInfo()
                        {
                            metaKey = "TestKey",
                            metaValue = WILDCARD
                        },
                        new GetAttachFileResponseModel.MetaInfo()
                        {
                            metaKey = "Key2",
                            metaValue = "KeyValue2"
                        }
                    },
                    IsExternalAttachFile = false,
                    _Version = 1,
                    _partitionkey = $"API~IntegratedTest~Attachfile~{WILDCARD}",
                    _Type = "API~IntegratedTest~Attachfile",
                    _Owner_Id = WILDCARD,
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD
                }
            };

            public CreateAttachFileRequestModel Data2 = new CreateAttachFileRequestModel()
            {
                fileName = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890ab.jpg",
                contentType = "image/jpeg",
                fileLength = 4076717,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };

            public GetAttachFileResponseModel Data2MetaExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890ab.jpg",
                contentType = "image/jpeg",
                fileLength = 4076717,
                isDrm = false,
                isUploaded = true,
                metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                {
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                        metaValue = WILDCARD
                    },
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };
            public List<GetAttachFileResponseModel> Data2MetaExpect2 = new List<GetAttachFileResponseModel>()
            {
                new GetAttachFileResponseModel()
                {
                    fileId = WILDCARD,
                    fileName = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890ab.jpg",
                    contentType = "image/jpeg",
                    fileLength = 4076717,
                    isDrm = false,
                    isUploaded = true,
                    metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                    {
                        new GetAttachFileResponseModel.MetaInfo()
                        {
                            metaKey = "TestKey",
                            metaValue = WILDCARD
                        },
                        new GetAttachFileResponseModel.MetaInfo()
                        {
                            metaKey = "Key2",
                            metaValue = "KeyValue2"
                        }
                    }
                }
            };

            public CreateAttachFileRequestModelEx DataBadRequest1 = new CreateAttachFileRequestModelEx()
            {
                FileId = Guid.NewGuid().ToString(),
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };
            public CreateAttachFileRequestModel DataBadRequest2 = new CreateAttachFileRequestModel()
            {
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };
            public CreateAttachFileRequestModel DataBadRequest3 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };
            public CreateAttachFileRequestModel DataBadRequest4 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };

            public AttachFileBase64Model Data0Base64 = new AttachFileBase64Model()
            {
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };
            public RegisterResponseModel Data0Base64RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}"
            };
            public List<AttachFileBase64Model> Data0Base64Get = new List<AttachFileBase64Model>()
            {
                new AttachFileBase64Model()
                {
                    id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                    name = "hogehoge"
                }
            };
            public List<AttachFileBase64Model> Data0Base64GetFull = new List<AttachFileBase64Model>()
            {
                new AttachFileBase64Model()
                {
                    id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                    name = "hogehoge",
                    _Version = 1,
                    _partitionkey = "API~IntegratedTest~Attachfile~1",
                    _Type = "API~IntegratedTest~Attachfile",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD
                }
            };

            public AttachFileBase64Model Data2Base64 = new AttachFileBase64Model()
            {
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };
            public RegisterResponseModel Data2Base64RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}"
            };
            public AttachFileBase64Model Data2Base64Get = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };
            public AttachFileBase64Model Data2Base64GetFull = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge",
                _Version = 1,
                _partitionkey = "API~IntegratedTest~Attachfile~1",
                _Type = "API~IntegratedTest~Attachfile",
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };

            public AttachFileBase64Model Data3Base64 = new AttachFileBase64Model()
            {
                file = "",
                name = "hogehoge"
            };

            public AttachFileBase64Model Data4Base64 = new AttachFileBase64Model()
            {
                fileObjects = new List<AttachFileBase64Object>()
                {
                    new AttachFileBase64Object()
                    {
                        file_01 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    },
                    new AttachFileBase64Object()
                    {
                        file_02 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    }
                },
                name = "base64_testdata_04"
            };
            public RegisterResponseModel Data4Base64RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}"
            };
            public AttachFileBase64Model Data4Base64Get = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                fileObjects = new List<AttachFileBase64Object>()
                {
                    new AttachFileBase64Object()
                    {
                        file_01 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    },
                    new AttachFileBase64Object()
                    {
                        file_02 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    }
                },
                name = "base64_testdata_04"
            };
            public AttachFileBase64Model Data4Base64GetFull = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                fileObjects = new List<AttachFileBase64Object>()
                {
                    new AttachFileBase64Object()
                    {
                        file_01 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    },
                    new AttachFileBase64Object()
                    {
                        file_02 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    }
                },
                name = "base64_testdata_04",
                _Version = 1,
                _partitionkey = "API~IntegratedTest~Attachfile~1",
                _Type = "API~IntegratedTest~Attachfile",
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };

            public AttachFileBase64Model Data5Base64 = new AttachFileBase64Model()
            {
                files = new List<string>()
                {
                    "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                    "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                },
                name = "base64_testdata_05"
            };
            public RegisterResponseModel Data5Base64RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}"
            };
            public AttachFileBase64Model Data5Base64Get = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                files = new List<string>()
                {
                    "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                    "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                },
                name = "base64_testdata_05"
            };
            public AttachFileBase64Model Data5Base64GetFull = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                files = new List<string>()
                {
                    "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                    "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                },
                name = "base64_testdata_05",
                _Version = 1,
                _partitionkey = "API~IntegratedTest~Attachfile~1",
                _Type = "API~IntegratedTest~Attachfile",
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };

            public AttachFileBase64Model Data6Base64 = new AttachFileBase64Model()
            {
                file_01 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file_02 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "base64_testdata_06"
            };
            public RegisterResponseModel Data6Base64RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}"
            };
            public AttachFileBase64Model Data6Base64Get = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                file_01 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file_02 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "base64_testdata_06"
            };
            public AttachFileBase64Model Data6Base64GetFull = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                file_01 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file_02 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "base64_testdata_06",
                _Version = 1,
                _partitionkey = "API~IntegratedTest~Attachfile~1",
                _Type = "API~IntegratedTest~Attachfile",
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };

            public AttachFileBase64Model Data8Base64 = new AttachFileBase64Model()
            {
                fileObject = new AttachFileBase64Object()
                {
                    file2_1 = new AttachFileBase64Object()
                    {
                        file3 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    },
                    file2_2 = new AttachFileBase64Object()
                    {
                        file3 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    }
                },
                name = "base64_testdata_08"
            };
            public RegisterResponseModel Data8Base64RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}"
            };
            public AttachFileBase64Model Data8Base64Get = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                fileObject = new AttachFileBase64Object()
                {
                    file2_1 = new AttachFileBase64Object()
                    {
                        file3 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    },
                    file2_2 = new AttachFileBase64Object()
                    {
                        file3 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    }
                },
                name = "base64_testdata_08"
            };
            public AttachFileBase64Model Data8Base64GetFull = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~Attachfile~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                fileObject = new AttachFileBase64Object()
                {
                    file2_1 = new AttachFileBase64Object()
                    {
                        file3 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    },
                    file2_2 = new AttachFileBase64Object()
                    {
                        file3 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
                    }
                },
                name = "base64_testdata_08",
                _Version = 1,
                _partitionkey = "API~IntegratedTest~Attachfile~1",
                _Type = "API~IntegratedTest~Attachfile",
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };

            public AttachfileTestData(string resourceUrl, bool isVendor = true, bool isPerson = false, Repository repository = Repository.Default, IntegratedTestClient client = null) : 
                base(repository, resourceUrl, isVendor, isPerson, client) { }
        }

        private class AttachfilePKTestData : TestDataBase
        {
            public AttachFileBase64Model Data0Base64 = new AttachFileBase64Model()
            {
                Code = "001",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };
            public RegisterResponseModel Data0Base64RegistExpected = new RegisterResponseModel()
            {
                id = "API~IntegratedTest~AttachfilePK~1~001"
            };
            public AttachFileBase64Model Data0Base64Get = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~AttachfilePK~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                Code = "001",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };

            public AttachFileBase64Model Data0Base64Update = new AttachFileBase64Model()
            {
                Code = "001",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };
            public AttachFileBase64Model Data0Base64UpdateGet = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~AttachfilePK~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                Code = "001",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };

            public AttachFileBase64Model Data0Base64Patch = new AttachFileBase64Model()
            {
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
            };
            public AttachFileBase64Model Data0Base64PatchGet = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~AttachfilePK~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                Code = "001",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };

            public AttachFileBase64Model Data0Base64PatchAdd = new AttachFileBase64Model()
            {
                file3 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)"
            };
            public AttachFileBase64Model Data0Base64PatchAddGet = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~AttachfilePK~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                Code = "001",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file3 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };

            public AttachFileBase64ModelForUpdate Data0Base64PatchRemove = new AttachFileBase64ModelForUpdate()
            {
                file3 = null
            };
            public AttachFileBase64Model Data0Base64PatchRemoveGet = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~AttachfilePK~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                Code = "001",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };

            public AttachFileBase64Model Data0Base64RemoveRegist = new AttachFileBase64Model()
            {
                Code = "001",
                name = "hogehoge"
            };
            public AttachFileBase64Model Data0Base64RemoveRegistGet = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~AttachfilePK~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                Code = "001",
                name = "hogehoge"
            };


            public AttachFileBase64Model Data1Base64 = new AttachFileBase64Model()
            {
                Code = "002",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge2"
            };
            public AttachFileBase64Model Data1Base64Get = new AttachFileBase64Model()
            {
                id = $"API~IntegratedTest~AttachfilePK~{WILDCARD}~{WILDCARD}",
                _Owner_Id = WILDCARD,
                Code = "002",
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                file2 = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge2"
            };

            public AttachfilePKTestData(string resourceUrl, bool isVendor = false, bool isPerson = false) : base(Repository.Default, resourceUrl, isVendor, isPerson) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AttachFileNormalSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAttachfileApi>();
            var testData = new AttachfileTestData(api.ResourceUrl, true, false, repository, client);

            var testKeyValue = Guid.NewGuid().ToString();

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data0)).Assert(RegisterSuccessExpectStatusCode, testData.Data0RegistExpected);

            // メタ作成
            testData.Data1.metaList.First().metaValue = testKeyValue;
            var fileId = client.GetWebApiResponseResult(api.CreateAttachFile(testData.Data1)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // FileUpload
            client.ChunkUploadAttachFile(fileId, client.SmallContentsPath, api);

            // FileDownload
            client.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(GetSuccessExpectStatusCode, GetContentsByte(client.SmallContentsPath));

            // MetaGet
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(GetSuccessExpectStatusCode, testData.Data1MetaExpected);

            // MetaGet(X-GetInternalAllField)
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(GetSuccessExpectStatusCode, testData.Data1MetaExExpected);
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // MetaGetList
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={Guid.NewGuid()}")).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={testKeyValue}")).Assert(GetSuccessExpectStatusCode, testData.Data1MetaExpect2);

            // MetaGetList(X-GetInternalAllField)
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={Guid.NewGuid()}")).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={testKeyValue}")).Assert(GetSuccessExpectStatusCode, testData.Data1MetaExExpect2);
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // データ取得(AttachFileが混ざっていないことの確認)
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testData.Data0Get);

            // FileDelete
            client.GetWebApiResponseResult(api.DeleteAttachFile(fileId)).Assert(DeleteSuccessStatusCode);

            // MetaGet (削除確認)
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(NotFoundStatusCode);

            // FileDownload(削除確認)
            client.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(NotFoundStatusCode);

            // 削除されているIDに対するUPLOAD
            client.GetWebApiResponseResult(api.UploadAttachFile(null, fileId)).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AttachFileNormalSenarioLargeFileAndKey(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAttachfileApi>();
            var testData = new AttachfileTestData(api.ResourceUrl, true, false, repository, client);

            var testKeyValue = Guid.NewGuid().ToString();

            // メタ作成
            testData.Data2.metaList.First().metaValue = testKeyValue;
            var fileId = client.GetWebApiResponseResult(api.CreateAttachFile(testData.Data2)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // FileUpload
            client.ChunkUploadAttachFile(fileId, client.LargeContentsPath, api);

            // FileDownload           
            // Keyなし
            client.GetWebApiResponseResult(api.GetAttachFile(fileId)).AssertErrorCode(BadRequestStatusCode, "E20402");
            // Keyあり
            client.GetWebApiResponseResult(api.GetAttachFile(fileId, $"Key={testData.Data2.key}")).Assert(GetSuccessExpectStatusCode, GetContentsByte(client.LargeContentsPath));

            // MetaGet
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(GetSuccessExpectStatusCode, testData.Data2MetaExpected);

            // MetaGetList
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={testKeyValue}")).Assert(GetSuccessExpectStatusCode, testData.Data2MetaExpect2);

            // FileDeleteKeyなし
            client.GetWebApiResponseResult(api.DeleteAttachFile(fileId)).Assert(BadRequestStatusCode);

            // FileDelete
            client.GetWebApiResponseResult(api.DeleteAttachFile(fileId, $"Key={testData.Data2.key}")).Assert(DeleteSuccessStatusCode);

            // MetaGet (削除確認)
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(NotFoundStatusCode);

            // FileDownload(削除確認)
            client.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        public void AttachFileNotImpleSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAttachfileNotImpleApi>();
            var testData = new AttachfileTestData(api.ResourceUrl);

            // AttachFile無効状態の確認。すべてNotImpleになることを確認
            // メタ作成
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.Data1)).Assert(NotImplementedExpectStatusCode);
            // FileUpload
            client.GetWebApiResponseResult(api.UploadAttachFile(null, Guid.NewGuid().ToString())).Assert(NotImplementedExpectStatusCode);
            // FileDownload           
            client.GetWebApiResponseResult(api.GetAttachFile(Guid.NewGuid().ToString())).Assert(NotImplementedExpectStatusCode);
            // MetaGet
            client.GetWebApiResponseResult(api.GetAttachFileMeta(Guid.NewGuid().ToString())).Assert(NotImplementedExpectStatusCode);
            // MetaGetList
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={Guid.NewGuid()}")).Assert(NotImplementedExpectStatusCode);
            // FileDelete
            client.GetWebApiResponseResult(api.DeleteAttachFile(Guid.NewGuid().ToString())).Assert(NotImplementedExpectStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AttachFilePrivateAttachfileSenario(Repository repository)
        {
            // 個人依存のAttachFileシナリオ。他人には見えない
            var clientA = new IntegratedTestClient("test1") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2") { TargetRepository = repository };
            var api = UnityCore.Resolve<IAttachfilePrivateApi>();
            var testData = new AttachfileTestData(api.ResourceUrl, false, true, repository, clientA);

            var testKeyValue = Guid.NewGuid().ToString();

            // メタ作成
            testData.Data1.metaList.First().metaValue = testKeyValue;
            var fileIdA = clientA.GetWebApiResponseResult(api.CreateAttachFile(testData.Data1)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // FileUpload
            clientA.ChunkUploadAttachFile(fileIdA, clientA.SmallContentsPath, api);

            // FileDownload
            clientA.GetWebApiResponseResult(api.GetAttachFile(fileIdA)).Assert(GetSuccessExpectStatusCode, GetContentsByte(clientA.SmallContentsPath));
            clientB.GetWebApiResponseResult(api.GetAttachFile(fileIdA)).Assert(NotFoundStatusCode);

            // MetaGet
            clientA.GetWebApiResponseResult(api.GetAttachFileMeta(fileIdA)).Assert(GetSuccessExpectStatusCode, testData.Data1MetaExpected);
            clientB.GetWebApiResponseResult(api.GetAttachFileMeta(fileIdA)).Assert(NotFoundStatusCode);

            // MetaGetList
            clientA.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={testKeyValue}")).Assert(GetSuccessExpectStatusCode, testData.Data1MetaExpect2);
            clientB.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={testKeyValue}")).Assert(NotFoundStatusCode);

            // FileDelete
            clientB.GetWebApiResponseResult(api.DeleteAttachFile(fileIdA)).Assert(NotFoundStatusCode);
            clientA.GetWebApiResponseResult(api.DeleteAttachFile(fileIdA)).Assert(DeleteSuccessStatusCode);

            // MetaGet (削除確認)
            clientA.GetWebApiResponseResult(api.GetAttachFileMeta(fileIdA)).Assert(NotFoundStatusCode);

            // FileDownload(削除確認)
            clientA.GetWebApiResponseResult(api.GetAttachFile(fileIdA)).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AttachFileBadrequestSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAttachfileApi>();
            var testData = new AttachfileTestData(api.ResourceUrl, true, false, repository, client);

            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest1)).AssertErrorCode(BadRequestStatusCode, "E20410");
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest2)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest3)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest4)).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void AttachFileBase64_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAttachfileApi>();
            var testData = new AttachfileTestData(api.ResourceUrl);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            var reg = client.GetWebApiResponseResult(api.RegistBase64(testData.Data0Base64)).Assert(RegisterSuccessExpectStatusCode, testData.Data0Base64RegistExpected).Result;

            // ヘッダーはかえって来ない
            var result = client.GetWebApiResponseResult(api.GetAllBase64()).Assert(GetSuccessExpectStatusCode);
            result.Headers.Any(x => x.Key == "X-Base64BlobPath").IsFalse();

            var keyValuePairs = new Dictionary<string, string[]>();
            keyValuePairs.Add("X-Base64BlobPath", new string[] { $"*attachfilebase64/{reg.id}/file" });

            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            if (IsIgnoreGetInternalAllField)
            {
                // ヘッダーはかえって来ない
                client.GetWebApiResponseResult(api.GetAllBase64()).Assert(GetSuccessExpectStatusCode, testData.Data0Base64Get);
            }
            else
            {
                // ヘッダーはかえって来る
                result = client.GetWebApiResponseResult(api.GetAllBase64()).Assert(GetSuccessExpectStatusCode, testData.Data0Base64GetFull, keyValuePairs);
            }

            // もう1件登録
            var reg2 = client.GetWebApiResponseResult(api.RegistBase64(testData.Data2Base64)).Assert(RegisterSuccessExpectStatusCode, testData.Data2Base64RegistExpected).Result;

            var keyValuePairs2 = new Dictionary<string, string[]>();
            keyValuePairs2.Add("X-Base64BlobPath", new string[] { $"*attachfilebase64/{reg.id}/file", $"*attachfilebase64/{reg2.id}/file" });

            if (IsIgnoreGetInternalAllField)
            {
                // ヘッダーはかえって来ない
                client.GetWebApiResponseResult(api.GetAllBase64()).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data0Base64Get.First(), testData.Data2Base64Get });
            }
            else
            {
                // ヘッダーはかえって来る
                client.GetWebApiResponseResult(api.GetAllBase64()).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data0Base64GetFull.First(), testData.Data2Base64GetFull }, keyValuePairs2, ',');
            }

            // ODataで取得
            if (IsIgnoreGetInternalAllField)
            {
                // ヘッダーはかえって来ない
                client.GetWebApiResponseResult(api.ODataBase64()).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data0Base64Get.First(), testData.Data2Base64Get });
            }
            else
            {
                // ヘッダーはかえって来る
                client.GetWebApiResponseResult(api.ODataBase64()).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data0Base64GetFull.First(), testData.Data2Base64GetFull }, keyValuePairs2, ',');
            }

            // 2MB以上のデータを登録
            var bigBase64 = new AttachFileBase64Model()
            {
                file = $"$Base64({Convert.ToBase64String(GetContentsByte(client.LargeContentsPath))})",
                name = "base64_testdata_07"
            };
            var reg7 = client.GetWebApiResponseResult(api.RegistBase64(bigBase64)).Assert(RegisterSuccessExpectStatusCode).Result;
            var result7 = client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_07'")).Assert(GetSuccessExpectStatusCode);

            // でかいサイズのAssertは時間かかる時があるので直接比較
            result7.RawContentString.ToJson().First().Value<string>("file").Is(bigBase64.file);
        }

        [TestMethod]
        public void AttachFileBase64_ArrayDataSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAttachfileApi>();
            var testData = new AttachfileTestData(api.ResourceUrl);

            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            var reg = client.GetWebApiResponseResult(api.RegistListBase64(new List<AttachFileBase64Model>() { testData.Data0Base64, testData.Data2Base64 })).Assert(RegisterSuccessExpectStatusCode).Result;

            var keyValuePairs = new Dictionary<string, string[]>();
            keyValuePairs.Add("X-Base64BlobPath", new string[] { $"*attachfilebase64/{reg[0].id}/file", $"*attachfilebase64/{reg[1].id}/file" });

            if (IsIgnoreGetInternalAllField)
            {
                client.GetWebApiResponseResult(api.GetAllBase64()).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data0Base64Get.First(), testData.Data2Base64Get });
            }
            else
            {
                client.GetWebApiResponseResult(api.GetAllBase64()).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data0Base64GetFull.First(), testData.Data2Base64GetFull }, keyValuePairs, ',');
            }

            // オブジェクト配列データ登録
            var reg4 = client.GetWebApiResponseResult(api.RegistBase64(testData.Data4Base64)).Assert(RegisterSuccessExpectStatusCode).Result;
            
            var  keyValuePairs4 = new Dictionary<string, string[]>();
            keyValuePairs4.Add("X-Base64BlobPath", new string[] { $"*attachfilebase64/{reg4.id}/fileObjects[0].file_01", $"*attachfilebase64/{reg4.id}/fileObjects[1].file_02" });
            
            if (IsIgnoreGetInternalAllField)
            {
                client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_04'")).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data4Base64Get });
            }
            else
            {
                client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_04'")).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data4Base64GetFull }, keyValuePairs4, ',');
            }

            // 配列データ登録
            var reg5 = client.GetWebApiResponseResult(api.RegistBase64(testData.Data5Base64)).Assert(RegisterSuccessExpectStatusCode).Result;
            
            var keyValuePairs5 = new Dictionary<string, string[]>();
            keyValuePairs5.Add("X-Base64BlobPath", new string[] { $"*attachfilebase64/{reg5.id}/files[0]", $"*attachfilebase64/{reg5.id}/files[1]" });
            
            if (IsIgnoreGetInternalAllField)
            {
                client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_05'")).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data5Base64Get });
            }
            else
            {
                client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_05'")).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data5Base64GetFull }, keyValuePairs5, ',');
            }

            // 配列データ登録
            var reg6 = client.GetWebApiResponseResult(api.RegistBase64(testData.Data6Base64)).Assert(RegisterSuccessExpectStatusCode).Result;

            var keyValuePairs6 = new Dictionary<string, string[]>();
            keyValuePairs6.Add("X-Base64BlobPath", new string[] { $"*attachfilebase64/{reg6.id}/file_01", $"*attachfilebase64/{reg6.id}/file_02" });

            if (IsIgnoreGetInternalAllField)
            {
                client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_06'")).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data6Base64Get });
            }
            else
            {
                client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_06'")).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data6Base64GetFull }, keyValuePairs6, ',');
            }

            // オブジェクト配列データ登録
            var reg8 = client.GetWebApiResponseResult(api.RegistBase64(testData.Data8Base64)).Assert(RegisterSuccessExpectStatusCode).Result;
            
            var keyValuePairs8 = new Dictionary<string, string[]>();
            keyValuePairs8.Add("X-Base64BlobPath", new string[] { $"*attachfilebase64/{reg8.id}/fileObject.file2_1.file3", $"*attachfilebase64/{reg8.id}/fileObject.file2_2.file3" });

            if (IsIgnoreGetInternalAllField)
            {
                client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_08'")).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data8Base64Get });
            }
            else
            {
                client.GetWebApiResponseResult(api.ODataBase64("$filter=name eq 'base64_testdata_08'")).Assert(GetSuccessExpectStatusCode, new List<AttachFileBase64Model>() { testData.Data8Base64GetFull }, keyValuePairs8, ',');
            }
        }

        [TestMethod]
        public void AttachFileBase64_CacheSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAttachfileApi>();
            var testData = new AttachfileTestData(api.ResourceUrl);

            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            var reg = client.GetWebApiResponseResult(api.RegistListBase64(new List<AttachFileBase64Model>() { testData.Data0Base64, testData.Data2Base64 })).Assert(RegisterSuccessExpectStatusCode).Result;

            var keyValuePairs = new Dictionary<string, string[]>();
            keyValuePairs.Add("X-Base64BlobPath", new string[] { $"*attachfilebase64/{reg[0].id}/file", $"*attachfilebase64/{reg[1].id}/file" });

            if (IsIgnoreGetInternalAllField)
            {
                var expectedList = new List<AttachFileBase64Model>() { testData.Data0Base64Get.First(), testData.Data2Base64Get };
                client.GetWebApiResponseResult(api.GetAllCache()).Assert(GetSuccessExpectStatusCode, expectedList);
                // 普通に取れること
                client.GetWebApiResponseResult(api.GetAllCache()).Assert(GetSuccessExpectStatusCode, expectedList);
            }
            else
            {
                var expectedList = new List<AttachFileBase64Model>() { testData.Data0Base64GetFull.First(), testData.Data2Base64GetFull };
                client.GetWebApiResponseResult(api.GetAllCache()).Assert(GetSuccessExpectStatusCode, expectedList, keyValuePairs, ',');
                // 普通に取れること
                client.GetWebApiResponseResult(api.GetAllCache()).Assert(GetSuccessExpectStatusCode, expectedList, keyValuePairs, ',');
            }
        }

        [TestMethod]
        public void AttachFileBase64_BadrequestSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAttachfileApi>();
            var testData = new AttachfileTestData(api.ResourceUrl);

            // 壊れたBase64
            client.GetWebApiResponseResult(api.RegistBase64(testData.Data3Base64)).Assert(RegisterErrorExpectStatusCode);

            // 8MB越えデータ登録
            var bigBase64 = new AttachFileBase64Model()
            {
                file = $"$Base64({GetContentsByte(client.LargeContentsPath)})",
                name = "base64_testdata_07"
            };
            client.GetWebApiResponseResult(api.RegistBase64(bigBase64)).Assert(RegisterErrorExpectStatusCode);

            // 添付ファイル未設定APIに対して
            var apiNotImpl = UnityCore.Resolve<IAttachfileNotImpleApi>();
            client.GetWebApiResponseResult(apiNotImpl.RegistBase64(testData.Data0Base64)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10403");
        }

        [TestMethod]
        public void AttachFileBase64PK_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAttachfileApiPK>();
            var testData = new AttachfilePKTestData(api.ResourceUrl);

            //データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data0Base64)).Assert(RegisterSuccessExpectStatusCode, testData.Data0Base64RegistExpected);
            client.GetWebApiResponseResult(api.Get("001")).Assert(GetSuccessExpectStatusCode, testData.Data0Base64Get);

            // 更新対象ではないデータを登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1Base64)).Assert(RegisterSuccessExpectStatusCode);

            // データ1をPostで上書き(添付ファイルの入れ替え)
            client.GetWebApiResponseResult(api.Regist(testData.Data0Base64Update)).Assert(RegisterSuccessExpectStatusCode, testData.Data0Base64RegistExpected);
            client.GetWebApiResponseResult(api.Get("001")).Assert(GetSuccessExpectStatusCode, testData.Data0Base64UpdateGet);

            // データを部分更新
            client.GetWebApiResponseResult(api.Update("001", testData.Data0Base64Patch)).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.Get("001")).Assert(GetSuccessExpectStatusCode, testData.Data0Base64PatchGet);
            // データを追記
            client.GetWebApiResponseResult(api.Update("001", testData.Data0Base64PatchAdd)).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.Get("001")).Assert(GetSuccessExpectStatusCode, testData.Data0Base64PatchAddGet);
            // データを部分削除
            client.GetWebApiResponseResult(api.Update("001", testData.Data0Base64PatchRemove)).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.Get("001")).Assert(GetSuccessExpectStatusCode, testData.Data0Base64PatchRemoveGet);
            // Postで添付データを削除
            client.GetWebApiResponseResult(api.Regist(testData.Data0Base64RemoveRegist)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.Get("001")).Assert(GetSuccessExpectStatusCode, testData.Data0Base64RemoveRegistGet);

            // 他のデータが更新されていないことを確認
            client.GetWebApiResponseResult(api.Get("002")).Assert(GetSuccessExpectStatusCode, testData.Data1Base64Get);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AttachFileOptimisticConcurrencyNormalSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAttachfileOptimisticConcurrencyApi>();
            var testData = new AttachfileTestData(api.ResourceUrl);

            // メタ作成
            var fileId = client.GetWebApiResponseResult(api.CreateAttachFile(testData.Data1)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // FileUpload
            client.ChunkUploadAttachFile(fileId, client.SmallContentsPath, api);

            // FileDownload           
            client.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(GetSuccessExpectStatusCode, GetContentsByte(client.SmallContentsPath));

            // FileDelete
            client.GetWebApiResponseResult(api.DeleteAttachFile(fileId)).Assert(DeleteSuccessStatusCode);

            // MetaGet (削除確認)
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(NotFoundStatusCode);

            // FileDownload(削除確認)
            client.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(NotFoundStatusCode);
        }
    }
}
