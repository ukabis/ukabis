using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JP.DataHub.Com.Extensions
{
    public static class ObjectMap
    {
        public static T ChangeType<T>(this object obj)
        {
            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// srcのプロパティの値を、dstのプロパティの値にコピーする
        /// 条件は
        /// 1. srcはGetterがあり、dstはGetterがある
        /// 2. Publicプロパティで、インスタンスプロパティである
        /// 3. プロパティ名が同じ
        /// 4. 型が同じ
        /// 5. オブジェクトの中のオブジェクトはやりません
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void ShallowMapProperty(this object src, object dst)
        {
            if (src == null || dst == null)
            {
                return;
            }

            var stype = src.GetType();
            var dtype = dst.GetType();
            var props = stype.GetProperties().Where(x => x.CanRead == true).ToList();
            foreach (var sprop in props)
            {
                var dprop = dtype.GetProperty(sprop.Name, BindingFlags.Public | BindingFlags.Instance);
                if (dprop == null || dprop.CanWrite == false)
                {
                    continue;
                }
                if (sprop.PropertyType != dprop.PropertyType)
                {
                    continue;
                }
                var val = sprop.GetValue(src);
                dprop.SetValue(dst, val);
            }
        }
    }
}
