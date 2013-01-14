namespace Honeycomb.Infrastructure
{
    using System;

    public class RaisedEvent
    {
        public RaisedEvent(Event @event, string transactionId, DateTimeOffset raisedTimestamp)
        {
            TransactionId = transactionId;
            Event = @event;
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
        public Event Event { get; private set; }

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