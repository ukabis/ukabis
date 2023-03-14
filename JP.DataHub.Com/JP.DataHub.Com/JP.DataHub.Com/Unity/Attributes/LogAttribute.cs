using JP.DataHub.Com.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace JP.DataHub.Com.Unity.Attributes
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class LogAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LogHandler();
        }
        [DebuggerStepThrough]
        public class LogHandler : ICallHandler
        {
            [DebuggerStepThrough]
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var Log = new JPDataHubLogger(input.MethodBase.DeclaringType.FullName);
                IMethodReturn result = null;
                bool isProperty = IsProperty(input);
                try
                {
                    if (!isProperty)
                    {
                        result = DisplayLogExecution(input, getNext, Log);
                    }
                    else
                    {
                        result = NonLogExecution(input, getNext);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return result;
            }

            private bool IsProperty(IMethodInvocation input)
            {
                if ((input.MethodBase.Name.StartsWith("set_") || input.MethodBase.Name.StartsWith("get_")) && input.MethodBase.ReflectedType.GetProperty(input.MethodBase.Name.Replace("set_", "").Replace("get_", "")) != null)
                {
                    return true;
                }
                return false;
            }
            
            private IMethodReturn DisplayLogExecution(IMethodInvocation input, GetNextHandlerDelegate getNext, JPDataHubLogger log)
            {
                IMethodReturn result;
                var isOutputStackLog = UnityCore.ResolveOrDefault("OutputStackLog", false);
                var sb = new StringBuilder();
                for (int i = 0; i < input.Arguments.Count; i++)
                {
                    var arg = input.Arguments[i];
                    if (sb.Length > 0) sb.Append(", ");
                    if (input.Arguments.ParameterName(i) == "password")
                    {
                        sb.AppendFormat("{0}:(XXXX)", input.Arguments.ParameterName(i));
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1}", input.Arguments.ParameterName(i), arg);
                    }
                }

                var start = DateTime.UtcNow;
                if (isOutputStackLog)
                {
                    log.Trace_SetCallerInfo("Start ", input.MethodBase.Name, new StackFrame(5, true)?.GetFileLineNumber() ?? 0, null);
                }
                else
                {
                    log.Trace($"Start {input.MethodBase.Name}");
                }

                result = getNext()(input, getNext);
                
                var task = result.ReturnValue as Task;
                var method = input.MethodBase as MethodInfo;
                var returnType = method.ReturnType;
                //AOPされるメソッドが非同期の場合の処理
                // 参考:https://docs.microsoft.com/ja-jp/archive/msdn-magazine/2014/february/async-programming-intercepting-asynchronous-methods-using-unity-interception
                if (returnType == typeof(Task))
                {
                    return input.CreateMethodReturn(CreateWrapperTask(input, log, result, isOutputStackLog, start, task), input);
                }
                else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    //OPTIMIZE Reflectionで取得したMethodInfoをキャッシュする
                    var genMethod = this.GetType().GetMethod(nameof(CreateGenericWrapperTask), BindingFlags.Instance | BindingFlags.NonPublic);
                    return input.CreateMethodReturn(genMethod.MakeGenericMethod(new Type[] { returnType.GenericTypeArguments[0] }).Invoke(this, new object[] { input,log,result, isOutputStackLog, start, task}), input);
                }
                else
                {
                    var end = DateTime.UtcNow;
                    if (result.Exception != null)
                    {
                        log.Error_SetCallerInfo(result.Exception.Message, result.Exception, input.MethodBase.Name, new StackFrame(5, true)?.GetFileLineNumber() ?? 0, null);
                    }
                    else
                    {
                        if (isOutputStackLog)
                        {
                            log.Trace_SetCallerInfo($"End({(end - start):mm\\:ss\\.fff}) result:{result.ReturnValue}", null, input.MethodBase.Name, new StackFrame(5, true)?.GetFileLineNumber() ?? 0);
                        }
                        else
                        {
                            log.Trace($"End {input.MethodBase.Name}({(end - start):mm\\:ss\\.fff}) result:{result.ReturnValue}");
                        }
                    }
                    return result;
                }
            }

            private Task CreateGenericWrapperTask<T>(IMethodInvocation input, JPDataHubLogger log, IMethodReturn result, bool isOutputStackLog, DateTime start, Task task) => DoCreateGenericWrapperTask(input, log, result, isOutputStackLog, start, (Task<T>)task);
            private async Task<T> DoCreateGenericWrapperTask<T>(IMethodInvocation input, JPDataHubLogger log, IMethodReturn result, bool isOutputStackLog, DateTime start, Task<T> task)
            {
                T returnValue = default;
                try
                {
                    returnValue = await task.ConfigureAwait(false);
                }
                catch (Exception)
                {
                    LogTaskExecution(input, log, returnValue, isOutputStackLog, start, task);
                    throw;
                }
                LogTaskExecution(input, log, returnValue, isOutputStackLog, start, task);
                return returnValue;
            }

            private async Task CreateWrapperTask(IMethodInvocation input, JPDataHubLogger log, IMethodReturn result, bool isOutputStackLog, DateTime start, Task task)
            {
                try
                {
                    await task.ConfigureAwait(false);

                }
                catch (Exception)
                {
                    LogTaskExecution(input, log, null, isOutputStackLog, start, task);
                    throw;
                }
                LogTaskExecution(input, log, null, isOutputStackLog, start, task);
            }

            private static void LogTaskExecution(IMethodInvocation input, JPDataHubLogger log, object result, bool isOutputStackLog, DateTime start, Task task)
            {
                var end = DateTime.UtcNow;
                if (task.Exception != null)
                {
                    log.Error_SetCallerInfo(task.Exception.Message, task.Exception, input.MethodBase.Name, new StackFrame(5, true)?.GetFileLineNumber() ?? 0, null);
                }
                else
                {
                    if (isOutputStackLog)
                    {
                        log.Trace_SetCallerInfo($"End({(end - start):mm\\:ss\\.fff}) result:{result ?? "void"}", null, input.MethodBase.Name, new StackFrame(5, true)?.GetFileLineNumber() ?? 0);
                    }
                    else
                    {
                        log.Trace($"End {input.MethodBase.Name}({(end - start):mm\\:ss\\.fff}) result:{result ?? "void"}");
                    }
                }
            }

            private IMethodReturn NonLogExecution(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                return getNext()(input, getNext);
            }

            public int Order
            {
                get => 1;
                set { }
            }
        }
    }
}
