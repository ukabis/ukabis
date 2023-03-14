using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using CsvHelper;
using CsvHelper.Configuration;

namespace JP.DataHub.Com.Converter
{
    public class JsonCsvConverter
    {
        /// <summary>
        /// CSVからJsonに変換
        /// </summary>
        public string CsvToJson(string csv, JSchema requestSchema, bool isArray)
        {
            var jSchema = new Lazy<JSchema>(() => requestSchema);
            var list = new List<Dictionary<string, object>>();
            using (var streamReader = new StringReader(csv))
            {
                using (var reader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.CurrentCulture)))
                {
                    while (reader.Read())
                    {
                        IDictionary<string, object> records = reader.GetRecord<dynamic>() as IDictionary<string, object>;
                        if (records == null) continue;
                        var dic = records.ToDictionary(record => record.Key, record => ParseObject(record, jSchema));

                        list.Add(dic);
                    }
                }
            }

            var json = JsonConvert.SerializeObject(list);
            if (!isArray)
            {
                json = json.TrimStart('[').TrimEnd(']');
            }

            return json;
        }

        /// <summary>
        /// JSONからCSVに変換
        /// </summary>
        public bool JsonToCsv(string json, out string csv)
        {
            csv = string.Empty;
            if (!json.TrimStart().StartsWith("["))
            {
                json = "[" + json + "]";
            }

            //[]や{}がある場合はCSVにできないはずなのでfalseを返す
            var r1 = new System.Text.RegularExpressions.Regex(@".*:[ \r\n\t]*\[");
            var r2 = new System.Text.RegularExpressions.Regex(@".*:[ \r\n\t]*\{");
            if (r1.Match(json).Success || r2.Match(json).Success)
            {
                return false;
            }

            try
            {
                var jArray = JArray.Parse(json);

                StringWriter csvString = new StringWriter();
                using (var csvWriter = new CsvWriter(csvString, new CsvConfiguration(CultureInfo.CurrentCulture)))
                {
                    // プロパティ名を取得
                    // (プロパティ自体が存在しないオブジェクトを考慮して全オブジェクトのプロパティを見る)
                    // (メモリ効率のためループで回す)
                    var fields = new List<string>();
                    foreach (JObject obj in jArray)
                    {
                        var propertyNames = obj.Children<JProperty>().Select(prop => prop.Name);
                        foreach (var name in propertyNames)
                        {
                            if (!fields.Contains(name))
                            {
                                fields.Add(name);
                            }
                        }
                    }

                    foreach (var field in fields)
                    {
                        csvWriter.WriteField(field);
                    }

                    csvWriter.NextRecord();

                    foreach (JObject obj in jArray)
                    {
                        foreach (var field in fields)
                        {
                            var hasProperty = obj.Properties().Any(prop => prop.Name == field);
                            csvWriter.WriteField(hasProperty ? obj.Property(field).Value.ToString() : string.Empty);
                        }

                        csvWriter.NextRecord();
                    }
                }

                csv = csvString.ToString();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// スキーマの型で変換する
        /// </summary>
        /// <param name="record">変換前のKeyValuePair</param>
        /// <param name="jSchema">スキーマ</param>
        /// <returns>変換後のValue</returns>
        private object ParseObject(KeyValuePair<string, object> record, Lazy<JSchema> jSchema)
        {
            if (jSchema.Value == null) return record.Value;

            //スキーマがある場合には変換をかける
            //変換失敗の場合は変換前の値をそのまま入れる（最終的にバリデーションを通るのでそこでBadRequestになる）
            if (jSchema.Value.Properties.Any(x => x.Key == record.Key))
            {
                var property = jSchema.Value.Properties.FirstOrDefault(x => x.Key == record.Key);
                switch (property.Value.Type & ~JSchemaType.Null)
                {
                    case JSchemaType.Number:
                        if (decimal.TryParse(record.Value.ToString(), out var resultDecimal))
                        {
                            return resultDecimal;
                        }
                        break;
                    case JSchemaType.Integer:
                        if (int.TryParse(record.Value.ToString(), out var resultInt))
                        {
                            return resultInt;
                        }
                        break;
                    case JSchemaType.Boolean:
                        if (bool.TryParse(record.Value.ToString(), out var resultBool))
                        {
                            return resultBool;
                        }
                        break;
                }
            }
            return record.Value;
        }
    }

}
