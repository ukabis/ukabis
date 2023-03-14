using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.PolicyInjection.Pipeline;
using JP.DataHub.Com.Unity;
using JP.DataHub.Service.Core.Repository;
using JP.DataHub.MVC.Controllers.Attributes;

namespace JP.DataHub.Service.Core.Impl
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class CrudServiceInjectorAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container) => new CrudServiceInjectorHandler();

        public class CrudServiceInjectorHandler : ICallHandler
        {
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var mb = FindControllerFromStackFrame();
                if (mb == null)
                {
                    return getNext()(input, getNext);
                }
                var names = GetRepositoryInfo(mb);
                if (names.ActionName == null || names.RepositoryName == null)
                {
                    return getNext()(input, getNext);
                }

                var dao = new RepositoryDaoInfo() { ClassName = names.RepositoryName, ModelName = names.RepositoryModelName, ActionName = names.ActionName, ResultType = names.ResultModel, Headers = names.Headers };
                var repository = UnityCore.Resolve<ICommonCrudRepository>();
                //var response = repository.GetList(dao);

                //var resultType = typeof(List<>).MakeGenericType(new Type[] { dao.ResultType });
                //var result = Activator.CreateInstance(resultType) as IList<object>;
                //if (response is IEnumerable<object> list)
                //{
                //    foreach (var item in list)
                //    {
                //        var converted = Activator.CreateInstance(dao.ResultType);
                //        result.Add(converted);
                //    }
                //}
                //else
                //{
                //}
                //return input.CreateMethodReturn(result);
                return null;
            }

            public static (string RepositoryName, string RepositoryModelName, string ActionName, Type ResultModel, Dictionary<string, string[]> Headers) GetRepositoryInfo(MethodBase mb)
            {
                if (mb != null)
                {
                    var controller = mb.GetCustomAttribute<MvcDaoAttribute>();
                    string repositoryName = controller?.RepositoryName;
                    string repositoryModelName = controller?.RepositoryModelName;
                    controller = mb.DeclaringType.GetCustomAttribute<MvcDaoAttribute>();
                    repositoryName = repositoryName ?? controller?.RepositoryName;
                    repositoryModelName = repositoryModelName ?? controller?.RepositoryModelName;

                    // ControllerとActionMethodから、付与するヘッダーを取得
                    var headers = new Dictionary<string, string[]>();
                    mb.DeclaringType.GetCustomAttributes<MvcDaoActionHeaderAttribute>()?.ToList().ForEach(header => headers.Add(header.Key, header.Value));
                    mb.GetCustomAttributes<MvcDaoActionHeaderAttribute>()?.ToList().ForEach(header => headers.Add(header.Key, header.Value));

                    return (repositoryName, repositoryModelName, mb.GetCustomAttributes<MvcDaoActionAttribute>()?.FirstOrDefault()?.ActionName, controller?.ResultModel, headers);
                }
                return (null, null, null, null, null);
            }


            public static MethodBase FindControllerFromStackFrame()
            {
                for (int i = 0; i < 100; i++)
                {
                    var sf = new StackFrame(i, true);
                    var mb = sf.GetMethod();
                    if (mb.DeclaringType?.ToString().EndsWith("Controller") == true)
                    {
                        return mb;
                    }
                    if (mb.DeclaringType?.ToString().EndsWith("Page") == true)
                    {
                        return mb;
                    }
                    if (mb.DeclaringType.GetInterfaces().ToList().FirstOrDefault(x => x == typeof(IPageOrController)) != null)
                    {
                        return mb;
                    }
                }
                return null;
            }

            public int Order { get => 1; set { } }
        }
    }
}
