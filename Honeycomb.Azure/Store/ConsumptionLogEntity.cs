namespace Honeycomb.Azure.Store
{
    using System;
    using Infrastructure;
    using Microsoft.WindowsAzure.Storage.Table;

    public class ConsumptionLogEntity : TableEntity
    {
        private const string longPad = "0000000000000000000";

        /// <summary>
        ///   For Storage API.
        /// </summary>
        public ConsumptionLogEntity()
        {
        }

        public ConsumptionLogEntity(ConsumptionLog consumptionLog, RaisedEvent raisedEvent)
        {
            PartitionKey = consumptionLog.AffectedAggregate.Type.FullName + "@" + consumptionLog.AffectedAggregate.Key;
            RowKey = consumptionLog.ConsumedTimestamp.Ticks.ToString(longPad);
            EventThumbprint = raisedEvent.Thumbprint;
            ConsumedTimestamp = consumptionLog.ConsumedTimestamp;
            ExecutionTime = consumptionLog.ExecutionTime;
            ConsumptionException = consumptionLog.ConsumptionException;
            Event = JsonEvent.ConvertToJson(raisedEvent);
        }

        // ReSharper disable MemberCanBePrivate.Global

        public Guid EventThumbprint { get; set; }

        public DateTimeOffset ConsumedTimestamp { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public Exception ConsumptionException { get; set; }

        public string Event { get; set; }

        // ReSharper restore MemberCanBePrivate.Global
    }
}