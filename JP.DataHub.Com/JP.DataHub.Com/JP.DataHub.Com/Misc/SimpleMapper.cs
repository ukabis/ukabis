using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Misc
{
    public static class SimpleMapper
    {
        public static T Map<T>(this object src) => (T)src.Map(typeof(T));

        private static void ListAdd(object list, object val)
        {
            var mi = list.GetType().GetMethod("Add");
            if (mi != null)
            {
                mi.Invoke(list, new object[] { val });
            }
        }

        private static object CreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        private static object CreateInstance(Type type, object obj)
        {
            try
            {
                return Activator.CreateInstance(type, new object[] { obj });
            }
            catch
            {
                return null;
            }
        }

        private static bool IsPrimitive(Type type) => type.IsPrimitive == true || type == typeof(string);

        private static bool IsPrimitive(Type type1, Type type2)
        {
            return IsPrimitive(type1) && IsPrimitive(type2);
        }

        public static object Map(this object src, Type type)
        {
            if (src == null)
            {
                return null;
            }
            var srcType = src.GetType();
            if (IsPrimitive(srcType, type) == true)
            {
                return src;
            }
            var propsTarget = type.GetProperties();
            var target = CreateInstance(type);
            if (target == null)
            {
                var xxx = new string("1");
                target = CreateInstance(type, new object[] { src });
                return target;
            }
            var propsSource = src.GetType().GetProperties().ToList();
            foreach (var propSource in propsSource)
            {
                var val = propSource.GetValue(src);
                if (val == null)
                {
                    // 何もしない。初期値がnullだから
                }
                else
                {
                    var propTarget = propsTarget.FirstOrDefault(x => x.Name == propSource.Name);
                    if (propTarget != null)
                    {
                        if (propSource.PropertyType.IsArray == true)
                        {
                            if (propTarget.PropertyType.IsArray == false && propSource.PropertyType.IsGenericType == false)
                            {
                                // マッピング不可能
                            }
                            else
                            {
                                throw new Exception("NotSupported");
                            }
                        }
                        if (propSource.PropertyType.IsGenericType && propSource.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                        {
                            if (val != null && val is IEnumerable<object>)
                            {
                                var listtype = typeof(List<>).MakeGenericType(propTarget.PropertyType.GenericTypeArguments[0]);
                                var converted = Activator.CreateInstance(listtype);
                                foreach (object item in (IEnumerable<object>)val)
                                {
                                    var item_converted = item.Map(propTarget.PropertyType.GenericTypeArguments[0]);
                                    ListAdd(converted, item_converted);
                                }
                                propTarget.SetValue(target, converted);
                            }
                            else
                            {
                                throw new Exception("NotSupported");
                                // else  dictionary....
                            }
                        }
                        else
                        {
                            if (val != null && val.GetType() != propTarget.PropertyType && !propSource.PropertyType.IsGenericType)
                            {
                                if (val.TryConvert(propTarget.PropertyType, out var newVal))
                                {
                                    val = newVal;
                                }
                                else
                                {
                                    val = val.Map(propTarget.PropertyType);
                                }
                            }
                            propTarget.SetValue(target, val);
                        }
                    }
                }
            }
            return target;
        }
    }
}