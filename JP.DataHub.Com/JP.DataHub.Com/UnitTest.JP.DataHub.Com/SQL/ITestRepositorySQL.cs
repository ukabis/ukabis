using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.SQL.Attributes;

namespace UnitTest.JP.DataHub.Com.SQL
{
    [OutsideSql]
    public interface ITestRepositorySQL
    {
        string SQL1 { get; }
        string SQL2 { get; }
    }
}
