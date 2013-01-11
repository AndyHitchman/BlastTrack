namespace Honeycomb
{
    using System;
    using System.Collections.Generic;

    public interface EventStore
    {
        IEnumerable<UniqueEvent> RecordedEventsForAggregate(Type aggregateIdentity, object key);
    }
}