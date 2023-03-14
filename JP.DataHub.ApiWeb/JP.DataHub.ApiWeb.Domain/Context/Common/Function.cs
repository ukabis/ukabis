using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record Function : IValueObject
    {
        /// <summary>
        /// ファンクションID
        /// </summary>
        public string FunctionId { get; }

        /// <summary>
        /// ファンクション名
        /// </summary>
        public string FunctionName { get; }

        /// <summary>
        /// ファンクション詳細
        /// </summary>
        public string FunctionDetail { get; }

        /// <summary>
        /// 使用可能かどうか
        /// </summary>
        public bool IsEnable { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="functionId">ファンクションID</param>
        /// <param name="functionName">ファンクション名</param>
        /// <param name="functionDetail">ファンクション詳細</param>
        /// <param name="isEnable">使用可能かどうか</param>
        public Function(string functionId, string functionName, string functionDetail, bool isEnable)
        {
            FunctionId = functionId;
            FunctionName = functionName;
            FunctionDetail = functionDetail;
            IsEnable = isEnable;
        }

        public static bool operator ==(Function me, object other) => me?.Equals(other) == true;

        public static bool operator !=(Function me, object other) => !me?.Equals(other) == true;
    }
}