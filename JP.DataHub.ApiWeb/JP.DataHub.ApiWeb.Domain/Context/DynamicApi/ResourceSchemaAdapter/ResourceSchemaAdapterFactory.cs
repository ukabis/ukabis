using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.ResourceSchemaAdapter
{
    // .NET6
    internal static class ResourceSchemaAdapterFactory
    {
        public static IResourceSchemaAdapter CreateResourceSchemaAdapter(RepositoryInfo repositoryInfo, ControllerId controllerId, DataSchema controllerSchema)
        {
            switch (repositoryInfo.Type)
            {
                case RepositoryType.SQLServer2:
                    return UnityCore.Resolve<IResourceSchemaAdapter>(
                        RepositoryType.SQLServer2.ToCode(),
                        new ParameterOverride("repositoryInfo", repositoryInfo),
                        new ParameterOverride("controllerId", controllerId),
                        new ParameterOverride("controllerSchema", controllerSchema));
                case RepositoryType.OracleDb:
                    return UnityCore.Resolve<IResourceSchemaAdapter>(
                        RepositoryType.OracleDb.ToCode(),
                        new ParameterOverride("repositoryInfo", repositoryInfo),
                        new ParameterOverride("controllerId", controllerId),
                        new ParameterOverride("controllerSchema", controllerSchema));
                default:
                    return UnityCore.Resolve<IResourceSchemaAdapter>();
            }
        }
    }
}