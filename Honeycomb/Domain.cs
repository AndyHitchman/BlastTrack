namespace Honeycomb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Infrastructure;
    using ReflectionMagic;

    public class Domain
    {
        public static Domain Current;

        protected readonly AggregateTracker AggregateTracker = new AggregateTracker();
        protected readonly EventStore EventStore;
        protected readonly SelectorMap Selectors = new SelectorMap();
        protected readonly TransactionTracker TransactionTracker = new TransactionTracker();

        public Domain(EventStore eventStore)
        {
            Current = this;
            this.EventStore = eventStore;
        }

        public TransactionScope StartTransaction()
        {
            return new TransactionScope();
        }

        public void Apply(Command command)
        {
            foreach (var aggregateInfo in applyTo(command))
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

        public void Raise(Aggregate source, Event @event)
        {
            if (AggregateTracker[source].Lifestate == AggregateLifestate.Building) return;

            var applyToAggregates = applyTo(@event).ToList();

            //Record before consuming to ensure order is preserved.
            TransactionTracker[Transaction.Current].RecordEvent(@event, applyToAggregates);

            foreach (var aggregateInfo in applyToAggregates)
            {
                //If it's not tracked, then select it from the domain.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    selectAggregate(aggregateInfo);

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
                        if (e.Source == "ReflectionMagic")
                            throw new MissingMethodException(aggregateInfo.Type.FullName,
                                                             "Receive(" + @event.GetType() + ")");

                        throw;
                    }
            }
        }

        private void selectAggregate(AggregateInfo aggregateInfo)
        {
            var replayEvents = EventStore.EventsForAggregate(aggregateInfo.Type, aggregateInfo.Key);
            if (replayEvents.Any())
                AggregateFactory.Buildup(aggregateInfo, replayEvents);
        }

        private IEnumerable<AggregateInfo> applyTo(Message message)
        {
            foreach (var selectorInfo in Selectors[message])
            {
                var key = (object) selectorInfo.Selector.AsDynamic().SelectKey(message);
                var aggregateInfo = AggregateTracker[selectorInfo.AggregateType, key];

                if (aggregateInfo.Lifestate == AggregateLifestate.Building) continue;

                yield return aggregateInfo;
            }
        }
    }
}