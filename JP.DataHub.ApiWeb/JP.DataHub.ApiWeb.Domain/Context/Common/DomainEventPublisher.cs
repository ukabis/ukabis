using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    class DomainEventPublisher : IDomainEventPublisher
    {
        private static Dictionary<string, object> _subjects = new Dictionary<string, object>();

        private static List<IDisposable> _subscribers = new List<IDisposable>();

        private bool isPublish = false;


        public void Publish<T>(T domainEvent) where T : IDomainEventData
        {
            var subject = (Subject<T>)_subjects[domainEvent.GetType().FullName];
            subject.OnNext(domainEvent);

        }

        public void Reset()
        {
            _subscribers.ForEach(x => x.Dispose());
            _subjects = new Dictionary<string, object>();
        }

        public void Subscribe<T>(IDomainEventSubscriber<T> subscriber) where T : class, IDomainEventData
        {
            if (isPublish == false)
            {
                if (!_subjects.ContainsKey(subscriber.DomainEventType.FullName))
                {
                    _subjects.Add(subscriber.DomainEventType.FullName, new Subject<T>());
                }
                var subject = (Subject<T>)_subjects[subscriber.DomainEventType.FullName];
                _subscribers.Add(subject.Subscribe(subscriber.HandleEvent));
            }
        }
    }
}
