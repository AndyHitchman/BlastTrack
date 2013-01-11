namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using ReflectionMagic;

    public class AggregateFactory
    {
        public Aggregate Restore(Type aggregateType, object key, IEnumerable<UniqueEvent> replayEvents)
        {
            var events = replayEvents;
            
            var creationEvent = events.First();
            var changeEvents = events.Skip(1);

            var aggregate = blank(aggregateType);

            AggregateContext.RestoreInProgress(key, aggregate);
            
            construct(aggregateType, creationEvent, aggregate);
            replay(changeEvents, aggregate);
            
            AggregateContext.BuildComplete(aggregate);

            return aggregate;
        }

        private static Aggregate blank(Type aggregateType)
        {
            return (Aggregate) FormatterServices.GetUninitializedObject(aggregateType);
        }

        private static void construct(Type aggregateType, UniqueEvent creationEvent, Aggregate aggregate)
        {
            var creationConstructor = aggregateType.GetConstructor(new[] {creationEvent.EventType});
            creationConstructor.Invoke(aggregate, new object[] {creationEvent.UntypedEvent});
        }

        private static void replay(IEnumerable<UniqueEvent> changeEvents, Aggregate aggregate)
        {
            foreach (var uniqueEvent in changeEvents)
                aggregate.AsDynamic().Receive(uniqueEvent.UntypedEvent);
        }
    }
}