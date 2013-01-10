namespace Honeycomb.Infrastructure
{
    using System;
    using System.Linq;
    using ReflectionMagic;

    public class AggregateFactory<TAggregate,TKey> where TAggregate : Aggregate
    {
        private readonly EventStore eventStore;

        public AggregateFactory(EventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        public TAggregate Restore(TKey key)
        {
            var aggregateType = typeof (TAggregate);
            var events = eventStore.GetEventsForAggregate(aggregateType, key);
            
            var creationEvent = events.First();
            var changeEvents = events.Skip(1);

            var aggregate = construct(aggregateType, creationEvent);
            AggregateContext.RestoreInProgress(aggregate);

            foreach (var uniqueEvent in changeEvents)
                aggregate.AsDynamic().Receive(uniqueEvent.UntypedEvent);
            
            AggregateContext.BuildComplete(aggregate);

            return aggregate;
        }

        private static TAggregate construct(Type aggregateType, UniqueEvent creationEvent)
        {
            var creationConstructor = aggregateType.GetConstructor(new[] {creationEvent.EventType});
            var aggregate = (TAggregate) creationConstructor.Invoke(new[] {creationEvent.UntypedEvent});
            return aggregate;
        }
    }
}