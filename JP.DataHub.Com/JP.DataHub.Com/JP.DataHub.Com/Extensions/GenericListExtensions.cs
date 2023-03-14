using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Extensions
{
    public static class GenericListExtensions
    {
        public static T FindOnce<T>(this List<T> list)
        {
            if (list.Count() != 1)
            {
                throw new Exception("listの要素が1以外です。FindOnceは1要素だけでその要素を返すものです");
            }
            return list[0];
        }
    }
}
