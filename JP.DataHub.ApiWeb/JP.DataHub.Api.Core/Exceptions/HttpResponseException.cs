using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class HttpResponseException : Exception
    {
        //
        // 概要:
        //     クライアントに返す HTTP 応答を取得します。
        //
        // 戻り値:
        //     HTTP 応答を表す System.Net.Http.HttpResponseMessage。
        public HttpResponseMessage Response { get; }

        //
        // 概要:
        //     System.Web.Http.HttpResponseException クラスの新しいインスタンスを初期化します。
        //
        // パラメーター:
        //   statusCode:
        //     応答の状態コード。
        public HttpResponseException(HttpStatusCode statusCode)
        {
            Response = new HttpResponseMessage(statusCode);
        }

        //
        // 概要:
        //     System.Web.Http.HttpResponseException クラスの新しいインスタンスを初期化します。
        //
        // パラメーター:
        //   response:
        //     クライアントに返す HTTP 応答。
        public HttpResponseException(HttpResponseMessage response)
        {
            Response = response;
        }
    }
}
