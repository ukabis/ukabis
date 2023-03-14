using JP.DataHub.Batch.TrailEventProcess.Models;
using JP.DataHub.Batch.TrailEventProcess.Repository.Interfaces;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Core.Storage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.SQL;

namespace JP.DataHub.Batch.TrailEventProcess.Repository.Impls
{
    public class TrailRepository : ITrailRepository
    {
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("Trail"));

        private readonly BlobStorageClient blobClient;

        public TrailRepository(IConfiguration config)
        {
            blobClient = new BlobStorageClient(config.GetValue<string>("ConnectionStrings:TrailLogBackUpStorage"),
                config.GetValue<string>("TrailEventProcessSetting:ContainerName"), config.GetValue<string>("TrailEventProcessSetting:RootPath"));
        }


        /// <summary>
        /// 証跡を登録します。
        /// </summary>
        /// <param name="trailInfo">証跡のリスト</param>
        /// <param name="dbType"></param>
        public void RegisterTrail(TrailInfoModel trailInfo, TwowaySqlParser.DatabaseType dbType = TwowaySqlParser.DatabaseType.Oracle)
        {
            DateTime now = DateTime.UtcNow;
            var registerData = new
            {
                trail_id = trailInfo.TrailId,
                trail_type_cd = trailInfo.TrailType.ToString(),
                detail = trailInfo.Detail?.ToString(),
                result = trailInfo.Result,
                reg_date = now
            };

            var sql = "";
            if (dbType.Equals(TwowaySqlParser.DatabaseType.Oracle))
            {
                sql = @"
MERGE INTO Trail target
USING
(
    SELECT
        :trail_id AS trail_id
    FROM DUAL
) source
ON  (
    target.trail_id = source.trail_id
    )
WHEN MATCHED THEN
    UPDATE
    SET
         trail_type_cd = :trail_type_cd
        ,result = :result
        ,detail = :detail
        ,reg_date = :reg_date
    WHERE
        trail_id = :trail_id
WHEN NOT MATCHED THEN
    INSERT
    (
        trail_id
        ,trail_type_cd
        ,result
        ,detail
        ,reg_date
    )
    VALUES
    (
        :trail_id
        ,:trail_type_cd
        ,:result
        ,:detail
        ,:reg_date
    )
";
            }
            else
            {
                sql = @"
MERGE INTO Trail AS target
USING
(
    SELECT
        @trail_id AS trail_id
) AS source
ON
    target.trail_id = source.trail_id
WHEN MATCHED THEN
    UPDATE
    SET
        trail_id = @trail_id
        ,trail_type_cd = @trail_type_cd
        ,result = @result
        ,detail = @detail
        ,reg_date = @reg_date
WHEN NOT MATCHED THEN
    INSERT
    (
        trail_id
        ,trail_type_cd
        ,result
        ,detail
        ,reg_date
    )
    VALUES
    (
        @trail_id
        ,@trail_type_cd
        ,@result
        ,@detail
        ,@reg_date
    );
";
            }
            Connection.Execute(sql, registerData);
        }

        /// <summary>
        /// 管理画面の証跡を登録します。
        /// </summary>
        /// <param name="trailAdmin">管理画面の証跡のリスト</param>
        /// <param name="dbType"></param>
        public void RegisterTrailAdmin(TrailAdmin trailAdmin, TwowaySqlParser.DatabaseType dbType = TwowaySqlParser.DatabaseType.Oracle)
        {
            DateTime now = DateTime.UtcNow;
            var registerData = new
            {
                trail_id = trailAdmin.TrailId,
                screen = trailAdmin.Screen,
                operation = trailAdmin.TrailOperation,
                contoller_class_name = trailAdmin.ContollerClassName,
                action_method_name = trailAdmin.ActionMethodName,
                vendor_id = string.IsNullOrEmpty(trailAdmin.VendorId) ? null : (Guid?)Guid.Parse(trailAdmin.VendorId),
                open_id = string.IsNullOrEmpty(trailAdmin.OpenId) ? null : (Guid?)Guid.Parse(trailAdmin.OpenId),
                http_status_code = trailAdmin.HttpStatusCode,
                http_method_type = trailAdmin.HttpMethodType,
                ip_address = trailAdmin.IpAddress,
                url = trailAdmin.Url,
                user_agent = trailAdmin.UserAgent,
                method_parameter = trailAdmin.MethodParameter?.ToString(),
                method_result = trailAdmin.MethodResult?.ToString(),
                reg_date = now
            };

            var sql = "";
            if (dbType.Equals(TwowaySqlParser.DatabaseType.Oracle))
            {
                sql = @"
MERGE INTO Admin target
USING
(
    SELECT
        :trail_id AS trail_id
    FROM DUAL
) source
ON  (
    target.trail_id = source.trail_id
    )
WHEN MATCHED THEN
    UPDATE
    SET
         screen = :screen
        ,operation = :operation
        ,contoller_class_name = :contoller_class_name
        ,action_method_name = :action_method_name
        ,open_id = :open_id
        ,vendor_id = :vendor_id
        ,ip_address = :ip_address
        ,user_agent = :user_agent
        ,url = :url
        ,http_method_type = :http_method_type
        ,http_status_code = :http_status_code
        ,method_parameter = :method_parameter
        ,method_result = :method_result
        ,reg_date = :reg_date
    WHERE
        trail_id = :trail_id
WHEN NOT MATCHED THEN
    INSERT
    (
        trail_id
        ,screen
        ,operation
        ,contoller_class_name
        ,action_method_name
        ,open_id
        ,vendor_id
        ,ip_address
        ,user_agent
        ,url
        ,http_method_type
        ,http_status_code
        ,method_parameter
        ,method_result
        ,reg_date
    )
    VALUES
    (
        :trail_id
        ,:screen
        ,:operation
        ,:contoller_class_name
        ,:action_method_name
        ,:open_id
        ,:vendor_id
        ,:ip_address
        ,:user_agent
        ,:url
        ,:http_method_type
        ,:http_status_code
        ,:method_parameter
        ,:method_result
        ,:reg_date
    )
";
            }
            else
            {
                sql = @"
MERGE INTO Admin AS target
USING
(
    SELECT
        @trail_id AS trail_id
) AS source
ON
    target.trail_id = source.trail_id
WHEN MATCHED THEN
    UPDATE
    SET
        trail_id = @trail_id
        ,screen = @screen
        ,operation = @operation
        ,contoller_class_name = @contoller_class_name
        ,action_method_name = @action_method_name
        ,open_id = @open_id
        ,vendor_id = @vendor_id
        ,ip_address = @ip_address
        ,user_agent = @user_agent
        ,url = @url
        ,http_method_type = @http_method_type
        ,http_status_code = @http_status_code
        ,method_parameter = @method_parameter
        ,method_result = @method_result
        ,reg_date = @reg_date
WHEN NOT MATCHED THEN
    INSERT
    (
        trail_id
        ,screen
        ,operation
        ,contoller_class_name
        ,action_method_name
        ,open_id
        ,vendor_id
        ,ip_address
        ,user_agent
        ,url
        ,http_method_type
        ,http_status_code
        ,method_parameter
        ,method_result
        ,reg_date
    )
    VALUES
    (
        @trail_id
        ,@screen
        ,@operation
        ,@contoller_class_name
        ,@action_method_name
        ,@open_id
        ,@vendor_id
        ,@ip_address
        ,@user_agent
        ,@url
        ,@http_method_type
        ,@http_status_code
        ,@method_parameter
        ,@method_result
        ,@reg_date
    );
";
            }
            Connection.Execute(sql, registerData);
        }

        /// <summary>
        /// 対象データをBlobに保存する
        /// </summary>
        /// <param name="targetData">Blobに保存するデータ</param>
        /// <param name="connectionString">保存接続先</param>
        /// <param name="containerName">Blobコンテナ名</param>
        /// <param name="path">ファイル名</param>
        /// <param name="extention">ファイルの拡張子、省略した場合は「.json」になります</param>
        /// <returns>保存先のPath</returns>
        public string SaveBlobFile(string targetData, string connectionString, string path, string extention = ".json")
        {
            // 保存先が無い場合はBlobに保存しない
            if (string.IsNullOrEmpty(connectionString))
            {
                return string.Empty;
            }

            var retValue = string.Empty;
            if (!string.IsNullOrEmpty(targetData))
            {
                retValue = blobClient.CopyToAsync
                (
                    path + extention,
                    new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(targetData)))
                ).ToString();
            }

            return retValue;
        }

        public TrailInfoModel GetTrailInfo(string url)
        {
            var dlData = blobClient.GetStream(GetBlobName(url));
            StreamReader reader = new StreamReader(dlData);
            var result = reader.ReadToEnd();
            var ret = JsonConvert.DeserializeObject<TrailInfoModel>(result);

            return ret;
        }

        public void DeleteTrailTempBlob(string url)
        {
            blobClient.DeleteAsync(GetBlobName(url));
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