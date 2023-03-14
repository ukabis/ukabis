using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Unity;
using Unity;

namespace UnitTest.JP.DataHub.Com.SQL
{
    [TestClass]
    public class UnitTest_OutsideSQL
    {
        [TestMethod]
        public void OutsideSQL_Oracle()
        {
            UnityCore.Reset();
            AbstractOutsideSQL.Clear();

            var container = new UnityContainer();
            UnityCore.Buildup(container, "UnityBuildup.json");
            container.RegisterInstance<DatabaseSettings>(new DatabaseSettings() { Type = "Oracle" });
            var tmp = container.Resolve<DatabaseSettings>();
            container.BuildupOutsideSql(this);
            var repo = container.Resolve<ITestRepositorySQL>();
            repo.SQL1.Is("SQL1.Oracle");
            repo.SQL2.Is("SQL2.Oracle");
        }

        [TestMethod]
        public void OutsideSQL_SqlServer()
        {
            UnityCore.Reset();
            AbstractOutsideSQL.Clear();

            var container = new UnityContainer();
            UnityCore.Buildup(container, "UnityBuildup.json");
            container.RegisterInstance<DatabaseSettings>(new DatabaseSettings() { Type = "SqlServer" });
            var tmp = container.Resolve<DatabaseSettings>();
            container.BuildupOutsideSql(this);
            var repo = container.Resolve<ITestRepositorySQL>();
            repo.SQL1.Is("SQL1.SqlServer");
            repo.SQL2.Is("SQL2.SqlServer");
        }
    }
}
