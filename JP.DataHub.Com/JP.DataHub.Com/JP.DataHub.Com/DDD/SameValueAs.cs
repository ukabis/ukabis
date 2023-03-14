using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.DDD
{
    public static class SameValueAs
    {
        /// <summary>
        /// オブジェクトのプロパティがすべて同値か比較する
        /// </summary>
        /// <param name="me">比較する側のオブジェクト</param>
        /// <param name="other">比較される側のオブジェクト</param>
        /// <returns>すべて同値の場合はtrue。それ以外の場合はfalse。</returns>
        public static bool SameValueAsTrue(object me, object other)
        {
            if (me == null && other == null)
            {
                return true;
            }
            else if (me == null || other == null)
            {
                return false;
            }

            if (me.GetType() != other.GetType())
            {
                return false;
            }

            if (me is string == false && me is IEnumerable<object> == true)
            {
                IEnumerator<object> enumOther = ((IEnumerable<object>)other).GetEnumerator();
                IEnumerator<object> enumMe = ((IEnumerable<object>)me).GetEnumerator();
                for (; ; )
                {
                    bool nextOther = enumOther.MoveNext();
                    bool nextMe = enumMe.MoveNext();
                    if (nextOther != nextMe)
                    {
                        return false;
                    }
                    if (nextOther == false && nextMe == false)
                    {
                        break;
                    }
                    if (SameValueAsTrue(enumOther.Current, enumMe.Current) == false)
                    {
                        return false;
                    }
                }

                return true;
            }

            var meType = me.GetType();
            if (me is string || meType.IsEnum || meType.IsPrimitive)
            {
                return me.Equals(other);
            }

            var compareResults = me.GetType().GetProperties()
                .Select(meProp => new
                {
                    Me = meProp.GetValue(me),
                    Other = other.GetType().GetProperty(meProp.Name).GetValue(other)
                })
                .AsParallel()
                .Select(prop =>
                {
                    if (prop.Me == null && prop.Other == null)
                    {
                        return true;
                    }

                    if (prop.Other == null || prop.Me == null)
                    {
                        return false;
                    }

                    if (prop.Other is string == false && prop.Other is IEnumerable<object> == true)
                    {
                        return SameValueAsTrue(prop.Other, prop.Me);
                    }

                    return prop.Other.Equals(prop.Me);
                });

            return !compareResults.Any(c => c == false);
        }
    }
}