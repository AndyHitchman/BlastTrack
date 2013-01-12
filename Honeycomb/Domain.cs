namespace Honeycomb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure;
    using ReflectionMagic;

    public class Domain
    {
        public static Domain Current;

        public readonly AggregateTracker Tracked = new AggregateTracker();
        private readonly EventStore eventStore;
        private readonly SelectorMap selectors = new SelectorMap();

        public Domain(EventStore eventStore)
        {
            Current = this;
            this.eventStore = eventStore;
        }

        public void Apply(Command command)
        {
            foreach (AggregateInfo aggregateInfo in applyTo(command))
            {
                //If it's not tracked, then select it from the domain.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    selectAggregate(aggregateInfo);

                //If we haven't found it in the domain, then create it, otherwise apply the command.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    AggregateFactory.Create(aggregateInfo, command);
                else
                    aggregateInfo.Instance.AsDynamic().Apply(command);
            }
        }

        public void Raise(Event @event)
        {
            foreach (AggregateInfo aggregateInfo in applyTo(@event))
            {
                //If it's not tracked, then select it from the domain.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    selectAggregate(aggregateInfo);

                //Record before consuming to ensure order is preserved.
                aggregateInfo.ResourceManager.RecordChange(@event);

                //If we haven't found it in the domain, then create it, otherwise consume the event.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    AggregateFactory.Create(aggregateInfo, @event);
                else
                    try
                    {
                        aggregateInfo.Instance.AsDynamic().Receive(@event);
                    }
                    catch (ApplicationException e)
                    {
                        if(e.Source == "ReflectionMagic")
                            throw new MissingMethodException(aggregateInfo.Type.FullName, "Receive(" + @event.GetType() + ")");

                        throw;
                    }
            }
        }

        private void selectAggregate(AggregateInfo aggregateInfo)
        {
            IEnumerable<Event> replayEvents = eventStore.EventsForAggregate(aggregateInfo.Type, aggregateInfo.Key);
            if (replayEvents.Any())
                AggregateFactory.Buildup(aggregateInfo, replayEvents);
        }

        private IEnumerable<AggregateInfo> applyTo(Message message)
        {
            foreach (SelectorMap.SelectorInfo selectorInfo in selectors[message])
            {
                var key = (object) selectorInfo.Selector.AsDynamic().SelectKey(message);
                AggregateInfo aggregateInfo = Tracked[selectorInfo.AggregateType, key];

                if (aggregateInfo.Lifestate == AggregateLifestate.Building) continue;

                yield return aggregateInfo;
            }
        }
    }
}