using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    class SystemFuncInfomation : IValueObject
    {
        public string SystemFunctionId { get; }

        public string SystemId { get; }

        public string FunctionId { get; }

        public bool IsActive { get; }

        public SystemFuncInfomation(string systemFunctionId, string systemId, string functionId, bool isActive)
        {
            SystemFunctionId = systemFunctionId ?? throw new ArgumentNullException(nameof(systemFunctionId));
            SystemId = systemId ?? throw new ArgumentNullException(nameof(systemId));
            FunctionId = functionId ?? throw new ArgumentNullException(nameof(functionId));
            IsActive = isActive;
        }

        public static bool operator ==(SystemFuncInfomation me, object other) => me?.Equals(other) == true;

        public static bool operator !=(SystemFuncInfomation me, object other) => !me?.Equals(other) == true;
    }
}