using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics;
using JP.DataHub.Com.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;

namespace JP.DataHub.SmartFoodChainAOP.Logic
{
    internal static class JasJudgment
    {
        /// <summary>
        /// 温度
        /// </summary>
        public static Guid TemptureId = "BC6A4FE1-4211-478A-A85E-EF1F3CE34B54".To<Guid>();
        /// <summary>
        /// 華氏
        /// </summary>
        public static Guid TemptureFahrenheitId = "711AF0FB-85D2-4D47-B380-6528A7F6DE75".To<Guid>();
        /// <summary>
        /// 摂氏
        /// </summary>
        public static Guid TemptureCelsiusId = "18E5AA34-FDE6-411F-A455-EEA588CAB34B".To<Guid>();
        /// <summary>
        /// 衝撃（複数あり）
        /// </summary>
        public static List<Guid> ImpactId = new List<Guid>()
        {
            "F41B052A-ACD7-46D4-B7D4-F496306FF1B3".To<Guid>(),
            "13943EF9-80E4-4AEC-A18B-56F3CAF0AA80".To<Guid>(),
            "CC2B4ECE-25A8-433D-A464-9493378EB520".To<Guid>(),
            "E63AFB7F-1E54-4A28-B329-FDB30A4DC776".To<Guid>(),
        };
        public static List<Guid> GravityId = new List<Guid>()
        {
            "36C17361-002A-45CD-AB5C-CF8B6BED9692".To<Guid>(),
            "92567BDB-2194-4526-9BE0-2FC2BA9080A4".To<Guid>(),
            "9A331E2F-10F4-4497-9D72-8FBDA511C3C2".To<Guid>(),
            "DB216E9F-233D-4CAC-B441-54EC72FD6D20".To<Guid>(),
        };
        public static List<Guid> MilligravityId = new List<Guid>()
        {
            "146BCA0D-B670-4308-AAC0-D224DF8E5CD4".To<Guid>(),
            "D787BB81-8019-416E-8724-93E0A7790523".To<Guid>(),
            "EF3E7AAF-417D-4F02-A0B5-DAA641A9C0C0".To<Guid>(),
            "F710436E-5DBA-4EBC-9766-9A5A25136E9F".To<Guid>(),
        };

        /// <summary>
        /// JAS判定API
        /// ★注意（次のものには対応していない）
        /// 1. センサーデータの欠損
        /// 2. センサーデータで、１つの項目が２つのセンサーデバイスで計測している
        /// </summary>
        /// <param name="judgment"></param>
        /// <param name="observationPropeties"></param>
        /// <param name="measurementUnits"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static List<JasFreshnessJudgmentFailResult> Judgment(this CropFreshnessJudgmentModel judgment, List<ObservationPropertiesModel> observationPropeties, List<MeasurementUnitsModel> measurementUnits, List<ShipmentSensorRawDataModel> raw)
        {
            var result = new List<JasFreshnessJudgmentFailResult>();

            foreach (var mesurementDetail in judgment.MeasurementDetail)
            {
                // 判定基準に合致したセンサー値を取得
                var hitraw = raw.Where(x => mesurementDetail.ObservedPropertiesCode.Contains(x.observedPropertyId)).ToList();
                // 温度：華氏を摂氏に変換
                // 衝撃：グラビティをミリグラビティに変換
                if (mesurementDetail.IsTempture() == true)
                {
                    hitraw = hitraw.ToTempture();
                }
                else if (mesurementDetail.IsImpact() == true)
                {
                    hitraw = hitraw.ToMilligravity();
                }
                // データストリーム毎に処理する
                var datastreamedRaw = hitraw.GroupBy(x => x.datastreamId);
                foreach (var group in datastreamedRaw)
                {
                    //var values = raw.Where(x => x.datestreamId == xx.Key).ToList();
                    var values = group.ToList();
                    // 閾値の条件にマッチする件数
                    foreach (var threshold in mesurementDetail.FailThreshold)
                    {
                        int count = threshold.Count(values);
                        // サンプリングレートを探す
                        var samplingRate = values.CalcSamplingRate() / 60;  // 単位は分
                        var time = count * samplingRate;
                        // 判定
                        if (count <= threshold.ThresholdLessTimes)
                        {
                            result.Add(new JasFreshnessJudgmentFailResult()
                            {
                                ObservedPropertiesCode = mesurementDetail.ObservedPropertiesCode,
                                DatastreamId = group.Key ?? "",
                                MeasurementUnitId = values[0].measurementId,
                                Threshold = new List<FailThresholdModel> { threshold },
                                Count = count
                            });
                        }
                        else if (count >= threshold.ThresholdGreatTimes)
                        {
                            result.Add(new JasFreshnessJudgmentFailResult()
                            {
                                ObservedPropertiesCode = mesurementDetail.ObservedPropertiesCode,
                                MeasurementUnitId = values[0].measurementId,
                                DatastreamId = group.Key ?? "",
                                Threshold = new List<FailThresholdModel> { threshold },
                                Count = count
                            });
                        }
                        else if (time <= threshold.ThresholdLessTotalMinute)
                        {
                            result.Add(new JasFreshnessJudgmentFailResult()
                            {
                                ObservedPropertiesCode = mesurementDetail.ObservedPropertiesCode,
                                MeasurementUnitId = values[0].measurementId,
                                DatastreamId = group.Key ?? "",
                                Threshold = new List<FailThresholdModel> { threshold },
                                Count = time
                            });
                        }
                        else if (time >= threshold.ThresholdGreatTotalMinute)
                        {
                            result.Add(new JasFreshnessJudgmentFailResult()
                            {
                                ObservedPropertiesCode = mesurementDetail.ObservedPropertiesCode,
                                MeasurementUnitId = values[0].measurementId,
                                DatastreamId = group.Key ?? "",
                                Threshold = new List<FailThresholdModel> { threshold },
                                Count = time
                            });
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// データから温度の華氏があるものを、摂氏に変換する
        /// </summary>
        /// <param name="raw">データ</param>
        /// <returns>摂氏に変換したデータ</returns>
        public static List<ShipmentSensorRawDataModel> ToTempture(this List<ShipmentSensorRawDataModel> raw)
        {
            // 温度データの華氏なら摂氏に変換
            raw.Where(x => x.observedPropertyId.To<Guid>() == TemptureId && x.measurementId.To<Guid>() == TemptureFahrenheitId.To<Guid>()).ToList().ForEach(x =>
            {
                x.measurementId = TemptureCelsiusId.ToString();
                x.observationResult = ToCelsius(x.observationResult);
            });
            return raw;
        }

        /// <summary>
        /// センサーの衝撃でグラビティがあるものを、ミリグラビティに変換する
        /// </summary>
        /// <param name="raw">データ</param>
        /// <returns>ミリグラビティに変換したデータ</returns>
        public static List<ShipmentSensorRawDataModel> ToMilligravity(this List<ShipmentSensorRawDataModel> raw)
        {
            // 衝撃のグラビティをミリグラビティに変換
            raw.Where(x => ImpactId.Contains(x.observedPropertyId.To<Guid>()) && GravityId.Contains(x.measurementId.To<Guid>())).ToList().ForEach(x =>
            {
                x.measurementId = MilligravityId[0].ToString();
                x.observationResult = x.observationResult * 1000;
            });
            return raw;
        }

        /// <summary>
        /// 華氏を摂氏に変換する
        /// </summary>
        /// <param name="val">華氏</param>
        /// <returns>摂氏</returns>
        public static float ToCelsius(float val)
        {
            return (float)Math.Round((val - 32) / 1.8f, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// グラビティからミリグラビティに変換する
        /// </summary>
        /// <param name="val">グラビティ</param>
        /// <returns>ミリグラビティ</returns>
        public static float ToMilligravity(float val)
        {
            return val * 1000;
        }

        /// <summary>
        /// データは全て温度か？
        /// </summary>
        /// <param name="measurementDetail">データ</param>
        /// <returns>true:全てのデータが温度</returns>
        private static bool IsTempture(this MeasurementDetailModel measurementDetail)
        {
            return measurementDetail.ObservedPropertiesCode?.Where(x => x.To<Guid?>() == TemptureId).Count() == measurementDetail.ObservedPropertiesCode.Count();
        }

        /// <summary>
        /// データは全て衝撃か？
        /// </summary>
        /// <param name="measurementDetail">データ</param>
        /// <returns>true:全てのデータが衝撃</returns>
        private static bool IsImpact(this MeasurementDetailModel measurementDetail)
        {
            return measurementDetail.ObservedPropertiesCode?.Where(x => ImpactId.Contains(x.To<Guid>())).Count() == measurementDetail.ObservedPropertiesCode.Count();
        }

        /// <summary>
        /// データ中の条件に合致するものの件数を返す
        /// </summary>
        /// <param name="threshold">閾値条件</param>
        /// <param name="raws">データ</param>
        /// <returns>閾値条件に合致するものの件数</returns>
        public static int Count(this FailThresholdModel threshold, List<ShipmentSensorRawDataModel> raws)
        {
            switch (threshold.Operator)
            {
                case "Greater":
                    return raws.Where(x => x.observationResult > threshold.ThresholdValue).Count();
                case "GreaterEqual":
                    return raws.Where(x => x.observationResult >= threshold.ThresholdValue).Count();
                case "Less":
                    return raws.Where(x => x.observationResult < threshold.ThresholdValue).Count();
                case "LessEqual":
                    return raws.Where(x => x.observationResult <= threshold.ThresholdValue).Count();
                default:
                    throw new Exception($"unknown Operator({threshold.Operator})");
            }
        }

        /// <summary>
        /// データのphenomenonTimeからサンプリングレートを算出する（ロジックは最頻値）
        /// </summary>
        /// <param name="raw">データ</param>
        /// <returns>サンプリングレートで単位は秒</returns>
        public static int CalcSamplingRate(this List<ShipmentSensorRawDataModel> raw)
        {
            var list = raw.Select(x => x.phenomenonTime).OrderBy(x => x).ToList();
            var diff = new List<double>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                var x = list[i + 1] - list[i];
                diff.Add(x.TotalSeconds);
            }
            return (int)Measures.Mode<double>(diff.ToArray());
        }
    }
}
