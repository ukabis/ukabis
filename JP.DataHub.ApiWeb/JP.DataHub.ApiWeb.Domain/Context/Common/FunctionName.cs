using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    class FunctionNames : IValueObject
    {
        [Key(0)]
        public List<string> Value { get; }

        public FunctionNames(List<string> functionNames)
        {
            this.Value = functionNames;
        }

        public static bool operator ==(FunctionNames me, object other) => me?.Equals(other) == true;

        public static bool operator !=(FunctionNames me, object other) => !me?.Equals(other) == true;
    }
}
