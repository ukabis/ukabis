using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Log;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    internal class CreateRegisterVersionActionInjector : ActionInjector
    {
        public override void Execute(Action action)
        {
            var target = Target as AbstractDynamicApiAction;
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var parallesExceptions = new ConcurrentQueue<Exception>();
            var parallelResult = target.DynamicApiDataStoreRepository
                .AsParallel()
                .AsOrdered()
                .Select(repository =>
                {
                    var threadPerRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                    Mapper.Map<IPerRequestDataContainer, IPerRequestDataContainer>(perRequestDataContainer, threadPerRequestDataContainer);
                    try
                    {
                        var result = repository.ResourceVersionRepository.CreateRegisterVersion(target.RepositoryKey);
                        //var result = repository.CreateRegisterVersion(target.RepositoryKey, repository.RepositoryInfo);
                        return result;
                    }
                    catch (NotImplementedException ex)
                    {
                        parallesExceptions.Enqueue(ex);
                        return null;
                    }
                })
                .Where(result => result != null)
                .ToList();
            if (parallelResult.Count == 0)
            {
                var exceptionList = parallesExceptions.ToList();
                if (exceptionList.Count > 1)
                {
                    throw new AggregateException(exceptionList);
                }
                else if (exceptionList.Count == 0)
                {
                    throw new NotImplementedException();
                }
                {
                    throw exceptionList[0];
                }
            }
            var tokenRegisterVersion = JToken.FromObject($"{{ \"RegisterVersion\" : {parallelResult[0].Value} }}");
            ReturnValue = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(tokenRegisterVersion.ToString(), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
        }
    }
}
