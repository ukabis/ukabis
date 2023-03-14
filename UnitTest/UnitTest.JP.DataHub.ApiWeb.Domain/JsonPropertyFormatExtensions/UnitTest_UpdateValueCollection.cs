using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    [TestClass]
    public class UnitTest_UpdateValueCollection
    {
        [TestMethod]
        public void MergeTes_ShallowPatht()
        {
            var x = new UpdateValueCollection();
            x.Add(new UpdateValue() { Url = "/API/Hoge", Property = "ABC", Value = "123" });
            x.Add(new UpdateValue() { Url = "/API/Hoge", Property = "XYZ", Value = "986" });
            x.Add(new UpdateValue() { Url = "/API/Peke", Property = "zxc", Value = "5465" });
            var m = x.Merge().ToList();
            m.Count.Is(2);
            m[0].Url.Is("/API/Hoge");
            m[0].Json.Is("{ 'ABC' : '123', 'XYZ' : '986' }".ToJson());
            m[1].Url.Is("/API/Peke");
            m[1].Json.Is("{ 'zxc' : '5465' }".ToJson());
        }

        [TestMethod]
        public void Merge_DeepPath()
        {
            var x = new UpdateValueCollection();
            x.Add(new UpdateValue() { Url = "/API/Hoge", Property = "ABC", Value = "123" });
            x.Add(new UpdateValue() { Url = "/API/Hoge", Property = "Array[2]", Value = "986" });
            x.Add(new UpdateValue() { Url = "/API/Hoge", Property = "NumberArray[1]", Value = 5465 });
            x.Add(new UpdateValue() { Url = "/API/Hoge", Property = "ArrayObject[0].Date", Value = "2001/01/01" });
            x.Add(new UpdateValue() { Url = "/API/Hoge", Property = "Object.prop3.prop31", Value = "01" });
            x.Add(new UpdateValue() { Url = "/API/Hoge2", Property = "123", Value = "456" });
            var m = x.Merge().ToList();
            m.Count.Is(2);
            m[0].Url.Is("/API/Hoge");
            m[0].Json.Is("{ 'ABC' : '123', 'Array[2]' : '986', 'NumberArray[1]' : 5465, 'ArrayObject[0].Date' : '2001/01/01', 'Object.prop3.prop31' : '01' }".ToJson());
            m[1].Url.Is("/API/Hoge2");
            m[1].Json.Is("{ '123' : '456' }".ToJson());
        }
    }
}
