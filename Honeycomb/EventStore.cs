namespace Honeycomb
{
    using System;
    using System.Collections.Generic;

    public interface EventStore
    {
        IEnumerable<UniqueEvent> GetEventsForAggregate(Type aggregateIdentity, object key);
    }
}