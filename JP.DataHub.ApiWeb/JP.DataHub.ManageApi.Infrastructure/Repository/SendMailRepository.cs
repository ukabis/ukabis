using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using Unity.Resolution;
using NVelocity;
using NVelocity.App;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Management;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class SendMailRepository : AbstractRepository, ISendMailRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Management"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        private static string s_sendGridWebApiKey { get => s_appConfig.GetValue<string>("SendGridWebApiKey"); }

        public async Task<bool> SendMailAsync(string mailTemplateCd, Dictionary<string, object> parameters, Dictionary<string, string> loggingKeyValue = null)
        {
            try
            {
                if (string.IsNullOrEmpty(mailTemplateCd))
                {
                    throw new ArgumentNullException(nameof(mailTemplateCd));
                }

                if (parameters == null)
                {
                    throw new ArgumentNullException(nameof(parameters));
                }

                // テンプレートエンジンにパラメータを設定
                Velocity.Init();
                var mailBodyContext = new VelocityContext();
                foreach (var parameter in parameters)
                {
                    mailBodyContext.Put(parameter.Key, parameter.Value);
                }

                // メール送信
                var mailTemplate = GetMailTemplate(mailTemplateCd);
                var ret = await SendMail(GetReplaceTemplateData(mailBodyContext, mailTemplate.from_mailaddress),
                    GetReplaceTemplateData(mailBodyContext, mailTemplate.to_mailaddress),
                    GetReplaceTemplateData(mailBodyContext, mailTemplate.cc_mailaddress),
                    GetReplaceTemplateData(mailBodyContext, mailTemplate.title),
                    GetReplaceTemplateData(mailBodyContext, mailTemplate.body),
                    mailTemplate.remark).ConfigureAwait(false);
                if (ret.StatusCode != HttpStatusCode.Accepted)
                {
                    LogSendmailError(mailTemplateCd, loggingKeyValue, ret.StatusCode, null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogSendmailError(mailTemplateCd, loggingKeyValue, null, ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// メールテンプレート情報取得
        /// </summary>
        private DB_MailTemplate GetMailTemplate(string sendMailTamplateCd)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
	from_mailaddress
	,to_mailaddress
	,cc_mailaddress
	,title
	,body
	,remark
FROM
	MAIL_TEMPLATE
WHERE
    mail_template_cd = /*ds mail_template_cd*/'1' 
	AND is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
	from_mailaddress
	,to_mailaddress
	,cc_mailaddress
	,title
	,body
	,remark
FROM
	MailTemplate
WHERE
    mail_template_cd = @mail_template_cd
	AND is_active = 1
";
            }

            if (string.IsNullOrEmpty(sendMailTamplateCd))
            {
                throw new ArgumentNullException("sendMailTamplateCd");
            }
            var param = new { mail_template_cd = sendMailTamplateCd };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.QuerySingleOrDefault<DB_MailTemplate>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// パラメータ埋め込み
        /// </summary>
        private string GetReplaceTemplateData(VelocityContext mailBodyContext, string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return null;
            }

            using (var writer = new StringWriter())
            {
                Velocity.Evaluate(mailBodyContext, writer, null, target);
                return writer.ToString();
            }
        }

        /// <summary>
        /// メール送信エラーログ出力
        /// </summary>
        private void LogSendmailError(string mailTemplateCd, Dictionary<string, string> loggingKeyValue = null, HttpStatusCode? httpStausCode = null, Exception ex = null)
        {
            var mailTemplateCdKey = "MailTemplateCode";
            var httpStatusKey = "httpStatus";

            loggingKeyValue = (loggingKeyValue ?? new Dictionary<string, string>());
            if (!loggingKeyValue.ContainsKey(mailTemplateCdKey))
            {
                loggingKeyValue.Add(mailTemplateCdKey, mailTemplateCd);
            }
            if (!loggingKeyValue.ContainsKey(httpStatusKey))
            {
                loggingKeyValue.Add(httpStatusKey, httpStausCode?.ToString());
            }

            // アラート対象とするためFatalで出力
            var log = new JPDataHubLogger(this.GetType());
            log.Fatal($"SendGridメール送信エラー({string.Join(", ", loggingKeyValue.Select(x => $"{x.Key} = {x.Value}"))})", ex);
        }

        /// <summary>
        /// メール送信
        /// </summary>
        private async Task<Response> SendMail(string from, string to, string cc, string subject, string body, string remark)
        {
            // メール送信には、SendGrid公式ライブラリを使用
            // メール送信データ設定
            var msg = new SendGridMessage();

            // メール送信元
            msg.SetFrom(new SendGrid.Helpers.Mail.EmailAddress(from));

            // メール送信先
            var toList = ParseMailAddressList(to);
            msg.AddTos(toList.Select(x => new SendGrid.Helpers.Mail.EmailAddress(x)).ToList());

            // メール送信CC先
            // CCに値がない場合に設定するとメール送信に失敗する
            // TOとCCに同じアドレスが含まれているとメール送信に失敗する(BadRequest)
            var ccList = ParseMailAddressList(cc).Where(x => !toList.Contains(x));
            if (ccList.Count() > 0)
            {
                msg.AddCcs(ccList.Select(x => new SendGrid.Helpers.Mail.EmailAddress(x)).ToList());
            }

            // 件名
            msg.SetSubject(subject);

            // メール本文:Text については、改行文字を、メール用改行文字にReplaceする
            msg.AddContent(MimeType.Text, body.Replace("\\n", "\r\n"));

            // メール送信は、SendGrid WebAPIを使用するため、そのAPIキーを、Web.configより取得
            var client = UnityCore.Resolve<ISendGridClient>(new ParameterOverride("apiKey", s_sendGridWebApiKey), new ParameterOverride("version", "v3"));

            // メール送信
            return await client.SendEmailAsync(msg).ConfigureAwait(false);
        }

        /// <summary>
        /// mailaddressにはカンマ区切りのメールアドレスを指定する
        /// カンマで分割してメールアドレスのリストを返す
        /// </summary>
        /// <param name="mailaddress"></param>
        /// <returns></returns>
        private IList<string> ParseMailAddressList(string mailaddress)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(mailaddress))
            {
                result.AddRange(mailaddress.Split(';').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Distinct());
            }
            return result;
        }
    }
}
