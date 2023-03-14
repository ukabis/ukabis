using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]

    /// RepositoryTypeに付与するRdbmsリポジトリ用の属性
    internal class RdbmsRepositoryAttribute : Attribute
    {
        public RdbmsRepositoryAttribute()
        {
        }
    }
}
