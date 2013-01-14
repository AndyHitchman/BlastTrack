namespace Honeycomb.Infrastructure
{
    using System;

    public class ConsumptionLog
    {
        public ConsumptionLog(Guid eventReceipt, DateTimeOffset collectedTimestamp, AggregateInfo affectedAggregate)
        {
            EventReceipt = eventReceipt;
            CollectedTimestamp = collectedTimestamp;
            AffectedAggregate = affectedAggregate;
        }

        /// <summary>
        ///   The unique identity of the event
        /// </summary>
        public Guid EventReceipt { get; private set; }

        public DateTimeOffset CollectedTimestamp { get; private set; }

        public AggregateInfo AffectedAggregate { get; private set; }

        public TimeSpan ExecutionTime { get; private set; }

        public Exception ConsumptionException { get; private set; }

        /// <summary>
        ///  Record an exception for an aggregate's consumption
        /// </summary>
        /// <param name="aggregateInfo"></param>
        /// <param name="exception"></param>
        public void RecordExceptionForConsumer(Exception exception)
        {
            ConsumptionException = exception;
        }

        /// <summary>
        ///  Record completion of an aggregate's consumption
        /// </summary>
        /// <param name="aggregateInfo"></param>
        public void RecordConsumptionComplete()
        {
            ExecutionTime = DateTimeOffset.UtcNow.Subtract(CollectedTimestamp);
        }
    }
}