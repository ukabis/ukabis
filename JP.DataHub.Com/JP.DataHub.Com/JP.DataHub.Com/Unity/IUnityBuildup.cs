using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace JP.DataHub.Com.Unity
{
    public interface IUnityBuildup
    {
        void Buildup(IUnityContainer container, IConfiguration Configuration);
    }
}
