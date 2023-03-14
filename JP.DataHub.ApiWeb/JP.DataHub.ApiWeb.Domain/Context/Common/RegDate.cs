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
    internal record RegDate : IValueObject
    {
        [Key(0)]
        public DateTime Value { get; }

        public RegDate(DateTime regDate)
        {
            this.Value = regDate;
        }
    }
}
