using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal interface IActionInjector
    {
        object ReturnValue { get; set; }

        object Target { get; set; }

        void Execute(Action action);
    }
}
