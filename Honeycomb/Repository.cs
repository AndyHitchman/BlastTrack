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
            var aggregateType = typeof (TAggregate);
            var replayEvents = eventStore.RecordedEventsForAggregate(aggregateType, key);
            return (TAggregate)AggregateFactory.Restore(aggregateType, key, replayEvents);
        }
    }
}