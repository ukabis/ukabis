using JP.DataHub.Batch.TrailEventProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.SQL;

namespace JP.DataHub.Batch.TrailEventProcess.Repository.Interfaces
{
    /// <summary>
    /// 証跡登録リポジトリのインターフェースです。
    /// </summary>
    public interface ITrailRepository
    {
        /// <summary>
        /// 証跡を登録します。
        /// </summary>
        /// <param name="trailInfo">証跡のリスト</param>
        /// <param name="dbType"></param>
        void RegisterTrail(TrailInfoModel trailInfo, TwowaySqlParser.DatabaseType dbType);

        /// <summary>
        /// 管理画面の証跡を登録します。
        /// </summary>
        /// <param name="trailAdmin">管理画面の証跡のリスト</param>
        /// <param name="dbType"></param>
        void RegisterTrailAdmin(TrailAdmin trailAdmin, TwowaySqlParser.DatabaseType dbType);

        /// <summary>
        /// 対象データをBlobに保存する
        /// </summary>
        /// <param name="targetData">Blobに保存するデータ</param>
        /// <param name="connectionString">保存接続先</param>
        /// <param name="containerName">Blobコンテナ名</param>
        /// <param name="path">ファイル名</param>
        /// <param name="extention">ファイルの拡張子、省略した場合は「.json」になります</param>
        /// <returns>保存先のPath</returns>
        string SaveBlobFile(string targetData, string connectionString, string path, string extention = ".json");

        TrailInfoModel GetTrailInfo(string url);

        void DeleteTrailTempBlob(string url);
    }
}