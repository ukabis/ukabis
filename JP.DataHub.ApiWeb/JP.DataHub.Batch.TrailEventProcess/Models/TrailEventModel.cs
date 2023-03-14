using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.TrailEventProcess.Models
{
    public class TrailEventModel
    {
        public string TrailId { get; set; }

        public TrailType TrailType { get; set; }

        public bool Result { get; set; }

        public object Detail { get; set; }
        public string UnregisteredBackupTrailUrl
        {
            get
            {
                dynamic detailUrl = JsonConvert.DeserializeObject(Detail.ToString());
                return detailUrl.UnregisteredBackupTrailUrl;
            }
        }
    }

    public enum TrailType
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