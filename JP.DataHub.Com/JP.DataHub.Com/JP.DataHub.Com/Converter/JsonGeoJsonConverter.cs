using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace JP.DataHub.Com.Converter
{
    /// <summary>
    /// JsonからGeoJsonのコンバーター
    /// </summary>
    public class JsonGeoJsonConverter
    {
        private enum PointType
        {
            None,
            Point,
            Polygons
        }

        /// <summary>
        /// JSONからGeoJsonに変換
        /// </summary>
        public bool JsonToGeoJson(string json, out string geoJson)
        {
            geoJson = string.Empty;
            try
            {
                var convertObj = JToken.Parse(json as string);
                PointType pointType = PointType.None;
                IGeoJSONObject convertedObj = null;
                if (convertObj.Type == JTokenType.Array)
                {
                    pointType = CheckPointType(convertObj.First);
                    convertedObj = ConvertArray((JArray)convertObj, pointType);
                }
                else
                {
                    pointType = CheckPointType(convertObj);
                    convertedObj = ConvertObject((JObject)convertObj, pointType);
                }

                geoJson = JsonConvert.SerializeObject(convertedObj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private FeatureCollection ConvertArray(JArray jArray, PointType pointType)
        {
            FeatureCollection retFeatureCollection = new FeatureCollection();
            foreach (var jobject in jArray)
            {
                retFeatureCollection.Features.Add(CreateFeature(jobject as JObject, pointType));
            }
            return retFeatureCollection;
        }

        private FeatureCollection ConvertObject(JObject data, PointType pointType)
        {
            FeatureCollection retFeatureCollection = new FeatureCollection();
            retFeatureCollection.Features.Add(CreateFeature(data, pointType));
            return retFeatureCollection;
        }

        private Feature CreateFeature(JObject data, PointType pointType)
        {
            var properties = new Dictionary<string, object>();
            foreach (var propertyData in data.Properties())
            {
                if (!IgnoreProperty(propertyData))
                {
                    properties.Add(propertyData.Name, propertyData.Value);
                }
            }
            IGeometryObject geometryObject = null;
            if (pointType == PointType.Point)
            {
                geometryObject = CreatePoint(data);
            }
            else
            {
                geometryObject = CreatePolygon(data);
            }
            return new Feature(geometryObject, properties);
        }

        private Point CreatePoint(JObject data)
        {
            return new Point(new Position(data["Latitude"].Value<double>(), data["Longitude"].Value<double>()));
        }

        private Polygon CreatePolygon(JObject data)
        {
            var retLineStrings = new List<LineString>();
            foreach (var polygon in data["Polygons"].ToObject<List<PolygonModel>>())
            {
                var retPositions = new List<Position>();
                foreach (var coordinate in polygon.Coordinates)
                {
                    retPositions.Add(new Position(coordinate.Latitude, coordinate.Longitude));
                }
                retLineStrings.Add(new LineString(retPositions));
            }
            return new Polygon(retLineStrings);
        }

        private bool IgnoreProperty(JProperty property)
        {
            var ignoreKeys = new[] { "Longitude", "Latitude", "Polygons" };
            return ignoreKeys.Contains(property.Name);
        }

        private PointType CheckPointType(JToken value)
        {
            if (value["Longitude"] != null && value["Latitude"] != null)
            {
                return PointType.Point;
            }
            if (value["Polygons"] != null)
            {
                return PointType.Polygons;
            }
            throw new ArgumentException("Value is not Point or Polygons Data");
        }

        public class PolygonModel
        {
            public List<CoordinateModel> Coordinates { get; set; }
        }

        public class CoordinateModel
        {
            public string Latitude { get; set; }
            public string Longitude { get; set; }
        }
    }
}