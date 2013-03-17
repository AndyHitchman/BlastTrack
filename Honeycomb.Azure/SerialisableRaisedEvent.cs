namespace Honeycomb.Azure
{
    using System;
    using Infrastructure;

    /// <summary>
    ///   This exists to encourage Json.NET to use the correct constructor without having to annotate <see cref="RaisedEvent" /> with
    ///   provider specific behaviour.
    /// </summary>
    public class SerialisableRaisedEvent : RaisedEvent
    {
        public SerialisableRaisedEvent(Guid thumbprint, Event @event, DateTimeOffset raisedTimestamp, string transactionId)
            : base(thumbprint, @event, raisedTimestamp, transactionId)
        {
        }
    }
}