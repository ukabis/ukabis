using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Lifetime;

namespace JP.DataHub.MVC.Unity
{
    public class PerRequestLifetimeManager : LifetimeManager, IInstanceLifetimeManager, IFactoryLifetimeManager, ITypeLifetimeManager
    {
        private readonly object _lifetimeKey = new object();
        public override object GetValue(ILifetimeContainer container = null)
        {
            return UnityMiddleware.GetValue(_lifetimeKey);
        }
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            UnityMiddleware.SetValue(_lifetimeKey, newValue);
        }
        public override void RemoveValue(ILifetimeContainer container = null)
        {
            var disposable = GetValue() as IDisposable;

            disposable?.Dispose();

            UnityMiddleware.SetValue(_lifetimeKey, null);
        }
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerRequestLifetimeManager();
        }
    }
}
