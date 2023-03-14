using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.DataContainer
{
    public static class DataContainerUtil
    {
        private static Lazy<Type> s_lazyGetDataContainerType = new Lazy<Type>(() => UnityCore.ResolveOrDefault<Type>("DataContainerType") ?? typeof(IDataContainer));

        public static Type DataContainerType()
            => s_lazyGetDataContainerType.Value;

        public static IDataContainer ResolveDataContainer()
        {
            return (IDataContainer)UnityCore.Resolve(DataContainerType());
        }
    }
}
