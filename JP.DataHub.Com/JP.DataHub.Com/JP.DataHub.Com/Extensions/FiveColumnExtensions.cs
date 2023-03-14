using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.Extensions
{
    public static class FiveColumnExtensions
    {
        public static T UpdateFiveColumn<T>(this object obj)
        {
            UpdateFiveColumn(obj);
            return (T)obj;
        }

        public static object UpdateFiveColumn(this object obj)
        {
            if (obj != null)
            {
                var dataContainer = DataContainerUtil.ResolveDataContainer();
                var now = dataContainer.GetDateTimeUtil().GetUtc(dataContainer.GetDateTimeUtil().LocalNow);
                var openid = dataContainer.OpenId;
                var props = obj.GetType().GetProperties();
                props.Where(x => x.Name == "reg_date").FirstOrDefault()?.Update<DateTime>(x => x == new DateTime(), obj, now);
                props.Where(x => x.Name == "reg_username").FirstOrDefault()?.Update<string>(x => x == null, obj, openid);
                props.Where(x => x.Name == "upd_date").FirstOrDefault()?.Update<DateTime>(obj, now);
                props.Where(x => x.Name == "upd_username").FirstOrDefault()?.Update<string>(obj, openid);
                props.Where(x => x.Name == "is_active").FirstOrDefault()?.Update<bool>(obj, true);
            }
            return obj;
        }

        public static void Update<T>(this PropertyInfo prop, object obj, T val)
        {
            if (prop.PropertyType == typeof(T))
            {
                prop.SetValue(obj, val, null);
            }
        }

        public static void Update<T>(this PropertyInfo prop, Func<T,bool> func, object obj, T val)
        {
            if (prop.PropertyType == typeof(T))
            {
                if (func((T)prop.GetValue(obj, null)) == true)
                {
                    prop.SetValue(obj, val, null);
                }
            }
        }
    }
}
