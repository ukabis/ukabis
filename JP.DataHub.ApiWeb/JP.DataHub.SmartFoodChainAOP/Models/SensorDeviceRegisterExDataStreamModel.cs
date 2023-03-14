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
    public class SensorDeviceRegisterExDataStreamModel
    {
        #region Mapper
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SensorDeviceRegisterExDataStreamModel, DatastreamModel>().ReverseMap();
            });

            return mappingConfig.CreateMapper();
        });

        private static IMapper s_mapper { get => s_lazyMapper.Value; }
        #endregion

        public string name { get; set; }

        public string description { get; set; }

        public string measurementUnit { get; set; }

        public string observedProperty { get; set; }

        public PolygonModel observedArea { get; set; }

        public string observationConditionId { get; set; }

        /// <summary>
        /// モデルの未設定プロパティを修正する。
        /// </summary>
        /// <param name="thingName">センサー群名。</param>
        public void FixProperties(string thingName)
        {
            // 名前が未設定なら"{センサー群名}-Datastream"を設定
            this.name = string.IsNullOrEmpty(this.name) ? $"{thingName}-Datastream" : this.name;
        }

        public DatastreamModel ToDataStream(string sensorId, string thingId)
        {
            var datastream = s_mapper.Map<DatastreamModel>(this);
            datastream.key = Guid.NewGuid().ToString();
            datastream.unitOfMeasurementId = this.measurementUnit;
            datastream.observedPropertyId = this.observedProperty;
            datastream.sensorId = sensorId;
            datastream.thingId = thingId;
            return datastream;
        }
    }
}
