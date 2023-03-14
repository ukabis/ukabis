using AutoMapper;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
    public class SensorDeviceRegisterExModel
    {
        public string name { get; set; }

        public string description { get; set; }

        public string sensorId { get; set; }

        public string sensorSerialNumber { get; set; }

        public string sensorUuid { get; set; }

        public bool isDefaultDataStream { get; set; }

        public SensorDeviceRegisterExThingModel thing { get; set; }

        public List<SensorDeviceRegisterExDataStreamModel> dataStreams { get; set; }

        /// <summary>
        /// モデルの未設定プロパティを修正する。
        /// </summary>
        /// <param name="sensor">センサー情報。</param>
        public void FixProperties(SensorModel sensor)
        {
            // シリアル番号が未設定ならUUIDを設定
            this.sensorSerialNumber = string.IsNullOrEmpty(this.sensorSerialNumber) ? this.sensorUuid : this.sensorSerialNumber;
            // 名前が未設定なら"{メーカー}-{モデル番号}-{シリアル番号}"を設定
            this.name = string.IsNullOrEmpty(this.name) ? $"{sensor?.manufacturer}-{sensor?.modelNumber}-{this.sensorSerialNumber}" : this.name;
            // センサー群のプロパティを修正
            if (this.thing == null)
            {
                this.thing = new SensorDeviceRegisterExThingModel();
            }
            this.thing.FixProperties(this.name);
            // データストリームのプロパティを修正
            this.dataStreams?.ForEach(stream => stream.FixProperties(this.thing.name));
        }

        public SensorDeviceModel ToSensorDevice(string openId, string sensorId, string thingId, List<string> datastreamIds)
        {
            return new SensorDeviceModel()
            {
                name = this.name,
                description = this.description,
                ownerOpenId = openId,
                thingsId = thingId,
                sensorId = sensorId,
                uuid = this.sensorUuid,
                datastreamIds = datastreamIds
            };
        }
    }
}
