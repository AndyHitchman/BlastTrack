namespace Honeycomb.Azure.Store
{
    using System;
    using Infrastructure;
    using Microsoft.WindowsAzure.Storage.Table;

    public class EventEntity : TableEntity
    {
        /// <summary>
        ///   For Storage API.
        /// </summary>
        public EventEntity()
        {
        }

        public EventEntity(RaisedEvent raisedEvent)
        {
            PartitionKey = raisedEvent.Thumbprint.ToString();
            RowKey = raisedEvent.Event.GetType().FullName;
            TransactionId = raisedEvent.TransactionId;
            RaisedTimestamp = raisedEvent.RaisedTimestamp;
            Event = JsonEvent.ConvertToJson(raisedEvent);
        }


        // ReSharper disable MemberCanBePrivate.Global

        public string TransactionId { get; set; }

        public DateTimeOffset RaisedTimestamp { get; set; }

        public string Event { get; set; }

        // ReSharper restore MemberCanBePrivate.Global
    }
}