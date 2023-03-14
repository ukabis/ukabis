using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net
{
    public static class HttpStatusCodeExtension
    {
        /// <summary>
        /// HTTP応答が成功したかどうかを返却する
        /// msのdocumentを参考 <seealso cref="https://docs.microsoft.com/ja-jp/dotnet/api/system.net.http.httpresponsemessage.issuccessstatuscode?view=net-5.0"/>
        /// </summary>
        /// <param name="code"></param>
        /// <returns>200-299の範囲内ならtrue それ以外はfalse</returns>
        public static bool IsSuccessStatusCode(this HttpStatusCode code)
            => (int)code is >= 200 and <= 299;
    }
}
