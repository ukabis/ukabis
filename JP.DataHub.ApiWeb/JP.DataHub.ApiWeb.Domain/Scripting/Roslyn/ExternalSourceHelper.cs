using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// WEBからCSVを取得するためのヘルパークラスです。
    /// </summary>
    [RoslynScriptHelp]
    public class ExternalSourceHelper
    {
        public ExternalSourceHelper(HttpClient client)
        {
            Client = client;
        }

        public class ParseConfiguration
        {
            public bool HasHeaderRecord { get; set; } = true;
            public Encoding Encoding { get; set; } = Encoding.UTF8;
            public bool IgnoreBlankLines { get; set; } = true;
            public bool DetectColumnCountChanges { get; set; } = true;
            public char Comment { get; set; } = '#';
        }

        private static Lazy<IMapper> _Mapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg => cfg.CreateMap<ParseConfiguration, CsvConfiguration>()
                .ConstructUsing(x => new CsvConfiguration(CultureInfo.CurrentCulture))
                .ReverseMap());
            return mappingConfig.CreateMapper();
        });
        private static IMapper Mapper
        {
            get
            {
                return _Mapper.Value;
            }
        }

        private HttpClient Client { get; set; }


        /// <summary>
        /// URLから取得したCSVをディクショナリに変換します。
        /// </summary>
        /// <param name="url">
        /// 取得元URL
        /// </param>
        /// <param name="config">
        /// CSVをパースする際の設定
        /// </param>
        /// <param name="BasicAuthenticationId">
        /// BASIC認証する場合のユーザー名
        /// </param>
        /// <param name="BasicAuthenticationPassword">
        /// BASIC認証する場合のパスワード
        /// </param>
        /// <returns>
        /// オブジェクトのリスト
        /// </returns>
        public IEnumerable<IDictionary<string, object>> ParseCsvFromUrlToDictionary(string url, ParseConfiguration config = null, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null)
        {
            if (BasicAuthenticationId != null && BasicAuthenticationPassword != null)
            {
                var byteArray = Encoding.ASCII.GetBytes($"{BasicAuthenticationId}:{BasicAuthenticationPassword}");
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            var response = Client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request Failed Code:{response.StatusCode.ToString()} ");
            }

            var csvByteArray = response.Content.ReadAsStreamAsync().Result;

            var csvConfig = config != null ? Mapper.Map<CsvConfiguration>(config) : new CsvConfiguration(CultureInfo.CurrentCulture);

            var resultList = new List<Dictionary<string, object>>();
            using (CsvReader reader = new CsvReader(new StreamReader(csvByteArray, csvConfig.Encoding), csvConfig))
            {
                while (reader.Read())
                {
                    var record = reader.GetRecord<dynamic>() as IDictionary<string, object>;
                    var dic = new Dictionary<string, object>();
                    foreach (var kvp in record)
                    {
                        dic.Add(kvp.Key, kvp.Value);
                    }
                    resultList.Add(dic);

                }
            }
            return resultList;
        }

        /// <summary>
        /// URLから取得したCSVを任意のオブジェクトへマッピングします。
        /// </summary>
        /// <param name="url">
        /// 取得元URL
        /// </param>
        /// <param name="mapper">
        /// CSVの項目を戻り値のオブジェクトへマッピングする関数(AutoMapperの書式に準拠)
        /// </param>
        /// <param name="config">
        /// CSVをパースする際の設定
        /// </param>
        /// <param name="validator">
        /// CSVの内容をバリデーションする関数
        /// </param>
        /// <param name="BasicAuthenticationId">
        /// BASIC認証する場合のユーザー名
        /// </param>
        /// <param name="BasicAuthenticationPassword">
        /// BASIC認証する場合のパスワード
        /// </param>
        /// <returns>
        /// オブジェクトのリスト
        /// </returns>
        public IEnumerable<T> ParseCsvFromUrlToObject<T>(string url, Func<IDictionary<string, object>, T> mapper = null, ParseConfiguration config = null, Func<IDictionary<string, object>, Boolean> validator = null, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null)
        {
            if (BasicAuthenticationId != null && BasicAuthenticationPassword != null)
            {
                var byteArray = Encoding.ASCII.GetBytes($"{BasicAuthenticationId}:{BasicAuthenticationPassword}");
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            var response = Client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request Failed Code:{response.StatusCode.ToString()} ");
            }

            var csvByteArray = response.Content.ReadAsStreamAsync().Result;

            var csvConfig = config != null ? Mapper.Map<CsvConfiguration>(config) : new CsvConfiguration(CultureInfo.CurrentCulture);
            validator = validator ?? ((x) => { return true; });

            IEnumerable<T> resultList = null;
            //AutoMapperに従ってマッピング
            if (mapper != null)
            {
                resultList = ParseCsvWithAutoMapper<T>(validator, mapper, csvByteArray, csvConfig);
            }
            //ヘッダーとクラス定義の名前でマッピング
            else if (csvConfig.HasHeaderRecord)
            {
                resultList = ParseCsvWithHeader<T>(validator, csvByteArray, csvConfig);
            }
            //ヘッダーの項目順とコンストラクタの引数の順番でマッピング
            else
            {
                resultList = ParseCsvWithItemSequence<T>(validator, csvByteArray, csvConfig);
            }

            return resultList;
        }

        /// <summary>
        /// AutoMapperを使用してCSVを任意のオブジェクトへマッピングする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="mapper"></param>
        /// <param name="csvByteArray"></param>
        /// <param name="csvConfig"></param>
        /// <returns></returns>
        private static List<T> ParseCsvWithAutoMapper<T>(Func<IDictionary<string, object>, bool> validator, Func<IDictionary<string, object>, T> mapper, Stream csvByteArray, CsvConfiguration csvConfig)
        {
            var resultList = new List<T>();
            using (CsvReader reader = new CsvReader(new StreamReader(csvByteArray, csvConfig.Encoding), csvConfig))
            {
                while (reader.Read())
                {
                    var record = reader.GetRecord<dynamic>() as IDictionary<string, object>;
                    if (validator(record))
                    {
                        resultList.Add(mapper(record));
                    }
                }
            }

            return resultList;
        }

        /// <summary>
        /// ヘッダーの項目名とクラス定義の名前でCSVを任意のオブジェクトへマッピングする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="csvByteArray"></param>
        /// <param name="csvConfig"></param>
        /// <returns></returns>
        private static List<T> ParseCsvWithHeader<T>(Func<IDictionary<string, object>, bool> validator, Stream csvByteArray, CsvConfiguration csvConfig) //where T : new()
        {
            var resultList = new List<T>();
            using (CsvReader reader = new CsvReader(new StreamReader(csvByteArray, csvConfig.Encoding), csvConfig))
            {
                while (reader.Read())
                {
                    var record = reader.GetRecord<dynamic>() as IDictionary<string, object>;
                    if (validator(record))
                    {
                        //var obj = new T();
                        var obj = Activator.CreateInstance(typeof(T));
                        foreach (var prop in typeof(T).GetProperties())
                        {
                            prop.SetValue(obj, Convert.ChangeType(record[prop.Name], prop.PropertyType));
                        }
                        resultList.Add((T)obj);
                    }

                }
            }

            return resultList;
        }

        /// <summary>
        /// ヘッダーの項目順とコンストラクタの引数の順番でCSVを任意のオブジェクトへマッピングする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="csvByteArray"></param>
        /// <param name="csvConfig"></param>
        /// <returns></returns>
        private static List<T> ParseCsvWithItemSequence<T>(Func<IDictionary<string, object>, bool> validator, Stream csvByteArray, CsvConfiguration csvConfig)
        {
            var resultList = new List<T>();
            var constructor = typeof(T).GetConstructors()[0];
            var prms = constructor.GetParameters();
            using (CsvReader reader = new CsvReader(new StreamReader(csvByteArray, csvConfig.Encoding), csvConfig))
            {
                while (reader.Read())
                {
                    var record = reader.GetRecord<dynamic>() as IDictionary<string, object>;
                    if (validator(record))
                    {
                        var args = new List<object>();
                        for (int i = 0; i < reader.ColumnCount; i++)
                        {
                            args.Add(Convert.ChangeType(reader[i], prms[i].ParameterType));
                        }
                        resultList.Add((T)constructor.Invoke(args.ToArray()));
                    }
                }
            }

            return resultList;
        }
    }
}
