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
    public class SensorDeviceRegisterExThingModel
    {
        #region Mapper
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SensorDeviceRegisterExThingModel, ThingModel>().ReverseMap();
            });

            return mappingConfig.CreateMapper();
        });

        private static IMapper s_mapper { get => s_lazyMapper.Value; }
        #endregion

        public string name { get; set; }

        public string description { get; set; }

        /// <summary>
        /// モデルの未設定プロパティを修正する。
        /// </summary>
        /// <param name="sensorDeviceName">センサーデバイス名。</param>
        public void FixProperties(string sensorDeviceName)
        {
            // 名前が未設定なら"{センサーデバイス名}-Thing"を設定
            this.name = string.IsNullOrEmpty(this.name) ? $"{sensorDeviceName}-Thing" : this.name;
        }

        public ThingModel ToThing()
        {
            var thing = s_mapper.Map<ThingModel>(this);
            thing.key = Guid.NewGuid().ToString();
            thing.thingType = ThingModel.ThingType.physical.ToString();
            return thing;
        }
    }
}
