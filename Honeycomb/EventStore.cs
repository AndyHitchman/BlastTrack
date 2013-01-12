namespace Honeycomb
{
    using System;
    using System.Collections.Generic;

    public interface EventStore
    {
        IEnumerable<Event> EventsForAggregate(Type aggregateType, object key);
    }
}