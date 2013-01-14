namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;

    public class RaisedEvent
    {
        public class ConsumptionRecord
        {
            public TimeSpan ExecutionTime { get; set; }
            public Exception ConsumptionException { get; set; }
        }

        public RaisedEvent(Event @event, string transactionId, DateTimeOffset raisedTimestamp)
        {
            TransactionId = transactionId;
            UntypedEvent = @event;
            RaisedTimestamp = raisedTimestamp;
            EventType = @event.GetType();
        }

        /// <summary>
        ///   The unique identity of the event
        /// </summary>
        public string TransactionId { get; private set; }

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
    }
}