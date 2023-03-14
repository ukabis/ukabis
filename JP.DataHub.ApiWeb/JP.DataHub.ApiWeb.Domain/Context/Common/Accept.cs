using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record Accept : IValueObject
    {
        // 返却可能リスト
        internal static List<string> practicableList = new()
        {
            MediaTypeConst.ApplicationJson,
            MediaTypeConst.ApplicationXml,
            MediaTypeConst.ApplicationGeoJson,
            MediaTypeConst.ApplicationVndGeoJson,
            MediaTypeConst.TextCsv,
            MediaTypeConst.ApplicationProblemJson,
            MediaTypeConst.ApplicationProblemXml,
            MediaTypeConst.TextXml
        };

        // 変換可能リスト
        internal static Dictionary<string, List<string>> compatibleList = new()
        {
            {
                MediaTypeConst.ApplicationJson,
                new List<string>() { MediaTypeConst.ApplicationProblemJson }
            },
            {
                MediaTypeConst.ApplicationXml,
                new List<string>() { MediaTypeConst.ApplicationProblemXml }
            },
        };

        public string Value { get; }
        public List<AcceptValue> AcceptValueList { get; }

        public Accept(string value)
        {
            Value = string.IsNullOrEmpty(value) ? "*/*" : value;
            AcceptValueList = CreateAcceptValueList();

            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(Accept me, object other) => me?.Equals(other) == true;

        public static bool operator !=(Accept me, object other) => !me?.Equals(other) == true;

        public List<MediaType> GetResponseMediaType(MediaType mediaType)
        {
            if (mediaType?.Value == null)
            {
                return new List<MediaType> { new MediaType(practicableList[0]) };
            }
            var madiaTypeSplit = mediaType.Value.Split('/');
            var result = new List<MediaType>();

            foreach (var av in AcceptValueList)
            {
                if (av.Type == "*" && av.Resource == "*")
                {
                    //指定なし(*/*)の場合はそのまま返却
                    result.Add(new MediaType(mediaType.Value));
                }
                else if (av.Type == madiaTypeSplit[0] && (av.Resource == madiaTypeSplit[1] || av.Resource == "*"))
                {
                    //同様のものがある場合はそのまま返却
                    result.Add(new MediaType(mediaType.Value));
                }
                else if (compatibleList.TryGetValue(av.MediaType, out var compTypes) && compTypes.Any(x => x == mediaType.Value))
                {
                    //互換性のあるものがある場合はそのまま返却
                    result.Add(new MediaType(mediaType.Value));

                }
                else
                {
                    foreach (var practicable in practicableList)
                    {
                        var ps = practicable.Split('/');
                        if (av.Type == ps[0] && av.Resource == ps[1])
                        {
                            result.Add(new MediaType(practicable));
                        }
                    }
                }
            }

            if (result.Count > 0) return result;

            //デフォルトJson
            return new List<MediaType> { new MediaType(practicableList[0]) };
        }

        private List<AcceptValue> CreateAcceptValueList()
        {
            List<AcceptValue> acceptValues = new List<AcceptValue>();
            var values = Value.Split(',');
            foreach (var item in values)
            {
                var ps = item.Split(';');
                var vs = ps[0].Split('/');
                acceptValues.Add(new AcceptValue()
                {
                    Type = vs[0].Trim(),
                    Resource = vs.Length > 1 ? vs[1].Trim() : "*",
                    Weight = ps.Length > 1 ? GetWeight(ps[1]) : 1
                });
            }
            return acceptValues.OrderByDescending(x => x.Weight).ToList();
        }

        private double GetWeight(string q)
        {
            double result = 1;
            var values = q.Split('=');
            if (values.Length <= 1)
            {
                return result;
            }

            if (values[0].Trim() == "q" && double.TryParse(values[1], out result))
            {
                return result;
            }
            return result;
        }

        internal class AcceptValue
        {
            public string Type { get; set; }
            public string Resource { get; set; }
            public double Weight { get; set; }
            public string MediaType => $"{Type}/{Resource}";
        }

    }

    internal static class AcceptExtension
    {
        public static Accept ToAccept(this string str) => str == null ? null : new Accept(str);
    }
}


