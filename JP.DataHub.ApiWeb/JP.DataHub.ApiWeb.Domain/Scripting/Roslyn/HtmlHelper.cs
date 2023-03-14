using System.Text;
using System.Net.Http.Headers;
using HtmlAgilityPack;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// WEBページを解析するためのヘルパークラスです。
    /// </summary>
    [RoslynScriptHelp]
    public class HtmlHelper
    {
        public HtmlHelper(HttpClient client)
        {
            Client = client;
        }

        private HttpClient Client { get; set; }


        /// <summary>
        /// URLから取得したHTMLを指定のid属性の値で検索して文字列コレクションとして返却します。
        /// </param>
        /// <param name="id">
        /// id値
        /// </param>
        /// <param name="BasicAuthenticationId">
        /// BASIC認証する場合のユーザー名
        /// </param>
        /// <param name="BasicAuthenticationPassword">
        /// BASIC認証する場合のパスワード
        /// </param>
        /// <returns>
        /// 指定のidをもつHTML要素の文字列コレクション
        /// </returns>
        public IEnumerable<string> SelectHtmlNodesById(string url, string id, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null) => SelectHtmlNodesByQuery(url, $"//*[@id='{id}']", BasicAuthenticationId, BasicAuthenticationPassword);


        /// <summary>
        /// URLから取得したHTMLを指定のclass属性の値で検索して文字列コレクションとして返却します。
        /// </summary>
        /// <param name="url">
        /// 取得元URL
        /// </param>
        /// <param name="ClassName">
        /// XPath形式文字列
        /// </param>
        /// <param name="BasicAuthenticationId">
        /// BASIC認証する場合のユーザー名
        /// </param>
        /// <param name="BasicAuthenticationPassword">
        /// BASIC認証する場合のパスワード
        /// </param>
        /// <returns>
        /// 指定のclassをもつHTML要素の文字列コレクション
        /// </returns>
        public IEnumerable<string> SelectHtmlNodesByClassName(string url, string ClassName, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null) => SelectHtmlNodesByQuery(url, $"//*[@class='{ClassName}']", BasicAuthenticationId, BasicAuthenticationPassword);


        /// <summary>
        /// URLから取得したHTMLを指定のXPathで検索して文字列コレクションとして返却します。
        /// </summary>
        /// <param name="url">
        /// 取得元URL
        /// </param>
        /// <param name="xpath">
        /// XPath形式文字列
        /// </param>
        /// <param name="BasicAuthenticationId">
        /// BASIC認証する場合のユーザー名
        /// </param>
        /// <param name="BasicAuthenticationPassword">
        /// BASIC認証する場合のパスワード
        /// </param>
        /// <returns>
        /// 指定のXPathに合致するHTML要素の文字列コレクション
        /// </returns>
        public IEnumerable<string> SelectHtmlNodesByQuery(string url, string xpath, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null)
        {
            if (BasicAuthenticationId != null && BasicAuthenticationPassword != null)
            {
                var byteArray = Encoding.ASCII.GetBytes($"{BasicAuthenticationId}:{BasicAuthenticationPassword}");
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            var res = Client.GetAsync(url).Result;
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception($"Request Failed Code:{res.StatusCode.ToString()} ");
            }

            string html = res.Content.ReadAsStringAsync().Result;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
            return nodes?.Select(x => x.OuterHtml);
        }
    }
}
