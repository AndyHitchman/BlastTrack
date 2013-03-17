namespace Honeycomb.Infrastructure
{
    using System;

    public class ConsumptionLog
    {
        public ConsumptionLog(RaisedEvent @event, DateTimeOffset collectedTimestamp, AggregateInfo affectedAggregate)
        {
            EventThumbprint = @event.Thumbprint;
            ConsumedTimestamp = collectedTimestamp;
            AffectedAggregate = affectedAggregate;
        }

        /// <summary>
        ///   The unique transaction that raised the event
        /// </summary>
        public Guid EventThumbprint { get; private set; }

        public DateTimeOffset ConsumedTimestamp { get; private set; }

        public AggregateInfo AffectedAggregate { get; private set; }

        public TimeSpan ExecutionTime { get; private set; }

        public Exception ConsumptionException { get; private set; }

        /// <summary>
        ///   Record an exception for an aggregate's consumption
        /// </summary>
        /// <param name="aggregateInfo"> </param>
        /// <param name="exception"> </param>
        public void RecordExceptionForConsumer(Exception exception)
        {
            ConsumptionException = exception;
        }

        /// <summary>
        ///   Record completion of an aggregate's consumption
        /// </summary>
        /// <param name="aggregateInfo"> </param>
        public void RecordConsumptionComplete()
        {
            ExecutionTime = DateTimeOffset.UtcNow.Subtract(ConsumedTimestamp);
        }
    }
}