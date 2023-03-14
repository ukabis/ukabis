using JP.DataHub.Batch.TrailEventProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.SQL;

namespace JP.DataHub.Batch.TrailEventProcess.Services.Interfaces
{
    public interface ITrailService
    {
        /// <summary>
        /// 証跡を登録します。
        /// </summary>
        /// <param name="trailInfo">証跡のリスト</param>
        /// <param name="dbType"></param>
        void Register(TrailInfoModel trailInfo, TwowaySqlParser.DatabaseType dbType);

        TrailInfoModel GetTrailInfo(string url);

        void DeleteTrailTempBlob(string url);
    }
}
