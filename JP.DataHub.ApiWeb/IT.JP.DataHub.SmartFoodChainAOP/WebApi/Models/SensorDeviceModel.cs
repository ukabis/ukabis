using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class SensorDeviceModel
    {
        public string sensorDeviceId { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string ownerOpenId { get; set; }

        public string thingsId { get; set; }

        public string sensorId { get; set; }

        public string uuid { get; set; }

        public List<string> datastreamIds { get; set; }
    }

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

        
    }

    public class SensorDeviceRegisterExThingModel
    {
        public string name { get; set; }

        public string description { get; set; }
    }

    public class SensorDeviceRegisterExDataStreamModel
    {
        public string name { get; set; }

        public string description { get; set; }

        public string measurementUnit { get; set; }

        public string observedProperty { get; set; }

        public DatastreamsModel.PolygonModel observedArea { get; set; }

        public string observationConditionId { get; set; }
    }

    public class SensorDeviceCreateThingsModel
    {
        public string sensorDeviceId { get; set; }

        public SensorDeviceRegisterExThingModel thing { get; set; }

        public List<SensorDeviceRegisterExDataStreamModel> dataStreams { get; set; }
    }

    public class SensorDeviceCreateThingsResponseModel
    {
        public string sensorDeviceId { get; set; }
    }

    public class RegisterResultModel
    {
        public string id { get; set; }
    }
}
