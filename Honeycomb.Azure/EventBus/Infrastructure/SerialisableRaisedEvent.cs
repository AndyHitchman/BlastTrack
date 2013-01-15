namespace Honeycomb.Azure.EventBus.Infrastructure
{
    using System;
    using Honeycomb.Infrastructure;

    public class SerialisableRaisedEvent : RaisedEvent
    {
        public SerialisableRaisedEvent(Guid thumbprint, Event @event, Type eventType, DateTimeOffset raisedTimestamp, string transactionId)
            : base(thumbprint, @event, eventType, raisedTimestamp, transactionId)
        {
        }
    }
}