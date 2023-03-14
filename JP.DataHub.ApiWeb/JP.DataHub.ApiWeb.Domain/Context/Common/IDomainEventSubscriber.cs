
namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal interface IDomainEventSubscriber<T> where T : IDomainEventData
    {
        Action<T> HandleEvent { get; }

        Type DomainEventType { get; }
    }
}
