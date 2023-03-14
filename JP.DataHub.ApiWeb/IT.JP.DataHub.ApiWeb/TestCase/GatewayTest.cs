using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Retry;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class GatewayTest : ApiWebItTestCase
    {
        public static int TestImage1Size = 391313;
        public static int TestImage2Size = 6586747;

        // 20秒おきに9回までリトライ max3minかかる可能性がある
        private RetryPolicy<HttpResponseMessage> _retryStatusPolicy(int size) => Policy
            .HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    return true;
                }

                var stream = r.Content.ReadAsStreamAsync().Result;
                return stream.Length != size;

            })
            .WaitAndRetry(9, i => TimeSpan.FromSeconds(20));


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// 基本シナリオ
        /// </summary>
        [TestMethod]
        public void GatewayTest_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IGatewayValuesCacheOffApi>();
            InitializeHeader(api);

            var request = api.GetByQueryString("id2=hoge&id1=fuga");
            var response = client.GetWebApiResponseResult(request).Assert(GetSuccessExpectStatusCode);
            var resultJson1 = response.ContentString.ToJson<GatewayValueResult>();

            response = client.GetWebApiResponseResult(request).Assert(GetSuccessExpectStatusCode);
            var resultJson2 = response.ContentString.ToJson<GatewayValueResult>();

            // キャッシュしていないためTimestampが異なる
            resultJson1.timestampString.IsNot(resultJson2.timestampString);

            // 中継ヘッダーが正しく中継できていることを確認 API側で1,3だけ中継する設定にしている
            resultJson1.IsContainsHeaderKey("X-RelayTest1").IsTrue();
            resultJson1.IsContainsHeaderKey("X-RelayTest2").IsFalse();
            resultJson1.IsContainsHeaderKey("X-RelayTest3").IsTrue();

            // パラメータが正しくセットされていることの確認
            resultJson1.IsContainsQueryString("id2", "hoge").IsTrue();
            resultJson1.IsContainsQueryString("id1", "fuga").IsTrue();

            // GatewayはQueryStringのマッチは行わないのでパラメータ数が異なっても利用可能
            // /values/get?id1={id1}&id2={id2}  APIの定義Gatewayへの定義
            response = client.GetWebApiResponseResult(api.GetByQueryString("id2=hoge")).Assert(GetSuccessExpectStatusCode);
            var resultJson = response.ContentString.ToJson<GatewayValueResult>();
            resultJson.IsContainsQueryString("id2", "hoge").IsTrue();
            resultJson.IsContainsQueryString("id1").IsFalse();

            // GatewayはQueryStringのマッチは行わないのでパラメータがなくても利用可能
            response = client.GetWebApiResponseResult(api.GetByQueryString("")).Assert(GetSuccessExpectStatusCode);
            resultJson = response.ContentString.ToJson<GatewayValueResult>();
            //QueryStringがない場合はGateway先にそのままアクセスされる。values/get?id1={id1}&id2={id2}
            //resultJson.IsContainsQueryString("id2").IsFalse();
            //resultJson.IsContainsQueryString("id1").IsFalse();

            // GatewayはQueryStringのマッチは行わないので関係ないパラメータが含まれていても利用可能
            response = client.GetWebApiResponseResult(api.GetByQueryString("id2=hoge&id3=fuga")).Assert(GetSuccessExpectStatusCode);
            resultJson = response.ContentString.ToJson<GatewayValueResult>();
            resultJson.IsContainsQueryString("id2", "hoge").IsTrue();
            resultJson.IsContainsQueryString("id3").IsFalse();
        }

        /// <summary>
        /// Acceptヘッダーに関するテスト
        /// </summary>
        [TestMethod]
        public void GatewayTest_AcceptScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IGatewayValuesCacheOffApi>();
            InitializeHeader(api);

            // 正常なAcceptヘッダー
            api.AddHeaders.Add(HeaderConst.Accept, "application/json");
            var response = client.GetWebApiResponseResult(api.GetByQueryString("id2=hoge&id1=fuga")).Assert(GetSuccessExpectStatusCode);
            var resultJson = response.ContentString.ToJson<GatewayValueResult>();

            // Acceptヘッダーが正しく中継できていることを確認
            var acceptValue = resultJson.headers.Where(x => x.StartsWith(HeaderConst.Accept)).ToList();
            acceptValue.Count.Is(1);
            acceptValue.Single().Split('=')[1].Trim().Is("application/json");

            // 異常なAccpetヘッダー
            api.AddHeaders.Remove(HeaderConst.Accept);
            api.AddHeaders.Add(HeaderConst.Accept, "xxxx");
            response = client.GetWebApiResponseResult(api.GetByQueryString("id2=hoge&id1=fuga")).Assert(GetSuccessExpectStatusCode);
            resultJson = response.ContentString.ToJson<GatewayValueResult>();

            // 不正なAcceptヘッダーでもそのまま中継できていることを確認
            acceptValue = resultJson.headers.Where(x => x.StartsWith(HeaderConst.Accept)).ToList();
            acceptValue.Count.Is(1);
            acceptValue.Single().Split('=')[1].Trim().Is("xxxx");

        }

        /// <summary>
        /// キャッシュ有効時のシナリオ
        /// </summary>
        [TestMethod]
        public void GatewayTest_CacheOnSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IGatewayValuesCacheOnApi>();
            InitializeHeader(api);

            var request = api.GetByQueryString("id1=a&id2=b");
            var response1 = client.GetWebApiResponseResult(request).Assert(GetSuccessExpectStatusCode);
            var resultJson1 = response1.ContentString.ToJson<GatewayValueResult>();

            var response2 = client.GetWebApiResponseResult(request).Assert(GetSuccessExpectStatusCode);
            var resultJson2 = response2.ContentString.ToJson<GatewayValueResult>();

            //キャッシュしているためTimestampは同じ
            resultJson1.timestampString.Is(resultJson2.timestampString);
        }

        /// <summary>
        /// キャッシュ無効時のシナリオ(画像)
        /// </summary>
        [TestMethod]
        public void GatewayTest_ImageCacheOffSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IGatewayImagesCacheOffApi>();
            InitializeHeader(api);

            // 1M以内の画像ファイル
            var response1 = client.GetWebApiResponseResult(api.Get("1")).Assert(GetSuccessExpectStatusCode);
            var stream = response1.RawContent.ReadAsStream();
            stream.Length.Is(TestImage1Size);

            // 1M以上の画像ファイル
            var response2 = client.GetWebApiResponseResult(api.Get("2")).Assert(GetSuccessExpectStatusCode);
            var stream2 = response2.RawContent.ReadAsStream();
            stream2.Length.Is(TestImage2Size);
        }

        /// <summary>
        /// キャッシュ有効時のシナリオ(画像)
        /// </summary>
        [TestMethod]
        public void GatewayTest_ImageCacheOnSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IGatewayImagesCacheOnApi>();
            InitializeHeader(api);

            // 1M以内の画像ファイル
            // 1M以内の場合はキャッシュされるためTimeStampは同一になる
            var response1_1 = client.GetWebApiResponseResult(api.Get("1")).Assert(GetSuccessExpectStatusCode);
            var stream1_1 = response1_1.RawContent.ReadAsStream();
            stream1_1.Length.Is(TestImage1Size);

            var response1_2 = client.GetWebApiResponseResult(api.Get("1")).Assert(GetSuccessExpectStatusCode);
            var stream1_2 = response1_2.RawContent.ReadAsStream();
            stream1_2.Length.Is(TestImage1Size);

            var timestamp1_1 = response1_1.Headers.Where(x => x.Key == "responsetime").Select(x => x.Value).First();
            var timestamp1_2 = response1_2.Headers.Where(x => x.Key == "responsetime").Select(x => x.Value).First();
            timestamp1_1.Is(timestamp1_2);

            // 1M以上の画像ファイル
            // 1M以上の場合はキャッシュされないためTimeStampは同一にならない
            var response2_1 = client.GetWebApiResponseResult(api.Get("2"), _retryStatusPolicy(TestImage2Size)).Assert(GetSuccessExpectStatusCode);

            var response2_2 = client.GetWebApiResponseResult(api.Get("2"), _retryStatusPolicy(TestImage2Size)).Assert(GetSuccessExpectStatusCode);

            var timestamp2_1 = response2_1.Headers.Where(x => x.Key == "responsetime").Select(x => x.Value).First();
            var timestamp2_2 = response2_2.Headers.Where(x => x.Key == "responsetime").Select(x => x.Value).First();
            timestamp2_1.IsNot(timestamp2_2);
        }

        /// <summary>
        /// キャッシュ有効時のシナリオ(画像)チャンクの場合
        /// </summary>
        [TestMethod]
        public void GatewayTest_ImageCacheOnSenarioChunked()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IGatewayImagesCacheOnApi>();
            InitializeHeader(api);

            // 1M以内の画像ファイル
            // 1M以内の場合はキャッシュされるためTimeStampは同一になる
            var response1_1 = client.GetWebApiResponseResult(api.Get("1_Chunked")).Assert(GetSuccessExpectStatusCode);
            response1_1.Headers.TransferEncodingChunked.Is(true);
            var stream1_1 = response1_1.RawContent.ReadAsStream();
            stream1_1.Length.Is(TestImage1Size);

            var response1_2 = client.GetWebApiResponseResult(api.Get("1_Chunked")).Assert(GetSuccessExpectStatusCode);
            response1_2.Headers.TransferEncodingChunked.Is(true);
            var stream1_2 = response1_2.RawContent.ReadAsStream();
            stream1_2.Length.Is(TestImage1Size);

            var timestamp1_1 = response1_1.Headers.Where(x => x.Key == "responsetime").Select(x => x.Value).First();
            var timestamp1_2 = response1_2.Headers.Where(x => x.Key == "responsetime").Select(x => x.Value).First();
            timestamp1_1.Is(timestamp1_2);

            // 1M以上の画像ファイル
            // 1M以上の場合はキャッシュされないためTimeStampは同一にならない
            var response2_1 = client.GetWebApiResponseResult(api.Get("2_Chunked")).Assert(GetSuccessExpectStatusCode);
            response2_1.Headers.TransferEncodingChunked.Is(true);
            var stream2_1 = response2_1.RawContent.ReadAsStream();
            stream2_1.Length.Is(TestImage2Size);

            var response2_2 = client.GetWebApiResponseResult(api.Get("2_Chunked")).Assert(GetSuccessExpectStatusCode);
            response2_2.Headers.TransferEncodingChunked.Is(true);
            var stream2_2 = response2_2.RawContent.ReadAsStream();
            stream2_2.Length.Is(TestImage2Size);

            var timestamp2_1 = response2_1.Headers.Where(x => x.Key == "responsetime").Select(x => x.Value).First();
            var timestamp2_2 = response2_2.Headers.Where(x => x.Key == "responsetime").Select(x => x.Value).First();
            timestamp2_1.IsNot(timestamp2_2);
        }

        /// <summary>
        /// キャッシュ無効時のシナリオ(画像)チャンク時
        /// </summary>
        [TestMethod]
        public void GatewayTest_ImageCacheOffSenarioChunked()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IGatewayImagesCacheOffApi>();
            InitializeHeader(api);

            // 1M以内の画像ファイル
            var response1 = client.GetWebApiResponseResult(api.Get("1_Chunked")).Assert(GetSuccessExpectStatusCode);
            response1.Headers.TransferEncodingChunked.Is(true);
            var stream = response1.RawContent.ReadAsStream();
            stream.Length.Is(TestImage1Size);

            // 1M以上の画像ファイル
            var response2 = client.GetWebApiResponseResult(api.Get("2_Chunked")).Assert(GetSuccessExpectStatusCode);
            response2.Headers.TransferEncodingChunked.Is(true);
            var stream2 = response2.RawContent.ReadAsStream();
            stream2.Length.Is(TestImage2Size);
        }

        /// <summary>
        /// 中継ヘッダ大文字小文字シナリオ
        /// </summary>
        [TestMethod]
        public void GatewayTest_RelayHeaderIgnoreCaseSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IGatewayValuesCacheOnApi>();

            // X-GetInternalIdをFalse指定しないとキャッシュが有効にならない不具合があるため暫定で設定
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "false");

            // API側の中継ヘッダの設定と大文字小文字の異なるヘッダ
            api.AddHeaders.Add("x-rELAYtEST1", "test1");
            api.AddHeaders.Add("x-rELAYtEST2", "test2");
            api.AddHeaders.Add("x-rELAYtEST3", "test3");

            var response1 = client.GetWebApiResponseResult(api.GetByQueryString("id2=foo&id1=bar")).Assert(GetSuccessExpectStatusCode);
            var resultJson1 = response1.ContentString.ToJson<GatewayValueResult>();

            // キャッシュされたリクエストと大文字小文字の異なるヘッダ
            api = UnityCore.Resolve<IGatewayValuesCacheOnApi>();
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "false");
            api.AddHeaders.Add("X-rELAYtEST1", "test1");
            api.AddHeaders.Add("X-rELAYtEST2", "test2");
            api.AddHeaders.Add("X-rELAYtEST3", "test3");

            var response2 = client.GetWebApiResponseResult(api.GetByQueryString("id2=foo&id1=bar")).Assert(GetSuccessExpectStatusCode);
            var resultJson2 = response2.ContentString.ToJson<GatewayValueResult>();

            // キャッシュしているためTimestampは同じ
            resultJson1.timestampString.Is(resultJson2.timestampString);

            // 大文字小文字が違っても中継ヘッダーが正しく中継できていることを確認(API側で1,3だけ中継する設定にしている)
            resultJson1.IsContainsHeaderKey("x-rELAYtEST1").IsTrue();
            resultJson1.IsContainsHeaderKey("x-rELAYtEST2").IsFalse();
            resultJson1.IsContainsHeaderKey("x-rELAYtEST3").IsTrue();

            // 大文字小文字が違ってもキャッシュが取得できていることを確認 (API側で1,3だけ中継する設定にしている)
            resultJson2.IsContainsHeaderKey("x-rELAYtEST1").IsTrue();
            resultJson2.IsContainsHeaderKey("x-rELAYtEST2").IsFalse();
            resultJson2.IsContainsHeaderKey("x-rELAYtEST3").IsTrue();
        }


        private void InitializeHeader<T>(ICommonResource<T> api)
        {
            // X-GetInternalIdをFalse指定しないとキャッシュが有効にならない不具合があるため暫定で設定
            // （AGRI_ICT-3271 DynamicApiでX-GetInternalId: falseを指定しないとキャッシュが使われない）
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "false");

            // 中継ヘッダーテスト用
            api.AddHeaders.Add("X-RelayTest1", "test1");
            api.AddHeaders.Add("X-RelayTest2", "test2");
            api.AddHeaders.Add("X-RelayTest3", "test3");
        }


        public class GatewayValueResult
        {
            public string year { get; set; }
            public string month { get; set; }
            public string day { get; set; }
            public string hour { get; set; }
            public string minute { get; set; }
            public string second { get; set; }
            public string millisecond { get; set; }
            public List<string> headers { get; set; }
            public string httpMethod { get; set; }
            public string requestUri { get; set; }
            public string body { get; set; }

            public string timestampString
            {
                get
                {
                    return year + month + day + hour + minute + second + millisecond;
                }
            }

            public bool IsContainsHeaderKey(string key)
            {
                foreach (var h in headers)
                {
                    var k = h.Split('=');
                    if (k.Length >= 2)
                    {
                        if (k[0].Trim() == key)
                            return true;
                    }
                }
                return false;
            }
            public bool IsContainsQueryString(string key, string value)
            {
                var su = requestUri.Split('?');
                if (su.Length >= 2)
                {
                    var qs = su[1].Split('&');
                    foreach (var q in qs)
                    {
                        var k = q.Split('=');
                        if (k.Length >= 2)
                        {
                            if (k[0].Trim() == key && k[1].Trim() == value)
                                return true;
                        }
                    }
                }
                return false;
            }
            public bool IsContainsQueryString(string key)
            {
                var su = requestUri.Split('?');
                if (su.Length >= 2)
                {
                    var qs = su[1].Split('&');
                    foreach (var q in qs)
                    {
                        var k = q.Split('=');
                        if (k.Length >= 2)
                        {
                            if (k[0].Trim() == key)
                                return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
