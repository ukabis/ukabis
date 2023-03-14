using System.Net;
using System.Text;
using AutoMapper;
using Newtonsoft.Json;
using JP.DataHub.Com.Log;
using JP.DataHub.Aop;

namespace JP.DataHub.ApiFilterSample
{
    public interface MyParam
    {
        string Action { get; set; }
        string VendorId { get; set; }
        string SystemId { get; set; }
        string OpenId { get; set; }
        string ResourceUrl { get; set; }
        string ApiUrl { get; set; }
        string MediaType { get; set; }
        string QueryString { get; set; }
        string Contents { get; set; }
        string Accept { get; set; }
        string ContentRange { get; set; }
        string HttpMethodType { get; set; }
    }


    public class ModifySchemaFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            param.RequestSchema = null;
            param.ResponseSchema = null;
            param.ControllerSchema = null;
            return null;
        }
    }

    public class ApiFilter : AbstractApiFilter
    {
        private MyParam MyParam { get; set; }

        private static Lazy<IMapper> _Mapper = new Lazy<IMapper>(() => new MapperConfiguration(cfg => cfg.CreateMap<IApiFilterActionParam, MyParam>().ReverseMap()).CreateMapper());
        private static IMapper Mapper { get => _Mapper.Value; }

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            Logger.Info("BeforeAction Start");
            var result = param.ApiHelper.ExecuteGetApi("hoge");
            MyParam = Mapper.Map<MyParam>(param);
            Logger.Info("BeforeAction End");
            return null;
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            Logger.Info("AfterAction Start");
            msg.Headers.Add("X-Hook", "1");
            msg.Headers.Add("X-BeforeAction", JsonConvert.SerializeObject(MyParam));
            Logger.Info("AfterAction End");
            return msg;
        }
    }

    public class ApiFilter1 : AbstractApiFilter
    {
    }


    public class OrderedApiFilterA : AbstractApiFilter
    {
        private string qs { get; set; }

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            qs = param.QueryString;
            param.QueryString = "?$select=AreaUnitCode";
            return null;
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            // OrderedApiFilterB でmsg返却しているので以下は呼ばれないはず
            msg.Headers.Add("X-Hook", "1");
            msg.Headers.Add("X-AfterAction", nameof(OrderedApiFilterA));
            msg.Headers.Add("X-RequestQueryString", qs);
            return msg;
        }
    }

    public class OrderedApiFilterB : AbstractApiFilter
    {
        private string qs { get; set; }

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            param.QueryString += "&$top=1";
            qs = param.QueryString;
            return null;
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            msg.Headers.Add("X-Hook", "2");
            msg.Headers.Add("X-AfterAction", nameof(OrderedApiFilterB));
            msg.Headers.Add("X-RequestUrl", qs);
            return msg;
        }
    }

    /// <summary>
    /// AOP用のキャッシュテスト
    /// </summary>
    /// <remarks>
    /// テストの流れはFilterApiTest_AopCacheScenario参照
    /// </remarks>
    public class AopCache2 : AbstractApiFilter
    {
        private string _key;
        private string _value2;

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            _key = param.QueryStringDic.GetOrDefault("key");
            _value2 = param.QueryStringDic.GetOrDefault("value2");

            // dllが違うためAopCache1で登録されたキャッシュは取得されない
            var result = CacheHelper.GetOrAdd(_key, () => _value2);
            if (result == _value2)
            {
                return null;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("AopCache2.BeforeAction: CacheHelper.GetOrAdd returns unexpected value.", Encoding.UTF8, "text/plain");
                return response;
            }
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            // AopCache2で登録されたキャッシュが取得される
            var result = CacheHelper.GetOrAdd(_key, () => Guid.NewGuid().ToString());
            if (result == _value2)
            {
                return null;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("AopCache2.AfterAction: CacheHelper.GetOrAdd returns unexpected value.", Encoding.UTF8, "text/plain");
                return response;
            }
        }
    }
}