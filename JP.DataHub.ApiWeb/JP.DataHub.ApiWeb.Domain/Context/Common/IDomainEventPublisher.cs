
namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal interface IDomainEventPublisher
    {
        void Publish<T>(T domainEvent) where T : IDomainEventData;

        void Reset();

        void Subscribe<T>(IDomainEventSubscriber<T> subscriber) where T : class, IDomainEventData;
    }
}
