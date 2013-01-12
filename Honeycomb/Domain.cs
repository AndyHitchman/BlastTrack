using System;
using System.Linq;
using ReflectionMagic;

namespace Honeycomb
{
    using System.Collections.Generic;
    using Infrastructure;

    public class Domain
    {
        public static Domain Current;

        public Domain(EventStore eventStore)
        {
            Current = this;
            this.eventStore = eventStore;
        }

        private EventStore eventStore;

        public readonly AggregateTracker Tracked = new AggregateTracker();
        private readonly SelectorMap selectors = new SelectorMap();

        public void Apply(Command command)
        {
            foreach (var aggregateInfo in applyTo(command))
            {
                //If it's not tracked, then select it from the domain.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    selectAggregate(aggregateInfo);
                
                //If we haven't found it in the domain, then create it, otherwise apply the command.
                if(aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    AggregateFactory.Create(aggregateInfo, command);
                else
                    aggregateInfo.Instance.AsDynamic().Apply(command);
            }
        }

        public void Raise(Event @event)
        {
            foreach (var aggregateInfo in applyTo(@event))
            {
                //If it's not tracked, then select it from the domain.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    selectAggregate(aggregateInfo);

                //Record before consuming to ensure order is preserved.
                aggregateInfo.ResourceManager.RecordChange(@event);

                //If we haven't found it in the domain, then create it, otherwise consume the event.
                if(aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    AggregateFactory.Create(aggregateInfo, @event);
                else
                    aggregateInfo.Instance.AsDynamic().Receive(@event);


            }
        }

        private void selectAggregate(AggregateInfo aggregateInfo)
        {
            var replayEvents = eventStore.EventsForAggregate(aggregateInfo.Type, aggregateInfo.Key);
            if (replayEvents.Any()) 
                AggregateFactory.Buildup(aggregateInfo, replayEvents);
            
        }

        private IEnumerable<AggregateInfo> applyTo(Message message)
        {
            foreach (var selectorInfo in selectors[message])
            {
                var key = (object)selectorInfo.Selector.AsDynamic().SelectKey(message);
                var aggregateInfo = Tracked[selectorInfo.AggregateType, key];

                if(aggregateInfo.Lifestate == AggregateLifestate.Building) continue;

                yield return aggregateInfo;
            }
        }
    }
}