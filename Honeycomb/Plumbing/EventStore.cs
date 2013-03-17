namespace Honeycomb.Plumbing
{
    using System;
    using Infrastructure;

    public interface EventStore
    {
        Event[] EventsForAggregate(Type aggregateType, object key);
        void RecordEvent(RaisedEvent raisedEvent);
        void LogConsumption(RaisedEvent raisedEvent, ConsumptionLog consumptionLog);
    }
}