using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class HttpHeaderModel
    {
        /// <summary>フィールド名</summary>
        public string FieldName { get; }

        /// <summary>値</summary>
        public string Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="fieldName">フィールド名</param>
        /// <param name="value">値</param>
        public HttpHeaderModel(string fieldName, string value)
        {
            FieldName = fieldName;
            Value = value;
        }
    }
}
