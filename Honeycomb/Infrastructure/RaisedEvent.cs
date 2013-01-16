namespace Honeycomb.Infrastructure
{
    using System;
    using System.Transactions;

    public class RaisedEvent
    {
        /// <summary>
        /// To support serialisation
        /// </summary>
        public RaisedEvent(Guid thumbprint, Event @event, DateTimeOffset raisedTimestamp, string transactionId)
        {
            Thumbprint = thumbprint;
            Event = @event;
            RaisedTimestamp = raisedTimestamp;
            TransactionId = transactionId;
        }

        public RaisedEvent(Event @event, DateTimeOffset raisedTimestamp)
            : this(
                Guid.NewGuid(),
                @event,
                raisedTimestamp,
                Transaction.Current == null ? null : Transaction.Current.TransactionInformation.LocalIdentifier)
        {
        }

        /// <summary>
        /// The unique identity of the event.
        /// </summary>
        public Guid Thumbprint { get; private set; }

        /// <summary>
        ///   The event
        /// </summary>
        public Event Event { get; private set; }

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