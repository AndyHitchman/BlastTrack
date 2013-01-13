namespace Honeycomb
{
    using System;
    using System.Collections.Generic;
    using Infrastructure;
    using System.Linq;

    public class UniqueEvent
    {
        public class ConsumptionRecord
        {
            public TimeSpan ExecutionTime { get; set; }
            public Exception ConsumptionException { get; set; }
        }

        public UniqueEvent(Event @event, DateTimeOffset raisedTimestamp, IEnumerable<AggregateInfo> affectedAggregates)
        {
            Receipt = Guid.NewGuid();
            UntypedEvent = @event;
            RaisedTimestamp = raisedTimestamp;
            AffectedAggregates = affectedAggregates.ToDictionary(info => info, info => new ConsumptionRecord());
            EventType = @event.GetType();
        }

        /// <summary>
        ///   The unique identity of the event
        /// </summary>
        public Guid Receipt { get; private set; }

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
        public Dictionary<AggregateInfo,ConsumptionRecord> AffectedAggregates { get; private set; }

        /// <summary>
        ///  Record an exception for an aggregate's consumption
        /// </summary>
        /// <param name="aggregateInfo"></param>
        /// <param name="exception"></param>
        public void RecordExceptionForConsumer(AggregateInfo aggregateInfo, Exception exception)
        {
            AffectedAggregates[aggregateInfo].ConsumptionException = exception;
        }

        /// <summary>
        ///  Record completion of an aggregate's consumption
        /// </summary>
        /// <param name="aggregateInfo"></param>
        public void RecordConsumptionComplete(AggregateInfo aggregateInfo)
        {
            AffectedAggregates[aggregateInfo].ExecutionTime = DateTimeOffset.UtcNow.Subtract(RaisedTimestamp);
        }
    }
}