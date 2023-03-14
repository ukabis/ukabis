using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Validations.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.All, AllowMultiple = false)]
    public class DbMapAttribute : Attribute
    {
        public string Database { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }

        public DbMapAttribute(object columnenum)
        {
            var type = columnenum.GetType();        // <= TableName class
            var field = type.GetField(columnenum.ToString());
            var dbmap = field.GetCustomAttribute<DbMapAttribute>();
            if (dbmap != null)
            {
                Database = dbmap.Database;
                Table = dbmap.Table;
                Column = dbmap.Column;
            }
        }

        public DbMapAttribute(string database, string table, string column)
        {
            Database = database;
            Table = table;
            Column = column;
        }
    }
}
