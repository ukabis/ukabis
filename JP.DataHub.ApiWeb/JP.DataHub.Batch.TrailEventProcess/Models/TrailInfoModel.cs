using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.TrailEventProcess.Models
{
    public class TrailInfoModel
    {
        public string TrailId { get; set; }

        public TrailTypeEnum TrailType { get; set; }

        public bool Result { get; set; }

        public object Detail { get; set; }

        public TrailInfoModel(string trailId, TrailTypeEnum trailType, bool result, object detail) 
        {
            TrailId = trailId;
            TrailType = trailType;
            Result = result;
            Detail = detail;
        }
        
    }
    public enum TrailTypeEnum
    {
        /// <summary>
        /// 管理画面
        /// </summary>
        adm,
        /// <summary>
        /// Azure Portal
        /// </summary>
        azl,
        /// <summary>
        /// SQL Server
        /// </summary>
        sql,
    }
}
