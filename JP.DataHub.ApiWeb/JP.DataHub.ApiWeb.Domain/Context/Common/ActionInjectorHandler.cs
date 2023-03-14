using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.ActionInjector;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ActionInjectorHandler : IValueObject
    {
        public Type Value { get; }

        private IActionInjector Instance { get; set; }

        public object Target
        {
            set { if (Instance != null) Instance.Target = value; }
            get { return Instance?.Target; }
        }

        public object ReturnValue
        {
            set { if (Instance != null) Instance.ReturnValue = value; }
            get { return Instance?.ReturnValue; }
        }

        public ActionInjectorHandler(Type type)
        {
            Value = type;
            if (Value != null)
            {
                Instance = Activator.CreateInstance(Value) as IActionInjector;
            }
        }

        public void Execute(Action action)
        {
            if (Instance != null)
            {
                Instance.Execute(action);
            }
        }

        public static bool operator ==(ActionInjectorHandler me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ActionInjectorHandler me, object other) => !me?.Equals(other) == true;
    }
}
