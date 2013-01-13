namespace Honeycomb
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;

    public class UniqueEvent
    {
        public UniqueEvent(Guid identity, Event @event, DateTimeOffset raisedTimestamp,
                           List<AggregateInfo> applyToAggregates)
        {
            Identity = identity;
            UntypedEvent = @event;
            RaisedTimestamp = raisedTimestamp;
            ApplyToAggregates = applyToAggregates;
            EventType = @event.GetType();
        }

        /// <summary>
        ///   The unique identity of the event
        /// </summary>
        public Guid Identity { get; private set; }

        /// <summary>
        ///   The event
        /// </summary>
        public Event UntypedEvent { get; private set; }

        /// <summary>
        ///   Type of the event
        /// </summary>
        public Type EventType { get; private set; }

        /// <summary>
        ///   The timestamp when the event was raised.
        /// </summary>
        public DateTimeOffset RaisedTimestamp { get; private set; }

        /// <summary>
        ///   Aggregates to apply the event to.
        /// </summary>
        public List<AggregateInfo> ApplyToAggregates { get; private set; }
    }
}