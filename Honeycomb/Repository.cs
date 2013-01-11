namespace Honeycomb
{
    using System.Collections.Generic;
    using Infrastructure;

    public static class Repository
    {
        private static EventStore eventStore;

        public static void SetEventStore(EventStore eventStore)
        {
            Repository.eventStore = eventStore;
        }

        public static TAggregate Select<TAggregate>(object key) where TAggregate : Aggregate
        {
            var aggregateFactory = new AggregateFactory();
            var aggregateType = typeof (TAggregate);

            AggregateContext.RestoreRequired(aggregateType, key);

            var replayEvents = eventStore.RecordedEventsForAggregate(aggregateType, key);
            return (TAggregate)aggregateFactory.Restore(aggregateType, key, replayEvents);
        }
    }
}