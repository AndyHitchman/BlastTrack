namespace Honeycomb
{
    using System;

    public interface EventStore
    {
        Event[] EventsForAggregate(Type aggregateType, object key);
    }
}