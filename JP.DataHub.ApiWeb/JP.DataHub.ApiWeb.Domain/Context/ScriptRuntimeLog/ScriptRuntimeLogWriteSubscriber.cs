using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog
{
    class ScriptRuntimeLogWriteSubscriber : IDomainEventSubscriber<ScriptRuntimeLogWriteEventData>
    {
        public ScriptRuntimeLogWriteSubscriber()
        {
            HandleEvent = data =>
            {
                Task.Run(() =>
                {
                    var fileRepository = UnityCore.Resolve<IScriptRuntimeLogFileRepository>();
                    fileRepository.AppendAsync(data.AppendFile);
                });
            };
        }

        public Action<ScriptRuntimeLogWriteEventData> HandleEvent { get; }

        public Type DomainEventType { get => typeof(ScriptRuntimeLogWriteEventData); }
    }
}
