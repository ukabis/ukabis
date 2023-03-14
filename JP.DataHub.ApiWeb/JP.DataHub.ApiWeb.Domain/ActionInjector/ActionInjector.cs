using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal class ActionInjector : IActionInjector
    {
        public object ReturnValue { get; set; }

        public object Target { get; set; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
            });
            return mappingConfig.CreateMapper();
        });

        protected static IMapper Mapper => s_lazyMapper.Value;

        public virtual void Execute(Action action)
        {
            action();
        }
    }
}
