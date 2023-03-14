using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.SQL
{
    public class AbstractOutsideSQL
    {
        protected static Dictionary<string, string> SqlCache = new Dictionary<string, string>();

        public object lockobj = new object();
        public bool ContainsFile(string key) => SqlCache.ContainsKey(key);
        public string GetSql(string key) => SqlCache[key];
        public void RemoveKey(string key) => SqlCache.Remove(key);
        public void RegisterSql(string key, string sql) => SqlCache.Add(key, sql);

        public static void Clear()
        {
            SqlCache = new Dictionary<string, string>();
        }

        /// <summary>
        /// クラスおよびメソッド名から、SQLを取得する。
        /// SQLは基本的に外部ファイルから読み込む。ファイル名の優先順位があり次の通り
        /// 例）
        /// 　　データベースのタイプ名が「Oracle」の場合
        /// 　　※プロパティの場合はプロパティ名となる（プロパティは内部的にはメソッドを呼び出す構造になっており、プレフィックスの「get_」は使われない（削除される）
        /// 　　キー：クラス名.メソッド名.sql
        /// 　　論理的優先順位
        ///         第1優先：実行パス\OutsideSql\クラス名.メソッド名.sql
        /// 　　  第2優先：実行パス\OutsideSql\クラス名.メソッド名.データベースタイプ名.sql
        /// 　　物理的優先順位
        ///         第1優先：bin\net5.0\OutsideSql\クラス名.メソッド名.sql
        ///         第2優先：bin\net5.0\OutsideSql\クラス名.メソッド名.Oracle.sql
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public string GetSql(Type type, string methodName)
        {
            // ディレクトリ名の確定、ファイル名の2パターンの候補
            var methodname = methodName.Replace("get_", "");
            var filename1 = $"{type.Name}.{methodname}.sql";
            var dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "OutsideSql");
            var key = filename1;
            filename1 = System.IO.Path.Combine(dir, filename1);

            // 既に読み込んでいればそれを返す
            lock (lockobj)
            {
                if (ContainsFile(key) == true)
                {
                    return GetSql(key);
                }
            }

            var dbsettings = UnityCore.Resolve<DatabaseSettings>();
            var filename2 = $"{type.Name}.{methodname}.{dbsettings.Type}.sql";
            filename2 = System.IO.Path.Combine(dir, filename2);

            string filename = null;
            if (System.IO.File.Exists(filename1))
            {
                filename = filename1;
            }
            else if (System.IO.File.Exists(filename2))
            {
                filename = filename2;
            }

            // ファイルから取得する。その内容はキャッシュ（Dictionary）する
            string text = null;
            if (System.IO.File.Exists(filename))
            {
                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        text = sr.ReadToEnd();
                    }
                }
            }
            else
            {
                throw new Exception($"sqlfile({key}) not found.");
            }
            lock (lockobj)
            {
                RemoveKey(key);
                RegisterSql(key, text);
            }
            return text;
        }
    }
}
