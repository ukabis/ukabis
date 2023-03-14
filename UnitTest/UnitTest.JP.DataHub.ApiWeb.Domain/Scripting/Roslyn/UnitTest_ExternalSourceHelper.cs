using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;
using UnitTest.JP.DataHub.ApiWeb.Domain.MockClass;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass()]
    public class UnitTest_ExternalSourceHelper : UnitTestBase
    {
        #region Setup

        private void SetUpContainer()
        {
            base.TestInitialize(true);
        }

        public class testCsvClass_AutoMapper
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Height { get; set; }
            public decimal Salary { get; set; }
            public bool IsHired { get; set; }
        }

        public class testCsvClass_HeaderName
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Height { get; set; }
            public decimal Salary { get; set; }
            public bool IsHired { get; set; }
        }

        public class testCsvClass_ItemSequence
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Height { get; set; }
            public decimal Salary { get; set; }
            public bool IsHired { get; set; }

            public testCsvClass_ItemSequence(int id, string name, double height, decimal salary, bool isHired)
            {
                Id = id;
                Name = name;
                Height = height;
                Salary = salary;
                IsHired = isHired;
            }
        }
        #endregion

        [TestMethod()]
        public void ParseCsvFromUrlTest_正常系_AutoMapper()
        {
            var url = "https://www.google.com";
            var testCsv = @"
Id,Name,Height,Salary,IsHired
1,テストネーム１,175.5,198000.15,true
2,テストネーム２,275.5,298000.30,false
3,テストネーム３,375.5,398000.45,true
";
            var extFileHelper = new ExternalSourceHelper(new HttpClient(new MoqHttpClient(testCsv)));
            Func<IDictionary<string, object>, testCsvClass_AutoMapper> mapper = (x) =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<IDictionary<string, object>, testCsvClass_AutoMapper>()
                        .ForMember(dst => dst.Id, src => src.MapFrom(s => s["Id"]))
                        .ForMember(dst => dst.Name, src => src.MapFrom(s => s["Name"]))
                        .ForMember(dst => dst.Height, src => src.MapFrom(s => s["Height"]))
                        .ForMember(dst => dst.Salary, src => src.MapFrom(s => s["Salary"]))
                        .ForMember(dst => dst.IsHired, src => src.MapFrom(s => s["IsHired"]));
                });
                return config.CreateMapper().Map<testCsvClass_AutoMapper>(x);
            };

            var resultList = extFileHelper.ParseCsvFromUrlToObject<testCsvClass_AutoMapper>(url, mapper, validator: null);

            var expectList = new List<testCsvClass_AutoMapper>();
            expectList.Add(new testCsvClass_AutoMapper()
            {
                Id = 1,
                Name = "テストネーム１",
                Height = 175.5,
                Salary = new Decimal(198000.15),
                IsHired = true
            });
            expectList.Add(new testCsvClass_AutoMapper()
            {
                Id = 2,
                Name = "テストネーム２",
                Height = 275.5,
                Salary = new Decimal(298000.30),
                IsHired = false
            });
            expectList.Add(new testCsvClass_AutoMapper()
            {
                Id = 3,
                Name = "テストネーム３",
                Height = 375.5,
                Salary = new Decimal(398000.45),
                IsHired = true
            });

            foreach (var result in resultList)
            {

                Assert.AreEqual(result.Id, expectList.Find(x => x.Id == result.Id).Id);
                Assert.AreEqual(result.Name, expectList.Find(x => x.Id == result.Id).Name);
                Assert.AreEqual(result.Height, expectList.Find(x => x.Id == result.Id).Height);
                Assert.AreEqual(result.Salary, expectList.Find(x => x.Id == result.Id).Salary);
                Assert.AreEqual(result.IsHired, expectList.Find(x => x.Id == result.Id).IsHired);
            }
        }
        [TestMethod()]
        public void ParseCsvFromUrlTest_正常系_ヘッダー項目名()
        {
            var url = "https://www.google.com";
            var testCsv = @"
Id,Name,Height,Salary,IsHired
1,テストネーム１,175.5,198000.15,true
2,テストネーム２,275.5,298000.30,false
3,テストネーム３,375.5,398000.45,true
";
            var extFileHelper = new ExternalSourceHelper(new HttpClient(new MoqHttpClient(testCsv)));

            List<testCsvClass_HeaderName> resultList = (List<testCsvClass_HeaderName>)extFileHelper.ParseCsvFromUrlToObject<testCsvClass_HeaderName>(url);

            var expectList = new List<testCsvClass_HeaderName>();
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 1,
                Name = "テストネーム１",
                Height = 175.5,
                Salary = new Decimal(198000.15),
                IsHired = true
            });
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 2,
                Name = "テストネーム２",
                Height = 275.5,
                Salary = new Decimal(298000.30),
                IsHired = false
            });
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 3,
                Name = "テストネーム３",
                Height = 375.5,
                Salary = new Decimal(398000.45),
                IsHired = true
            });

            foreach (var result in resultList)
            {

                Assert.AreEqual(result.Id, expectList.Find(x => x.Id == result.Id).Id);
                Assert.AreEqual(result.Name, expectList.Find(x => x.Id == result.Id).Name);
                Assert.AreEqual(result.Height, expectList.Find(x => x.Id == result.Id).Height);
                Assert.AreEqual(result.Salary, expectList.Find(x => x.Id == result.Id).Salary);
                Assert.AreEqual(result.IsHired, expectList.Find(x => x.Id == result.Id).IsHired);
            }
        }
        [TestMethod()]
        public void ParseCsvFromUrlTest_正常系_項目順()
        {
            var url = "https://www.google.com";
            var testCsv = @"
1,テストネーム１,175.5,198000.15,true
2,テストネーム２,275.5,298000.30,false
3,テストネーム３,375.5,398000.45,true
";
            var extFileHelper = new ExternalSourceHelper(new HttpClient(new MoqHttpClient(testCsv)));
            var config = new ExternalSourceHelper.ParseConfiguration() { HasHeaderRecord = false };
            List<testCsvClass_ItemSequence> resultList = (List<testCsvClass_ItemSequence>)extFileHelper.ParseCsvFromUrlToObject<testCsvClass_ItemSequence>(url, null, config);

            var expectList = new List<testCsvClass_HeaderName>();
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 1,
                Name = "テストネーム１",
                Height = 175.5,
                Salary = new Decimal(198000.15),
                IsHired = true
            });
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 2,
                Name = "テストネーム２",
                Height = 275.5,
                Salary = new Decimal(298000.30),
                IsHired = false
            });
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 3,
                Name = "テストネーム３",
                Height = 375.5,
                Salary = new Decimal(398000.45),
                IsHired = true
            });

            foreach (var result in resultList)
            {

                Assert.AreEqual(result.Id, expectList.Find(x => x.Id == result.Id).Id);
                Assert.AreEqual(result.Name, expectList.Find(x => x.Id == result.Id).Name);
                Assert.AreEqual(result.Height, expectList.Find(x => x.Id == result.Id).Height);
                Assert.AreEqual(result.Salary, expectList.Find(x => x.Id == result.Id).Salary);
                Assert.AreEqual(result.IsHired, expectList.Find(x => x.Id == result.Id).IsHired);
            }
        }
        [TestMethod()]
        public void ParseCsvFromUrlTest_正常系_マッピングなし()
        {
            var url = "https://www.google.com";
            var testCsv = @"
1,テストネーム１,175.5,198000.15,true
2,テストネーム２,275.5,298000.30,false
3,テストネーム３,375.5,398000.45,true
";
            var extFileHelper = new ExternalSourceHelper(new HttpClient(new MoqHttpClient(testCsv)));
            var config = new ExternalSourceHelper.ParseConfiguration() { HasHeaderRecord = false };
            List<Dictionary<string, object>> resultDic = (List<Dictionary<string, object>>)extFileHelper.ParseCsvFromUrlToDictionary(url, config);

            var expectList = new List<testCsvClass_HeaderName>();
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 1,
                Name = "テストネーム１",
                Height = 175.5,
                Salary = new Decimal(198000.15),
                IsHired = true
            });
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 2,
                Name = "テストネーム２",
                Height = 275.5,
                Salary = new Decimal(298000.30),
                IsHired = false
            });
            expectList.Add(new testCsvClass_HeaderName()
            {
                Id = 3,
                Name = "テストネーム３",
                Height = 375.5,
                Salary = new Decimal(398000.45),
                IsHired = true
            });

            foreach (var result in resultDic)
            {
                Assert.AreEqual(Convert.ToInt32(result["Field1"]), expectList.Find(x => x.Id == Convert.ToInt32(result["Field1"])).Id);
                Assert.AreEqual(Convert.ToString(result["Field2"]), expectList.Find(x => x.Id == Convert.ToInt32(result["Field1"])).Name);
                Assert.AreEqual(Convert.ToDouble(result["Field3"]), expectList.Find(x => x.Id == Convert.ToInt32(result["Field1"])).Height);
                Assert.AreEqual(Convert.ToDecimal(result["Field4"]), expectList.Find(x => x.Id == Convert.ToInt32(result["Field1"])).Salary);
                Assert.AreEqual(Convert.ToBoolean(result["Field5"]), expectList.Find(x => x.Id == Convert.ToInt32(result["Field1"])).IsHired);
            }
        }

        public void ParseCsvFromUrlTest_正常系_大量呼び出し()
        {
            for (int i = 0; i < 10; i++)
            {
                var url = "https://www.google.com";
                var testCsv = @"
1,テストネーム１,175.5,198000.15,true
2,テストネーム２,275.5,298000.30,false
3,テストネーム３,375.5,398000.45,true
";
                var extFileHelper = new ExternalSourceHelper(new HttpClient(new MoqHttpClient(testCsv)));
                var config = new ExternalSourceHelper.ParseConfiguration() { HasHeaderRecord = false };
                List<testCsvClass_ItemSequence> resultList = (List<testCsvClass_ItemSequence>)extFileHelper.ParseCsvFromUrlToObject<testCsvClass_ItemSequence>(url, null, config);

                var expectList = new List<testCsvClass_HeaderName>();
                expectList.Add(new testCsvClass_HeaderName()
                {
                    Id = 1,
                    Name = "テストネーム１",
                    Height = 175.5,
                    Salary = new Decimal(198000.15),
                    IsHired = true
                });
                expectList.Add(new testCsvClass_HeaderName()
                {
                    Id = 2,
                    Name = "テストネーム２",
                    Height = 275.5,
                    Salary = new Decimal(298000.30),
                    IsHired = false
                });
                expectList.Add(new testCsvClass_HeaderName()
                {
                    Id = 3,
                    Name = "テストネーム３",
                    Height = 375.5,
                    Salary = new Decimal(398000.45),
                    IsHired = true
                });

                foreach (var result in resultList)
                {

                    Assert.AreEqual(result.Id, expectList.Find(x => x.Id == result.Id).Id);
                    Assert.AreEqual(result.Name, expectList.Find(x => x.Id == result.Id).Name);
                    Assert.AreEqual(result.Height, expectList.Find(x => x.Id == result.Id).Height);
                    Assert.AreEqual(result.Salary, expectList.Find(x => x.Id == result.Id).Salary);
                    Assert.AreEqual(result.IsHired, expectList.Find(x => x.Id == result.Id).IsHired);
                }
            }
            Assert.IsTrue(true);
        }
        [TestMethod()]
        public void ParseCsvFromUrlTest_正常系_validationError()
        {
            var url = "https://www.google.com";
            var testCsv = @"
Id,Name,Height,Salary,IsHired
1,テストネーム１,175.5,198000.15,true
2,テストネーム２,275.5,298000.30,false
3,テストネーム３,375.5,398000.45,true
";
            var extFileHelper = new ExternalSourceHelper(new HttpClient(new MoqHttpClient(testCsv)));
            Func<IDictionary<string, object>, testCsvClass_AutoMapper> mapper = (x) =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<IDictionary<string, object>, testCsvClass_AutoMapper>()
                        .ForMember(dst => dst.Id, src => src.MapFrom(s => s["Id"]))
                        .ForMember(dst => dst.Name, src => src.MapFrom(s => s["Name"]))
                        .ForMember(dst => dst.Height, src => src.MapFrom(s => s["Height"]))
                        .ForMember(dst => dst.Salary, src => src.MapFrom(s => s["Salary"]))
                        .ForMember(dst => dst.IsHired, src => src.MapFrom(s => s["IsHired"]));
                });
                return config.CreateMapper().Map<testCsvClass_AutoMapper>(x);
            };
            Func<IDictionary<string, object>, bool> validator = (line) => { return Convert.ToInt32(line["Id"]) > 1; };
            var resultList = extFileHelper.ParseCsvFromUrlToObject<testCsvClass_AutoMapper>(url, mapper, null, validator);

            var expectList = new List<testCsvClass_AutoMapper>();
            //expectList.Add(new testCsvClass_AutoMapper()
            //{
            //    Id = 1,
            //    Name = "テストネーム１",
            //    Height = 175.5,
            //    Salary = new Decimal(198000.15),
            //    IsHired = true
            //});
            expectList.Add(new testCsvClass_AutoMapper()
            {
                Id = 2,
                Name = "テストネーム２",
                Height = 275.5,
                Salary = new Decimal(298000.30),
                IsHired = false
            });
            expectList.Add(new testCsvClass_AutoMapper()
            {
                Id = 3,
                Name = "テストネーム３",
                Height = 375.5,
                Salary = new Decimal(398000.45),
                IsHired = true
            });
            Assert.AreEqual(resultList.Count(), 2);
            foreach (var result in resultList)
            {

                Assert.AreEqual(result.Id, expectList.Find(x => x.Id == result.Id).Id);
                Assert.AreEqual(result.Name, expectList.Find(x => x.Id == result.Id).Name);
                Assert.AreEqual(result.Height, expectList.Find(x => x.Id == result.Id).Height);
                Assert.AreEqual(result.Salary, expectList.Find(x => x.Id == result.Id).Salary);
                Assert.AreEqual(result.IsHired, expectList.Find(x => x.Id == result.Id).IsHired);
            }
        }
    }

}