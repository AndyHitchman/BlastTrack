namespace Honeycomb.Plumbing
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;

    public interface EventStore
    {
        Event[] EventsForAggregate(Type aggregateType, object key);
        void RecordEvent(Type eventType, Event untypedEvent, Dictionary<AggregateInfo, RaisedEvent.ConsumptionRecord> affectedAggregates);
    }
}