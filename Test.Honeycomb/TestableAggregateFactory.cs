namespace Test.Honeycomb
{
    using System.Collections.Generic;
    using global::Honeycomb;
    using global::Honeycomb.Infrastructure;

    public class TestableAggregateFactory : AggregateFactory
    {
        public new void Buildup(AggregateInfo aggregateInfo, ICollection<Event> replayEvents)
        {
            base.Buildup(aggregateInfo, replayEvents);
        }

        public new void Create(AggregateInfo aggregateInfo, Event @event)
        {
            base.Create(aggregateInfo, @event);
        }

        public new void Create(AggregateInfo aggregateInfo, Command command)
        {
            base.Create(aggregateInfo, command);
        }
    }
}