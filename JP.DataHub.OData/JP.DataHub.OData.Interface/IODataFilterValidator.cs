using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.OData.Interface
{
    // .NET6
    /// <summary>
    /// ODataのfilterのバリデーションを行う。
    /// </summary>
    public interface IODataFilterValidator
    {
        bool IsFilterValueUnescapeEnabled { get; }

        object ValidateAndFormat(string propertyName, object value);

        bool IsBooleanProperty(string propertyName);
    }
}
