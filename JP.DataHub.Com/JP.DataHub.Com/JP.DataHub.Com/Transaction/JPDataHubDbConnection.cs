using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Dapper;

namespace JP.DataHub.Com.Transaction
{
    public class JPDataHubDbConnection : AbstractJPDataHubDbConnection
    {
       public JPDataHubDbConnection()
        {
        }

        public JPDataHubDbConnection(JPDataHubDbConnectionParam param)
            : base(param)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionString">データベース接続文字列</param>
        /// <param name="providerName">データベースプロバイダー名</param>
        /// <param name="isMultithread">マルチスレッドモードで実行するかどうか</param>
        public JPDataHubDbConnection(string connectionString, string providerName, bool isMultithread = false)
            : base(connectionString, providerName, isMultithread)
        {
            // これはトランザクションの管理をしてもらう（そのインスタンスで管理してもらう）
            base.IsTransactionManage = true;
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Dispose Resources.
        /// </summary>
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~JPDataHubDbConnection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose Resources.
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected virtual new void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (Connection != null)
                {
                    System.Console.WriteLine("DbClose");
                    Close();
                    Connection.Dispose();
                    //Connection = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Dapper.SimpleCRUDのprivateメソッド
        /// <summary>
        /// テーブル名をエンティティ型から取得します。
        /// </summary>
        /// <param name="t">エンティティ型</param>
        /// <returns>テーブル名</returns>
        private string GetTableName(Type t)
        {
            var m = typeof(SimpleCRUD).GetMethod("GetTableName", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(Type) }, null);
            return (string)m.Invoke(null, new object[] { t });
        }

        /// <summary>
        /// プロパティ情報からSelect句を作成します。
        /// </summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="props">プロパティ情報</param>
        private void BuildSelect(StringBuilder sb, IEnumerable<PropertyInfo> props)
        {
            var m = typeof(SimpleCRUD).GetMethod("BuildSelect", BindingFlags.Static | BindingFlags.NonPublic);
            m.Invoke(null, new object[] { sb, props });
        }

        /// <summary>
        /// プロパティ情報からSelect句を作成します。
        /// </summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="props">プロパティ情報</param>
        /// <param name="sourceEntity"></param>
        /// <param name="whereConditions"></param>
        private void BuildWhere(StringBuilder sb, IEnumerable<PropertyInfo> props, object sourceEntity, object whereConditions = null)
        {
            var m = typeof(SimpleCRUD).GetMethod("BuildWhere", BindingFlags.Static | BindingFlags.NonPublic);
            m.Invoke(null, new object[] { sb, props, sourceEntity, whereConditions });
        }
        #endregion

    }
}
