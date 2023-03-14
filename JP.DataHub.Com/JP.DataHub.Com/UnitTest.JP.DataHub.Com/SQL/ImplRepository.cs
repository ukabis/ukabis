using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.SQL.Attributes;

namespace UnitTest.JP.DataHub.Com.SQL
{
    [OutsideSql]
    public interface IImplRepository
    {
        string SQL1 { get; }
        string SQL2 { get; }
    }

    public class ImplRepository : IImplRepository
    {
        public string SQL1 { get; }
        public string SQL2 { get; }
    }
}
