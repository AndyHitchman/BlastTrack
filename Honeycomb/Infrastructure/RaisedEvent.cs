namespace Honeycomb.Infrastructure
{
    using System;
    using System.Transactions;

    public class RaisedEvent
    {
        /// <summary>
        /// To support serialisation
        /// </summary>
        protected RaisedEvent(Guid thumbprint, Event @event, Type eventType, DateTimeOffset raisedTimestamp, string transactionId)
        {
            Thumbprint = thumbprint;
            Event = @event;
            EventType = eventType;
            RaisedTimestamp = raisedTimestamp;
            TransactionId = transactionId;
        }

        public RaisedEvent(Event @event, DateTimeOffset raisedTimestamp)
        {
            Thumbprint = Guid.NewGuid();
            Event = @event;
            RaisedTimestamp = raisedTimestamp;
            EventType = @event.GetType();
            TransactionId = Transaction.Current == null ? null : Transaction.Current.TransactionInformation.LocalIdentifier;
        }

        /// <summary>
        /// The unique identity of the event.
        /// </summary>
        public Guid Thumbprint { get; set; }

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

        /// <summary>
        ///   The identify of the transaction that raised of the event
        /// </summary>
        public string TransactionId { get; private set; }
    }
}