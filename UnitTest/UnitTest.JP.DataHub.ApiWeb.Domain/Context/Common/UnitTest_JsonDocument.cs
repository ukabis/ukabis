using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_JsonDocument
    {
        [TestMethod]
        public void JsonDocument_RemoveTokenNormal()
        {
            var test = new
            {
                id = "aaaaaaa",
                _Type = "aaa",
                _Vendor_Id = "aaaa",
                _System_Id = "aaaaa",
                _Reguser_Id = "aaaaa",
                _Regdate = DateTime.UtcNow,
                _Version = 1,
                _partitionkey = "aaaa",
                _rid = "aaa",
                _self = "aaa",
                _etag = "aaa",
                _attachments = "aaaa",
                _ts = "aaaa",
                test1 = "aaaaa"
            };
            var target = new List<JsonDocument> { new JsonDocument(JToken.FromObject(test)) }.RemoveTokenToJson(false);
            var result = JToken.Parse(target.FirstOrDefault());
            new[]
            {
                "_Type", "_Vendor_Id", "_System_Id", "_Reguser_Id", "_Regdate", "_Version", "_partitionkey", "_rid",
                "_self", "_etag", "_attachments", "_ts"
            }.ToList().ForEach(
                x =>
                {
                    var compare = result.SelectToken(x);
                    compare.IsNull();
                }
            );

            result["test1"].Is("aaaaa");
        }

        [TestMethod]
        public void JsonDocument_RemoveTokenXGetInnerAllField()
        {
            var test = new
            {
                id = "aaaaaaa",
                _Type = "aaa",
                _Vendor_Id = "aaaa",
                _System_Id = "aaaaa",
                _Reguser_Id = "aaaaa",
                _Regdate = DateTime.UtcNow,
                _Version = 1,
                _partitionkey = "aaaa",
                _rid = "aaa",
                _self = "aaa",
                _etag = "aaa",
                _attachments = "aaaa",
                _ts = "aaaa",
                test1 = "aaaaa"
            };
            var target = new List<JsonDocument> { new JsonDocument(JToken.FromObject(test)) }.RemoveTokenToJson(true);
            var result = JToken.Parse(target.FirstOrDefault());
            new[]
            {
                "_rid", "_self", "_etag", "_attachments", "_ts",
            }.ToList().ForEach(
                x =>
                {
                    var compare = result.SelectToken(x);
                    compare.IsNull();
                }
            );
            new[]
            {
                "id","_Type", "_Vendor_Id", "_System_Id", "_Reguser_Id", "_Regdate", "_Version", "_partitionkey",

            }.ToList().ForEach(
                x =>
                {
                    var compare = result.SelectToken(x);
                    compare.IsNotNull();
                }
            );

            result["test1"].Is("aaaaa");
        }
    }
}
