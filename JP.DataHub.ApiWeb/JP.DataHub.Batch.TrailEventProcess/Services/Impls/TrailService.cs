using JP.DataHub.Batch.TrailEventProcess.Models;
using JP.DataHub.Batch.TrailEventProcess.Repository.Interfaces;
using JP.DataHub.Batch.TrailEventProcess.Services.Interfaces;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.SQL;

namespace JP.DataHub.Batch.TrailEventProcess.Services.Impls
{
    public class TrailService: ITrailService
    {
        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(TrailService));

        private Lazy<ITrailRepository> _lazyTrailRepository = new(() => UnityCore.Resolve<ITrailRepository>());
        private ITrailRepository _trailRepository { get => _lazyTrailRepository.Value; }

        private Lazy<IPhysicalRepositoryGroupRepository> _lazyPhysicalRepositoryGroupRepository = new(() => UnityCore.Resolve<IPhysicalRepositoryGroupRepository>());
        private IPhysicalRepositoryGroupRepository _physicalRepositoryGroupRepository { get => _lazyPhysicalRepositoryGroupRepository.Value; }


        #region 管理画面証跡の設定

        private readonly string trailFilePathKeyWord = "$TrailFilePath({0})";
        private readonly string trailParameterFilePath = "methodparameter/";
        private readonly string trailResultFilePath = "methodresult/";

        #endregion

        /// <summary>
        /// 証跡を登録します。
        /// </summary>
        /// <param name="trailInfo">証跡のリスト</param>
        /// <param name="dbType"></param>
        public void Register(TrailInfoModel trailInfo, TwowaySqlParser.DatabaseType dbType)
        {
            // 管理画面以外の証跡を登録
            if(trailInfo.TrailType != TrailTypeEnum.adm)
                _trailRepository.RegisterTrail(trailInfo, dbType);
            // 管理画面の証跡を登録
            else if (trailInfo.TrailType == TrailTypeEnum.adm)
                RegisterAdmin(trailInfo, dbType);
        }

        /// <summary>
        /// 管理画面の証跡を登録します。
        /// </summary>
        /// <param name="trailInfo">証跡のリスト</param>
        /// <param name="dbType"></param>
        private void RegisterAdmin(TrailInfoModel trailInfo, TwowaySqlParser.DatabaseType dbType)
        {
            // MethodParameter・Resultの保存接続先
            var connection = this.GetMethodParameterResultConnectionString(dbType);

            TrailAdmin deserializeModel = null;

            string paramPath = string.Empty;
            string resultPath = string.Empty;
            string regDate = DateTime.UtcNow.ToString("yyyy/MM/dd/HH") + "/";
            string regParamPath = trailParameterFilePath + regDate + Guid.NewGuid().ToString();
            string regResultPath = trailResultFilePath + regDate + Guid.NewGuid().ToString();

            try
            {
                deserializeModel = JsonConvert.DeserializeObject<TrailAdmin>(trailInfo.Detail.ToString());

                // MethodParameter・ResultはBlobに保存する
                paramPath = _trailRepository.SaveBlobFile(deserializeModel.MethodParameter?.ToString(), connection, regParamPath);
                resultPath = _trailRepository.SaveBlobFile(deserializeModel.MethodResult?.ToString(), connection, regResultPath);
            }
            catch (Exception e)
            {
                logger.Fatal($"TrailDomainService RegistAdmin Error:{ e.Message }");
                return;
            }

            var trailAdmin = new TrailAdmin
            (
                trailInfo.TrailId,
                deserializeModel.Screen,
                deserializeModel.TrailOperation,
                deserializeModel.ContollerClassName,
                deserializeModel.ActionMethodName,
                deserializeModel.OpenId,
                deserializeModel.VendorId,
                deserializeModel.IpAddress,
                deserializeModel.UserAgent,
                deserializeModel.Url,
                deserializeModel.HttpMethodType,
                deserializeModel.HttpStatusCode,
                string.IsNullOrEmpty(paramPath) ? null : string.Format(trailFilePathKeyWord, paramPath),
                string.IsNullOrEmpty(resultPath) ? null : string.Format(trailFilePathKeyWord, resultPath)
            );

            // TrailのDetailに不要なものは除く
            var jtokenTrailAdmin = JObject.FromObject(trailAdmin);
            jtokenTrailAdmin.Remove(nameof(trailAdmin.TrailId));
            jtokenTrailAdmin.Remove(nameof(trailAdmin.RegDate));

            var addTrailInfo = new TrailInfoModel
            (
                trailInfo.TrailId,
                TrailTypeEnum.adm,
                trailInfo.Result,
                jtokenTrailAdmin.ToString()
            );

            _trailRepository.RegisterTrail(addTrailInfo, dbType);
            _trailRepository.RegisterTrailAdmin(trailAdmin, dbType);
        }

        /// <summary>
        /// MethodParameterResultの保存先を管理しているRepositoryGroupから有効な接続文字列を取得する。
        /// </summary>
        /// <returns>保存先の接続文字列</returns>
        private string GetMethodParameterResultConnectionString(TwowaySqlParser.DatabaseType dbType)
        {
            // 接続先取得
            // 証跡の登録先を取得
            var repId = UnityCore.Resolve<IConfiguration>().GetValue<string>("TrailEventProcessSetting:TrailMethodParameterResultRepositoryGroup");

            // Configから保存先のRepositoyGroupIdを取得・接続先を取得する
            var repList = _physicalRepositoryGroupRepository.GetPhysicalRepository(repId, dbType);

            // 一覧取得できない
            if (repList == null)
            {
                logger.Fatal($"TrailMethodParameterResultRepository Null RepositoryGroupId={repId}");
                return string.Empty;
            }

            var phisical = repList.Where(x => !x.IsFull).FirstOrDefault()?.ConnectionString;

            // 有効な(フルじゃない)接続先がない
            if (phisical == null)
            {
                logger.Fatal("TrailMethodParameterResultRepository Connection does not exist or is full");
                return string.Empty;
            }

            return phisical;
        }

        public TrailInfoModel GetTrailInfo(string url)
            => _trailRepository.GetTrailInfo(url);

        public void DeleteTrailTempBlob(string url)
            => _trailRepository.DeleteTrailTempBlob(url);
    }
}