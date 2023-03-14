using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Core.Storage;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using Microsoft.Extensions.Configuration;
using System.Text;
namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class LoggingRepository : AbstractRepository, ILoggingRepository
    {
		private readonly string loggingFilePathKeyWord = "$LoggingFilePath";

		private Lazy<IJPDataHubDbConnection> _lazyLoggingConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Logging"));
		private IJPDataHubDbConnection _loggingConnection { get => _lazyLoggingConnection.Value; }
		private IRepositoryGroupRepository _repositoryGroupRepository => _lazyRepositoryGroupRepository.Value;
		private Lazy<IRepositoryGroupRepository> _lazyRepositoryGroupRepository = new(() => UnityCore.Resolve<IRepositoryGroupRepository>());


		/// <summary>
		/// LogIdに紐づくLogging情報を一件取得する
		/// </summary>
		/// <param name="logId">LogId</param>
		/// <returns>Logging情報</returns>
		public LoggingQueryModel GetLogging(string logId)
		{
			if (string.IsNullOrEmpty(logId))
			{
				throw new ArgumentNullException(nameof(logId));
			}
            if (!Guid.TryParse(logId, out _))
            {
				throw new NotFoundException();
			}

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
			if (dbSettings.Type == "Oracle")
			{
				sql = @"
SELECT
	log_id as LogId
	,controller_id as ControllerId
	,api_id as ApiId
	,request_date as RequestDate
	,execute_time as ExecuteTime
	,httpstatus_code as HttpStatusCode
	,vendor_id as VendorId
	,system_id as SystemId
	,open_id as OpenId
	,provider_vendorid as ProviderVendorId
	,provider_systemid as ProviderSystemId
	,client_ipaddress as ClientIpAddress
	,controller_name as ControllerName
	,action_name as ActionName
	,httpmethodtype as HttpMethodType
	,url as Url
	,querystring as QueryString
	,request_headers as RequestHeaders
	,request_contenttype as RequestContentType
	,request_contentlength as RequestContentLength
	,requestbody as RequestBody
	,response_headers as ResponseHeaders
	,response_contenttype as ResponseContentType
	,response_contentlength as ResponseLength
	,response_body as ResponseBody
	,request_date_ymd as RequestDateYmd
	,request_date_ymdh as RequestDateYmdH
	,request_date_ymdhm as RequestDateYmdHm
FROM 
	LOGGING
WHERE
	log_id = /*ds logId*/'id'
";
			}
			else
			{
                sql = @"
SELECT
	log_id as LogId
	,controller_id as ControllerId
	,api_id as ApiId
	,request_date as RequestDate
	,execute_time as ExecuteTime
	,httpstatus_code as HttpStatusCode
	,vendor_id as VendorId
	,system_id as SystemId
	,open_id as OpenId
	,provider_vendorid as ProviderVendorId
	,provider_systemid as ProviderSystemId
	,client_ipaddress as ClientIpAddress
	,controller_name as ControllerName
	,action_name as ActionName
	,httpmethodtype as HttpMethodType
	,url as Url
	,querystring as QueryString
	,request_headers as RequestHeaders
	,request_contenttype as RequestContentType
	,request_contentlength as RequestContentLength
	,requestbody as RequestBody
	,response_headers as ResponseHeaders
	,response_contenttype as ResponseContentType
	,response_contentlength as ResponseLength
	,response_body as ResponseBody
	,request_date_ymd as RequestDateYmd
	,request_date_ymdh as RequestDateYmdH
	,request_date_ymdhm as RequestDateYmdHm
FROM 
	Logging
WHERE
	log_id = @logId
";
            }
            var param = new { logId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new { logId = true });
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _loggingConnection.QuerySingleOrDefault<LoggingQueryModel>(twowaySql.Sql, dynParams);
		}

		/// <summary>
		/// Logggin情報からRequestBodyを取得する
		/// </summary>
		/// <param name="logId">logId</param>
		/// <returns>RequestBody</returns>
		public (Stream Stream, string MimeType) GetRequestBody(string logId)
		{
			return this.GetBlobStream(logId, true);
		}

		/// <summary>
		/// Logggin情報からResponseBodyを取得する
		/// </summary>
		/// <param name="logId">logId</param>
		/// <returns>ResponseBody</returns>
		public (Stream Stream, string MimeType) GetResponseBody(string logId)
		{
			return this.GetBlobStream(logId, false);
		}


		/// <summary>
		/// LogIdに紐づくLoggin情報を取得し、RequestまたはResponseを返す
		/// </summary>
		/// <param name="logId">logId</param>
		/// <param name="isRequest">Requestを取得する場合はtrue、Responseを取得する場合はfalseを指定</param>
		/// <returns>RequestまたはResponseのStreamとMimeType</returns>
		private (Stream Stream, string MimeType) GetBlobStream(string logId, bool isRequest)
		{
			// Logging情報を取得
			var log = this.GetLogging(logId);
			// Logging情報取得できず
			if (log == null)
			{
				throw new NotFoundException();
			}
			return this.GetData(log, isRequest);
		}

		/// <summary>
		/// Loggin情報を取得し、RequestまたはResponseを返す
		/// </summary>
		/// <param name="model">Logging情報</param>
		/// <param name="isRequest">Requestを取得する場合はtrue、Responseを取得する場合はfalseを指定</param>
		/// <returns>RequestまたはResponseのStreamとMimeType</returns>
		private (Stream Data, string ContentType) GetData(LoggingQueryModel model, bool isRequest)
		{
			var target = isRequest ? model.RequestBody : model.ResponseBody;
			var contentType = isRequest ? model.RequestContentType : model.ResponseContentType;

			if (string.IsNullOrEmpty(target))
			{
				throw new NotFoundException();
			}

			// $LoggingFilePathで始まっていない場合はDBの値をそのまま返す(Blob対応前用)
			if (!target.StartsWith(loggingFilePathKeyWord))
			{
				return (new MemoryStream(Encoding.UTF8.GetBytes(target)), contentType);
			}
			// $LoggingFilePathで始まる場合はBlobから取得して返す
			else
			{
				var url = target.TrimStart(loggingFilePathKeyWord.ToCharArray()).Trim('(', ')');

				// URLから接続先を取得・Blobから取得する
				var con = this.GetConnectionString(url);

				if (string.IsNullOrEmpty(con))
				{
					// DBに保存されているUrlの接続先が見つからない
					throw new NotFoundException($"Logging RequestResponse Connection does not exist. url = {url}");
				}
				var client = new BlobStorageClient(con, GetBlobContainerName(url), "");
				return (client.GetStream(GetBlobName(url)), contentType);
			}
		}

		/// <summary>
		/// Urlを元にLogging用のリポジトリグループから一致する接続先を取得する
		/// </summary>
		/// <param name="url">Logging情報に登録されているBlobファイルUrl</param>
		/// <returns>接続先</returns>
		private string GetConnectionString(string url)
		{
			// AccountName取得
			var accountName = this.GetBlobUrlAccountName(url);
			var repositoryGroupId = UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:LoggingResponseRequestRepositoryGroupId");
			// Logging用のRepositoryGroupから接続先の一覧を取得する
			var conList = _repositoryGroupRepository.GetPhysicalRepository(repositoryGroupId);

			foreach (var con in conList)
			{
				var items = con.ConnectionString.Split(';').Select(x => x.Split('=')).ToDictionary(x => x[0].ToUpper(), x => x[1].ToUpper());
				if (items["ACCOUNTNAME"] == accountName.ToUpper())
				{
					return con.ConnectionString;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// UrlからBlobのAccountNameを取得する
		/// </summary>
		/// <param name="url">Url</param>
		/// <remarks>Urlが「https://jpdatahubdev.blob.core.windows.net/xxx」であれば、jpdatahubdevを取得</remarks>
		/// <returns>AccountName</returns>
		private string GetBlobUrlAccountName(string url)
		{
			var uri = new Uri(url);
			return uri.Host.Split('.')[0];
		}

		private string GetBlobContainerName(string url)
		{
			return url.Replace("https://", "").Split('/')[1];
		}
		private string GetBlobName(string url)
		{
			var uri = new Uri(url);
			return uri.AbsolutePath.Substring(GetBlobContainerName(url).Length + 2);
		}

	}
}