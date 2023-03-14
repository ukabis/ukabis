using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal class FuncInfomation : IValueObject
    {
        public string FunctionId { get; }

        public string FunctionName { get; }

        public string FunctionDetail { get; }

        public bool IsEnable { get; }

        public bool IsActive { get; }

        public FuncInfomation(string functionId, string functionName, string functionDetail, bool isEnable, bool isActive)
        {
            FunctionId = functionId ?? throw new ArgumentNullException(nameof(functionId));
            FunctionName = functionName ?? throw new ArgumentNullException(nameof(functionName));
            FunctionDetail = functionDetail ?? throw new ArgumentNullException(nameof(functionDetail));
            IsEnable = isEnable;
            IsActive = isActive;
        }

        public static bool operator ==(FuncInfomation me, object other) => me?.Equals(other) == true;

        public static bool operator !=(FuncInfomation me, object other) => !me?.Equals(other) == true;
    }
}
