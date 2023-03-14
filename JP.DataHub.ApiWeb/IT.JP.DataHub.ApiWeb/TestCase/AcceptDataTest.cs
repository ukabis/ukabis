using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class AcceptDataTest : ApiWebItTestCase
    {
        #region TestData

        private class AcceptDataTestData : TestDataBase
        {
            public AcceptDataModel Data1 = new AcceptDataModel()
            {
                id = "hogehoge",
                Code = "AA",
                Name = "aaa"
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Accept~1~AA"
            };
            public AcceptDataModel Data1Get = new AcceptDataModel()
            {
                id = "API~IntegratedTest~Accept~1~AA",
                Code = "AA",
                Name = "aaa"
            };
            public string Data1GetXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<root  xmlns:dh=""http://example.com/XMLSchema-instance/"">
<Code>AA</Code>
<id>API~IntegratedTest~Accept~1~AA</id>
<Name>aaa</Name>
</root>";

            public string Data1GetCsv = $@"Code,id,Name
AA,API~IntegratedTest~Accept~1~AA,aaa
";

            public string Data2Xml = @"<?xml version=""1.0"" encoding=""utf-8""?><root xmlns:dh=""http://example.com/XMLSchema-instance/""><Code>BB</Code><Name>hoge</Name></root>";
            public string Data2RegistExpectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?><root xmlns:dh=""http://example.com/XMLSchema-instance/""><id>API~IntegratedTest~Accept~1~BB</id></root>";

            public string Data3RegistExpectedXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<root  xmlns:dh=""http://example.com/XMLSchema-instance/"" dh:anonymous_array=""true"">
<item dh:array=""true"">
<Code>AA</Code>
<id>API~IntegratedTest~Accept~1~AA</id>
<Name>aaa</Name>
</item>
<item dh:array=""true"">
<Code>BB</Code>
<id>API~IntegratedTest~Accept~1~BB</id>
<Name>hoge</Name>
</item>
<item dh:array=""true"">
<Code>CC</Code>
<id>API~IntegratedTest~Accept~1~CC</id>
<Name>aaa</Name>
</item>
<item dh:array=""true"">
<Code>DD</Code>
<id>API~IntegratedTest~Accept~1~DD</id>
<Name>hoge</Name>
</item>
<item dh:array=""true"">
<Code>EE</Code>
<id>API~IntegratedTest~Accept~1~EE</id>
<Name>ee</Name>
</item>
<item dh:array=""true"">
<Code>FF</Code>
<id>API~IntegratedTest~Accept~1~FF</id>
<Name>ff</Name>
</item>
<item dh:array=""true"">
<Code>GG</Code>
<id>API~IntegratedTest~Accept~1~GG</id>
<Name>gg</Name>
</item>
</root>";

            public string Data4Xml = @"<?xml version=""1.0"" encoding=""utf-8""?><root  xmlns:dh=""http://example.com/XMLSchema-instance/""><Name>update</Name></root>";
            public string Data4GetXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<root  xmlns:dh=""http://example.com/XMLSchema-instance/"">
<Code>AA</Code>
<id>API~IntegratedTest~Accept~1~AA</id>
<Name>update</Name>
</root>";

            public string Data5Xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root  xmlns:dh=""http://example.com/XMLSchema-instance/"" dh:anonymous_array=""true"">
<item dh:array=""true"">
<Code>CC</Code>
<Name>aaa</Name>
</item>
<item dh:array=""true"">
<Code>DD</Code>
<Name>hoge</Name>
</item>
</root>";
            public string Data5RegistExpectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com/XMLSchema-instance/"" dh:anonymous_array=""true"">
<item dh:array = ""true"">
<id>API~IntegratedTest~Accept~1~CC</id>
</item>
<item dh:array = ""true"" >
<id>API~IntegratedTest~Accept~1~DD</id>
</item>
</root>"
;

            public string Data6Xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root  xmlns:dh=""http://example.com/XMLSchema-instance/"" dh:anonymous_array=""true"">
<item dh:array=""true"">
<Code>CC</Code>
<Name>aaa</Name>
<Number dh:type=""number"">100.999</Number>
</item>
<item dh:array=""true"">
<Code>DD</Code>
<Name>hoge</Name>
<Number dh:type=""number"">100</Number>
</item>
</root>";
            public string Data6RegistExpectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root  xmlns:dh=""http://example.com/XMLSchema-instance/"" dh:anonymous_array=""true"">
<item dh:array = ""true"">
<id>API~IntegratedTest~Accept~1~CC</id>
</item>
<item dh:array = ""true"" >
<id>API~IntegratedTest~Accept~1~DD</id>
</item>
</root>"
    ;
            public string Data6GetXml1 = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com/XMLSchema-instance/"">
<Code>CC</Code>
<id>API~IntegratedTest~Accept~1~CC</id>
<Name>aaa</Name>
<Number dh:type=""number"">100.999</Number>
</root>";

            public string Data6GetXml2 = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com/XMLSchema-instance/"">
<Code>DD</Code>
<id>API~IntegratedTest~Accept~1~DD</id>
<Name>hoge</Name>
<Number dh:type=""number"">100</Number>
</root>";
            public AcceptDataModel Data6Get = new AcceptDataModel()
            {
                id = "API~IntegratedTest~Accept~1~CC",
                Code = "CC",
                Name = "aaa",
                Number = 100.999m

            };

            public List<AcceptDataModel> Data7 = new List<AcceptDataModel>()
            {
                new AcceptDataModel()
                {
                    Name = "aaa",
                    Number = 100.999m
                }
            };
            public string Data7RegistExpected = @"""Code"":[""Required properties are missing from object: Code.(code:14)""]}";

            public string Data8Xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com/XMLSchema-instance/"" dh:anonymous_array=""true"">
<item dh:array=""true"">
<Name>aaa</Name>
<Number dh:type=""number"">100.999</Number>
</item>
</root>";
            public string Data8RegistExpected = @"<Code dh:array=""true"">Required properties are missing from object: Code.(code:14)</Code>";

            public List<AcceptDataModel> Data9 = new List<AcceptDataModel>()
            {
                new AcceptDataModel()
                {
                    Code = "XX",
                    Number = 100.999m
                }
            };
            public string Data9RegistExpected = @"{""Name"":[""Required properties are missing from object: Name.(code:14)""]}";

            public string Data10Xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root  xmlns:dh=""http://example.com/XMLSchema-instance/"" dh:anonymous_array=""true"">
<item dh:array=""true"">
<Code>XX</Code>
<Number dh:type=""number"">100.999</Number>
</item>
</root>";
            public string Data10RegistExpected = @"<Name dh:array=""true"">Required properties are missing from object: Name.(code:14)</Name>";

            public AcceptDataModel Data11 = new AcceptDataModel()
            {
                Code = "AA",
                Name = "aaa",
                AR = new List<AcceptDataArrayItem>()
                {
                    new AcceptDataArrayItem()
                    {
                        ar1 = "hoge"
                    },
                    new AcceptDataArrayItem()
                    {
                        ar2 = "fuga"
                    }
                },
                AR2 = new List<int>() { 1, 2, 3 }
            };
            public RegisterResponseModel Data11RegistExpected = new RegisterResponseModel()
            {
                id = "API~IntegratedTest~Accept~1~AA"
            };
            public AcceptDataModel Data11Get = new AcceptDataModel()
            {
                id = "API~IntegratedTest~Accept~1~AA",
                Code = "AA",
                Name = "aaa",
                AR = new List<AcceptDataArrayItem>()
                {
                    new AcceptDataArrayItem()
                    {
                        ar1 = "hoge"
                    },
                    new AcceptDataArrayItem()
                    {
                        ar2 = "fuga"
                    }
                },
                AR2 = new List<int>() { 1, 2, 3 }
            };
            public string Data11GetXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com/XMLSchema-instance/"">
<Code>AA</Code>
<id>API~IntegratedTest~Accept~1~AA</id>
<Name>aaa</Name>
<AR dh:array=""true""><ar1>hoge</ar1></AR>
<AR dh:array= ""true""><ar2>fuga</ar2></AR>
<AR2 dh:type=""number"" dh:array=""true"">1</AR2>
<AR2 dh:type=""number"" dh:array=""true"">2</AR2>
<AR2 dh:type=""number"" dh:array=""true"">3</AR2>
</root>";

            public string Data12Xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com/XMLSchema-instance/"">
<Code>BB</Code>
<Name>aaa</Name>
<AR dh:array=""true""><ar1>hoge</ar1></AR>
<AR dh:array= ""true""><ar2>fuga</ar2></AR>
<AR2 dh:type=""number"" dh:array=""true"">1</AR2>
<AR2 dh:type=""number"" dh:array=""true"">2</AR2>
<AR2 dh:type=""number"" dh:array=""true"">3</AR2>
</root>";
            public string Data12RegistExpectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com/XMLSchema-instance/"">
<id>API~IntegratedTest~Accept~1~BB</id>
</root>";
            public AcceptDataModel Data12Get = new AcceptDataModel()
            {
                id = "API~IntegratedTest~Accept~1~BB",
                Code = "BB",
                Name = "aaa",
                AR = new List<AcceptDataArrayItem>()
                {
                    new AcceptDataArrayItem()
                    {
                        ar1 = "hoge"
                    },
                    new AcceptDataArrayItem()
                    {
                        ar2 = "fuga"
                    }
                },
                AR2 = new List<int>() { 1, 2, 3 }
            };
            public string Data12GetXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com/XMLSchema-instance/"">
<Code>BB</Code>
<id>API~IntegratedTest~Accept~1~BB</id>
<Name>aaa</Name>
<AR dh:array=""true""><ar1>hoge</ar1></AR>
<AR dh:array= ""true""><ar2>fuga</ar2></AR>
<AR2 dh:type=""number"" dh:array=""true"">1</AR2>
<AR2 dh:type=""number"" dh:array=""true"">2</AR2>
<AR2 dh:type=""number"" dh:array=""true"">3</AR2>
</root>";

            public string Data13Csv = @"Code,Name
EE,ee
";
            public string Data13RegistExpectedCsv = @"id
API~IntegratedTest~Accept~1~EE
";

            public string Data14Csv = @"Code,Name
FF,ff
GG,gg
";
            public string Data14RegistExpectedCsv = @"id
API~IntegratedTest~Accept~1~FF
API~IntegratedTest~Accept~1~GG
";

            public string Data15RegistExpectedCsv = @"Code,id,Name
AA,API~IntegratedTest~Accept~1~AA,aaa
BB,API~IntegratedTest~Accept~1~BB,hoge
CC,API~IntegratedTest~Accept~1~CC,aaa
DD,API~IntegratedTest~Accept~1~DD,hoge
EE,API~IntegratedTest~Accept~1~EE,ee
FF,API~IntegratedTest~Accept~1~FF,ff
GG,API~IntegratedTest~Accept~1~GG,gg
";

            public string Data16Csv = @"Name
update_csv";

            public string Data16GetCsv = @"Code,id,Name
AA,API~IntegratedTest~Accept~1~AA,update_csv
";

            public string Data18Csv = @"Code,Name,Number,Integer,Boolean
AA,aa,1.1,1,True
BB,bb,-2.2,-2,False
";
            public string Data18RegistExpectedCsv = @"id
API~IntegratedTest~Accept~1~AA
API~IntegratedTest~Accept~1~BB
";
            public string Data18GetCsv1 = @"Code,id,Name,Number,Integer,Boolean
AA,API~IntegratedTest~Accept~1~AA,aa,1.1,1,True
";
            public string Data18GetCsv2 = @"Code,id,Name,Number,Integer,Boolean
BB,API~IntegratedTest~Accept~1~BB,bb,-2.2,-2,False
";
            public AcceptDataModel Data18Get = new AcceptDataModel()
            {
                id = "API~IntegratedTest~Accept~1~AA",
                Code = "AA",
                Name = "aa",
                Number = 1.1m,
                Integer = 1,
                Boolean = true
            };

            public string Data19Csv = @"Code,Name,Number
AA,put_csv,1";

            public string Data19GetCsv = @"Code,id,Name,Number
AA,API~IntegratedTest~Accept~1~AA,put_csv,1
";
            public AcceptDataModel Data19Get = new AcceptDataModel()
            {
                id = "API~IntegratedTest~Accept~1~AA",
                Code = "AA",
                Name = "put_csv",
                Number = 1
            };

            public List<AcceptDataModel> Data20 = new List<AcceptDataModel>()
            {
                new AcceptDataModel()
                {
                    Code = "AA",
                    Name = "aa",
                    AR = new List<AcceptDataArrayItem>()
                    {
                        new AcceptDataArrayItem()
                        {
                            ar1 = "hoge"
                        },
                        new AcceptDataArrayItem()
                        {
                            ar2 = "fuga"
                        }
                    }
                },
                new AcceptDataModel()
                {
                    Code = "BB",
                    Name = "bb",
                    AR2 = new List<int>() { 1, 2, 3 }
                }
            };
            public List<RegisterResponseModel> Data20RegistExpected = new List<RegisterResponseModel>()
            { 
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~Accept~1~AA"
                },
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~Accept~1~BB"
                }
            };
            public AcceptDataModel Data20_1Get = new AcceptDataModel()
            {
                id = "API~IntegratedTest~Accept~1~AA",
                Code = "AA",
                Name = "aa",
                AR = new List<AcceptDataArrayItem>()
                {
                    new AcceptDataArrayItem()
                    {
                        ar1 = "hoge"
                    },
                    new AcceptDataArrayItem()
                    {
                        ar2 = "fuga"
                    }
                }
            };
            public AcceptDataModel Data20_2Get = new AcceptDataModel()
            {
                id = "API~IntegratedTest~Accept~1~BB",
                Code = "BB",
                Name = "bb",
                AR2 = new List<int>() { 1, 2, 3 }
            };

            public string Data21Csv = @"Code,Name,Number
AA,aa,abc
";
            public string Data21RegistExpected = @"{""Number"":[""Invalid type. Expected Number but got String.(code:18)""]}";

            public List<AcceptDataModel> Data22 = new List<AcceptDataModel>()
            {
                new AcceptDataModel()
                {
                    id = "API~IntegratedTest~Accept~1~HH",
                    Code = "HH",
                    Name = "hh",
                    NumberOrNull = 8.8m,
                    IntegerOrNull = 8
                },
                new AcceptDataModel()
                {
                    id = "API~IntegratedTest~Accept~1~II",
                    Code = "II",
                    Name = "ii",
                    NumberOrNull = 9.9m,
                    IntegerOrNull = null
                }
            };
            public string Data22GetCsv = @"Code,id,Name,Number,NumberOrNull,IntegerOrNull
AA,API~IntegratedTest~Accept~1~AA,put_csv,1,,
BB,API~IntegratedTest~Accept~1~BB,hoge,,,
CC,API~IntegratedTest~Accept~1~CC,aaa,,,
DD,API~IntegratedTest~Accept~1~DD,hoge,,,
EE,API~IntegratedTest~Accept~1~EE,ee,,,
FF,API~IntegratedTest~Accept~1~FF,ff,,,
GG,API~IntegratedTest~Accept~1~GG,gg,,,
HH,API~IntegratedTest~Accept~1~HH,hh,,8.8,8
II,API~IntegratedTest~Accept~1~II,ii,,9.9,
";

        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void AcceptDataTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAcceptDataApi>();
            var testData = new AcceptDataTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 登録した１件を取得(Json)
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // 登録した１件を取得(Xml)
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/xml" };
            var getResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode);
            getResponse.RawContentString.StringToXml().Is(testData.Data1GetXml.StringToXml());

            // 登録した１件を取得(csv)
            api.AddHeaders[HeaderConst.Accept] = new string[] { "text/csv" };
            getResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode);
            getResponse.RawContentString.Is(testData.Data1GetCsv);

            // データ1を１件登録(XML)
            api.AddHeaders.Remove(HeaderConst.Accept);
            var reqRegXml = api.RegistAsText(testData.Data2Xml);
            reqRegXml.MediaType = "application/xml";
            var regResponse = client.GetWebApiResponseResult(reqRegXml).Assert(RegisterSuccessExpectStatusCode);
            regResponse.RawContentString.StringToXml().Is(testData.Data2RegistExpectedXml.StringToXml());

            // 複数件登録(XML)
            var reqRegXmlList = api.RegistListAsText(testData.Data5Xml);
            reqRegXmlList.MediaType = "application/xml";
            var regListResponse = client.GetWebApiResponseResult(reqRegXmlList).Assert(RegisterSuccessExpectStatusCode);
            regListResponse.RawContentString.StringToXml().Is(testData.Data5RegistExpectedXml.StringToXml());

            // データ1を１件登録(CSV)
            var reqRegCsv = api.RegistAsText(testData.Data13Csv);
            reqRegCsv.MediaType = "text/csv";
            regResponse = client.GetWebApiResponseResult(reqRegCsv).Assert(RegisterSuccessExpectStatusCode);
            regResponse.RawContentString.Is(testData.Data13RegistExpectedCsv);

            // 複数件登録(CSV)
            var reqRegCsvList = api.RegistListAsText(testData.Data14Csv);
            reqRegCsvList.MediaType = "text/csv";
            regListResponse = client.GetWebApiResponseResult(reqRegCsvList).Assert(RegisterSuccessExpectStatusCode);
            regListResponse.RawContentString.Is(testData.Data14RegistExpectedCsv);

            // 登録した7件(配列)をXMLで取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/xml" };
            var getListResponse = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode);
            getListResponse.RawContentString.StringToXml().Is(testData.Data3RegistExpectedXml.StringToXml());

            // 登録した7件(配列)をCSVで取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "text/csv" };
            getListResponse = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode);
            getListResponse.RawContentString.Is(testData.Data15RegistExpectedCsv);

            // Update(XML)
            api.AddHeaders.Remove(HeaderConst.Accept);
            var reqUpdXml = api.UpdateAsText("AA", testData.Data4Xml);
            reqUpdXml.MediaType = "application/xml";
            client.GetWebApiResponseResult(reqUpdXml).Assert(UpdateSuccessExpectStatusCode);

            // Updateしたデータを取得(XML)
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/xml" };
            var updResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode);
            updResponse.RawContentString.StringToXml().Is(testData.Data4GetXml.StringToXml());

            // Update(CSV)
            api.AddHeaders.Remove(HeaderConst.Accept);
            var reqUpdCsv = api.UpdateAsText("AA", testData.Data16Csv);
            reqUpdCsv.MediaType = "text/csv";
            client.GetWebApiResponseResult(reqUpdCsv).Assert(UpdateSuccessExpectStatusCode);

            // Updateしたデータを取得(CSV)
            api.AddHeaders[HeaderConst.Accept] = new string[] { "text/csv" };
            updResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode);
            updResponse.RawContentString.Is(testData.Data16GetCsv);

            // Put(CSV)
            api.AddHeaders.Remove(HeaderConst.Accept);
            var reqPutCsv = api.PutAsText("AA", testData.Data19Csv);
            reqPutCsv.MediaType = "text/csv";
            client.GetWebApiResponseResult(reqPutCsv).Assert(HttpStatusCode.OK);

            // Putしたデータを取得(CSV)
            api.AddHeaders[HeaderConst.Accept] = new string[] { "text/csv" };
            var putResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode);
            putResponse.RawContentString.Is(testData.Data19GetCsv);

            // プロパティ欠損データを登録
            api.AddHeaders.Remove(HeaderConst.Accept);
            client.GetWebApiResponseResult(api.RegistList(testData.Data22)).Assert(RegisterSuccessExpectStatusCode);

            // プロパティ欠損データを含む全データを取得(CSV)
            api.AddHeaders[HeaderConst.Accept] = new string[] { "text/csv" };
            getListResponse = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode);
            getListResponse.RawContentString.Is(testData.Data22GetCsv);

            // 優先度をCSV→jsonで配列を取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/json;q=0.5,text/csv" };
            getResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode);
            getResponse.RawContentString.Is(testData.Data19GetCsv);

            // 優先度をjson→CSVで配列を取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/json,text/csv;q=0.5" };
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data19Get);
        }

        [TestMethod]
        public void AcceptDataTest_TypePatternScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAcceptDataApi>();
            var testData = new AcceptDataTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 複数件登録(XML)
            var req = api.RegistListAsText(testData.Data6Xml);
            req.MediaType = "application/xml";
            var regResponse = client.GetWebApiResponseResult(req).Assert(RegisterSuccessExpectStatusCode);
            regResponse.RawContentString.StringToXml().Is(testData.Data6RegistExpectedXml.StringToXml());

            // 登録した2件をXMLで取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/xml" };
            var getResponse = client.GetWebApiResponseResult(api.Get("CC")).Assert(GetSuccessExpectStatusCode);
            getResponse.RawContentString.StringToXml().Is(testData.Data6GetXml1.StringToXml());
            getResponse = client.GetWebApiResponseResult(api.Get("DD")).Assert(GetSuccessExpectStatusCode);
            getResponse.RawContentString.StringToXml().Is(testData.Data6GetXml2.StringToXml());

            // Jsonで取得
            api.AddHeaders.Remove(HeaderConst.Accept);
            client.GetWebApiResponseResult(api.Get("CC")).Assert(GetSuccessExpectStatusCode, testData.Data6Get);
        }

        [TestMethod]
        public void AcceptDataTest_TypePatternCsvScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAcceptDataApi>();
            var testData = new AcceptDataTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 複数件登録(CSV)
            var req = api.RegistListAsText(testData.Data18Csv);
            req.MediaType = "text/csv";
            var regResponse = client.GetWebApiResponseResult(req).Assert(RegisterSuccessExpectStatusCode);
            regResponse.RawContentString.Is(testData.Data18RegistExpectedCsv);

            // 登録した2件をCSVで取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "text/csv" };
            var getResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode);
            getResponse.RawContentString.Is(testData.Data18GetCsv1);
            getResponse = client.GetWebApiResponseResult(api.Get("BB")).Assert(GetSuccessExpectStatusCode);
            getResponse.RawContentString.Is(testData.Data18GetCsv2);

            // Jsonで取得
            api.AddHeaders.Remove(HeaderConst.Accept);
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data18Get);
        }

        [TestMethod]
        public void AcceptDataTest_ArrayTypeScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAcceptDataApi>();
            var testData = new AcceptDataTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Jsonで配列を登録
            client.GetWebApiResponseResult(api.Regist(testData.Data11)).Assert(RegisterSuccessExpectStatusCode, testData.Data11RegistExpected);

            // Jsonで取得
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data11Get);

            // XMLで取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/xml" };
            var getResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode);
            getResponse.RawContentString.StringToXml().Is(testData.Data11GetXml.StringToXml());

            // XMLで配列を登録
            var req = api.RegistListAsText(testData.Data12Xml);
            req.MediaType = "application/xml";
            var regResponse = client.GetWebApiResponseResult(req).Assert(RegisterSuccessExpectStatusCode);
            regResponse.RawContentString.StringToXml().Is(testData.Data12RegistExpectedXml.StringToXml());

            // Jsonで配列を取得
            api.AddHeaders.Remove(HeaderConst.Accept);
            client.GetWebApiResponseResult(api.Get("BB")).Assert(GetSuccessExpectStatusCode, testData.Data12Get);
        }

        [TestMethod]
        public void AcceptDataTest_ArrayTypeScenarioCsv()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAcceptDataApi>();
            var testData = new AcceptDataTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Jsonで配列とオブジェクトを登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data20)).Assert(RegisterSuccessExpectStatusCode, testData.Data20RegistExpected);

            // CSVでオブジェクトを取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "text/csv" };
            client.GetWebApiResponseResult(api.Get("AA")).AssertErrorCode(BadRequestStatusCode, "E50408");

            // CSVで配列を取得
            client.GetWebApiResponseResult(api.Get("BB")).AssertErrorCode(BadRequestStatusCode, "E50408");


            // 優先度をCSV→jsonで配列を取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/json;q=0.5,text/csv" };
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data20_1Get);

            // 優先度をCSV→jsonで配列を取得
            client.GetWebApiResponseResult(api.Get("BB")).Assert(GetSuccessExpectStatusCode, testData.Data20_2Get);

            // 優先度をjson→CSVで配列を取得
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/json,text/csv;q=0.5" };
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data20_1Get);

            // 優先度をjson→CSVで配列を取得
            client.GetWebApiResponseResult(api.Get("BB")).Assert(GetSuccessExpectStatusCode, testData.Data20_2Get);
        }

        [TestMethod]
        public void AcceptDataTest_BadRequestScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAcceptDataApi>();
            var testData = new AcceptDataTestData();

            // Json
            // キーなし
            var reg7 =client.GetWebApiResponseResult(api.RegistList(testData.Data7)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10402");
            reg7.RawContentString.Contains(testData.Data7RegistExpected).Is(true);

            // 必須項目なし
            var reg9 = client.GetWebApiResponseResult(api.RegistList(testData.Data9)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10402");
            reg9.RawContentString.Contains(testData.Data9RegistExpected).Is(true);

            // Xml
            // キーなし
            api.AddHeaders[HeaderConst.Accept] = new string[] { "application/xml" };
            var req = api.RegistListAsText(testData.Data8Xml);
            req.MediaType = "application/xml";
            var reg8 = client.GetWebApiResponseResult(req).Assert(RegisterErrorExpectStatusCode);
            reg8.RawContentString.Contains(testData.Data8RegistExpected).Is(true);

            req = api.RegistListAsText(testData.Data10Xml);
            req.MediaType = "application/xml";
            var reg10 = client.GetWebApiResponseResult(req).Assert(RegisterErrorExpectStatusCode);
            reg10.RawContentString.Contains(testData.Data10RegistExpected).Is(true);

            // CSV
            // スキーマの型に変換できないパターン
            api.AddHeaders[HeaderConst.Accept] = new string[] { "text/csv" };
            req = api.RegistListAsText(testData.Data21Csv);
            req.MediaType = "text/csv";
            var reg21 = client.GetWebApiResponseResult(req).Assert(RegisterErrorExpectStatusCode);
            reg21.RawContentString.Contains(testData.Data21RegistExpected).Is(true);
        }
    }
}
