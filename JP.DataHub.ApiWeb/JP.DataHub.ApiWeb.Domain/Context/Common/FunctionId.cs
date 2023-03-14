using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record FunctionId : IValueObject
    {
        /// <summary>
        /// ファンクションID
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="functionId">ファンクションID</param>
        public FunctionId(string functionId)
        {
            Value = functionId;
        }

        public static bool operator ==(FunctionId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(FunctionId me, object other) => !me?.Equals(other) == true;
    }

    internal static class FunctionIdExtension
    {
        public static FunctionId ToFunctionId(this string val) => val == null ? null : new FunctionId(val);
    }
}
