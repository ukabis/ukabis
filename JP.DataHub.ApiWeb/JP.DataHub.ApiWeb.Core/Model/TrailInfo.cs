using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public class TrailInfo
    {
        private static object lockObj = new object();

        public string TrailId { get; set;}

        public TrailTypeEnum TrailType { get; set; }

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

        public TrailInfo(string trailId, TrailTypeEnum trailType, bool result, object detail)
        {
            if (string.IsNullOrEmpty(trailId))
            {
                lock (lockObj)
                {
                    TrailId = Guid.NewGuid().ToString();
                }
            }
            else
            {
                TrailId = trailId;
            }

            TrailType = trailType;
            Result = result;
            Detail = detail;
        }

        public TrailInfo()
        {
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
}