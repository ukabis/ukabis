using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Dapper
{
    /// <summary>
    /// BulkCopy用のDataReaderです。
    /// </summary>
    /// <typeparam name="T">テーブル定義の型</typeparam>
    public class SqlBulkCopyDataReader<T> : IDataReader
    {
        private IEnumerator<T> dataEnumerator;
        private List<PropertyInfo> propertyInfos;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="dataList">登録データリスト</param>
        public SqlBulkCopyDataReader(IEnumerable<T> dataList)
        {
            dataEnumerator = dataList.GetEnumerator();
            propertyInfos = typeof(T).GetProperties().ToList();
        }

        /// <summary>
        /// 指定されたカラム名の順序を返します。
        /// </summary>
        /// <param name="name">カラム名</param>
        /// <returns>カラムの順序</returns>
        public int GetOrdinal(string name)
            => propertyInfos.FindIndex(pi => pi.Name == name);

        /// <summary>
        /// 指定された順序のカラムの値を返します。
        /// </summary>
        /// <param name="i">カラムの順序</param>
        /// <returns>カラムの値</returns>
        public object GetValue(int i)
            => propertyInfos[i].GetValue(dataEnumerator.Current);

        public int Depth => 1;

        public bool IsClosed => dataEnumerator == null;

        public int RecordsAffected => -1;

        public int FieldCount => propertyInfos.Count;

        public bool NextResult() => false;

        public DataTable GetSchemaTable() => null;

        public bool Read() => dataEnumerator.MoveNext();

        public void Close() => Dispose();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dataEnumerator != null)
                {
                    dataEnumerator.Dispose();
                    dataEnumerator = null;
                }
            }
        }

        #region NotImplemented
        public object this[int i] => throw new NotImplementedException();

        public object this[string name] => throw new NotImplementedException();

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}