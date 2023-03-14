using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_DocumentDbPartitionKey : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        public const string VendorId = "e85825d7-032e-4574-b3fa-90c4f6217243";
        public const string SystemId = "e85825d7-032e-4574-b3fa-90c4f6217243";
        public const string OpenId = "e85825d7-032e-4574-b3fa-90c4f6217243";

        #region  CreateQueryPartition

        [TestMethod]
        public void CreateQueryPartition_正常系_パーティションキー複数指定_依存なし()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var result = DocumentDbPartitionKey.CreateQueryPartition(new PartitionKey("/API/ParKey/{key1}/{key2}/{key3}"),
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(false), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(false), new OpenId(OpenId), new ResourceVersion(1), qs, up, new IsOverPartition(false),
                out var outputDbPartitionKey);
            result.IsTrue();
            outputDbPartitionKey.Value.Is("API~ParKey~1~value1~value2~value3");

        }

        [TestMethod]
        public void CreateQueryPartition_正常系_レポジトリキー複数指定_依存なし()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var result = DocumentDbPartitionKey.CreateQueryPartition(null,
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(false), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(false), new OpenId(OpenId), new ResourceVersion(1), qs, up, new IsOverPartition(false),
                out var outputDbPartitionKey);
            result.IsTrue();
            outputDbPartitionKey.Value.Is("API~Repokey~1");

        }

        [TestMethod]
        public void CreateQueryPartition_正常系_パーティションキー複数指定_ベンダー依存()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var result = DocumentDbPartitionKey.CreateQueryPartition(new PartitionKey("/API/ParKey/{key1}/{key2}/{key3}"),
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(true), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(false), new OpenId(OpenId), new ResourceVersion(1), qs, up, new IsOverPartition(false),
                out var outputDbPartitionKey);
            result.IsTrue();
            outputDbPartitionKey.Value.Is($"API~ParKey~{VendorId}~{SystemId}~1~value1~value2~value3");

        }

        [TestMethod]
        public void CreateQueryPartition_正常系_レポジトリキー複数指定_ベンダー依存()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var result = DocumentDbPartitionKey.CreateQueryPartition(new PartitionKey(""),
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(true), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(false), new OpenId(OpenId), new ResourceVersion(1), qs, up, new IsOverPartition(false),
                out var outputDbPartitionKey);
            result.IsTrue();
            outputDbPartitionKey.Value.Is($"API~Repokey~{VendorId}~{SystemId}~1");

        }

        [TestMethod]
        public void CreateQueryPartition_正常系_パーティションキー複数指定_個人依存()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var result = DocumentDbPartitionKey.CreateQueryPartition(new PartitionKey("/API/ParKey/{key1}/{key2}/{key3}"),
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(false), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(true), new OpenId(OpenId), new ResourceVersion(1), qs, up, new IsOverPartition(false),
                out var outputDbPartitionKey);
            result.IsTrue();
            outputDbPartitionKey.Value.Is($"API~ParKey~{OpenId}~1~value1~value2~value3");

        }

        [TestMethod]
        public void CreateQueryPartition_正常系_レポジトリキー複数指定_個人依存()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var result = DocumentDbPartitionKey.CreateQueryPartition(new PartitionKey(""),
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(false), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(true), new OpenId(OpenId), new ResourceVersion(1), qs, up, new IsOverPartition(false),
                out var outputDbPartitionKey);
            result.IsTrue();
            outputDbPartitionKey.Value.Is($"API~Repokey~{OpenId}~1");

        }

        [TestMethod]
        public void CreateQueryPartition_異常系_引数がnull()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var result = DocumentDbPartitionKey.CreateQueryPartition(null,
                null, null, null, null,
                null, null, null, null, null, null,
                out var outputDbPartitionKey);
            result.IsFalse();
            outputDbPartitionKey.IsNull();
        }

        #endregion

        #region  CreateRegisterPartition

        [TestMethod]
        public void CreateRegisterPartition_正常系_パーティションキー複数指定_依存なし()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var input = JToken.Parse(@"
{
    'key1' : 'hoge',
    'key2' : 'fuga',
    'key3' : 'piyo',
    'value' : 'value'
}");

            var result = DocumentDbPartitionKey.CreateRegisterPartition(new PartitionKey("/API/ParKey/{key1}/{key2}/{key3}"),
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(false), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(false), new OpenId(OpenId), new ResourceVersion(1), input);
            result.Value.Is("API~ParKey~1~hoge~fuga~piyo");
        }

        [TestMethod]
        public void CreateRegisterPartition_正常系_レポジトリキー複数指定_依存なし()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var input = JToken.Parse(@"
{
    'key1' : 'hoge',
    'key2' : 'fuga',
    'key3' : 'piyo',
    'value' : 'value'
}");

            var result = DocumentDbPartitionKey.CreateRegisterPartition(null,
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(false), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(false), new OpenId(OpenId), new ResourceVersion(1), input);
            result.Value.Is("API~Repokey~1");
        }

        [TestMethod]
        public void CreateRegisterPartition_正常系_パーティションキー複数指定_ベンダー依存()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var input = JToken.Parse(@"
{
    'key1' : 'hoge',
    'key2' : 'fuga',
    'key3' : 'piyo',
    'value' : 'value'
}");

            var result = DocumentDbPartitionKey.CreateRegisterPartition(new PartitionKey("/API/ParKey/{key1}/{key2}/{key3}"),
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(true), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(false), new OpenId(OpenId), new ResourceVersion(1), input);
            result.Value.Is($"API~ParKey~{VendorId}~{SystemId}~1~hoge~fuga~piyo");
        }

        [TestMethod]
        public void CreateRegisterPartition_正常系_レポジトリキー複数指定_ベンダー依存()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var input = JToken.Parse(@"
{
    'key1' : 'hoge',
    'key2' : 'fuga',
    'key3' : 'piyo',
    'value' : 'value'
}");

            var result = DocumentDbPartitionKey.CreateRegisterPartition(null,
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(true), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(false), new OpenId(OpenId), new ResourceVersion(1), input);
            result.Value.Is($"API~Repokey~{VendorId}~{SystemId}~1");
        }

        [TestMethod]
        public void CreateRegisterPartition_正常系_パーティションキー複数指定_個人依存()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var input = JToken.Parse(@"
{
    'key1' : 'hoge',
    'key2' : 'fuga',
    'key3' : 'piyo',
    'value' : 'value'
}");

            var result = DocumentDbPartitionKey.CreateRegisterPartition(new PartitionKey("/API/ParKey/{key1}/{key2}/{key3}"),
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(false), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(true), new OpenId(OpenId), new ResourceVersion(1), input);
            result.Value.Is($"API~ParKey~{OpenId}~1~hoge~fuga~piyo");
        }

        [TestMethod]
        public void CreateRegisterPartition_正常系_レポジトリキー複数指定_個人依存()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            var input = JToken.Parse(@"
{
    'key1' : 'hoge',
    'key2' : 'fuga',
    'key3' : 'piyo',
    'value' : 'value'
}");

            var result = DocumentDbPartitionKey.CreateRegisterPartition(null,
                new RepositoryKey("/API/Repokey/{key1}/{key2}/{key3}"), new IsVendor(false), new VendorId(VendorId), new SystemId(SystemId),
                new IsPerson(true), new OpenId(OpenId), new ResourceVersion(1), input);
            result.Value.Is($"API~Repokey~{OpenId}~1");
        }

        [TestMethod]
        public void CreateRegisterPartition_異常系_引数がnull()
        {
            var qs = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                {new QueryStringKey("key1"), new QueryStringValue("value1") },
                {new QueryStringKey("key2"), new QueryStringValue("value2") },
                {new QueryStringKey("key3"), new QueryStringValue("value3") }
            });
            var up = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                {new UrlParameterKey("key1"), new UrlParameterValue("value1") },
                {new UrlParameterKey("key2"), new UrlParameterValue("value2") },
                {new UrlParameterKey("key3"), new UrlParameterValue("value3") }
            });

            AssertEx.Catch<NullReferenceException>(() =>
            {
                var result = DocumentDbPartitionKey.CreateRegisterPartition(null,
                    null, null, null, null,
                    null, null, null, null);
            });
        }

        #endregion
    }
}
