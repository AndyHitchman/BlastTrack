namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public class PendingEvent
    {
        private readonly RaisedEvent raisedEvent;
        private List<ConsumptionLog> consumptionLogs;

        public PendingEvent(RaisedEvent raisedEvent)
        {
            this.raisedEvent = raisedEvent;
            consumptionLogs = new List<ConsumptionLog>();
        }

        public void RecordConsumptionFailure(AggregateInfo aggregateInfo, Exception exception)
        {
        }

        public void RecordConsumptionComplete(AggregateInfo aggregateInfo)
        {
        }

        public Event Event { get { return raisedEvent.UntypedEvent; } }
    }
}