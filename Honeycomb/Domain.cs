namespace Honeycomb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Infrastructure;
    using Plumbing;
    using ReflectionMagic;

    public class Domain
    {
        public static Domain Current;

        protected readonly AggregateTracker AggregateTracker = new AggregateTracker();
        protected readonly EventEmitter Emitter;
        protected readonly EventCollector Collector;
        protected readonly EventStore Store;
        protected readonly SelectorMap Selectors = new SelectorMap();
        protected readonly TransactionTracker TransactionTracker;
        protected readonly AggregateFactory AggregateFactory = new AggregateFactory();

        protected Domain(EventEmitter emitter, EventCollector collector, EventStore store)
        {
            Current = this;
            Emitter = emitter;
            Collector = collector;
            Store = store;
            TransactionTracker = new TransactionTracker(emitter);
        }

        public virtual void Apply(Command command)
        {
            foreach (var aggregateInfo in applyTo(command))
            {
                //If it's not tracked, then select it from the domain.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    selectAggregate(aggregateInfo);

                try
                {
                    //If we haven't found it in the domain, then create it, otherwise apply the command.
                    if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                        AggregateFactory.Create(aggregateInfo, command);
                    else
                        aggregateInfo.Instance.AsDynamic().Apply(command);
                }
                catch (ApplicationException e)
                {
                    if (e.Source == "ReflectionMagic")
                        throw new MissingMethodException(
                            aggregateInfo.Type.FullName,
                            string.Format(
                                "{0}({1})",
                                aggregateInfo.Lifestate == AggregateLifestate.Untracked ? "_ctor" : "Accept",
                                command.GetType()));

                    throw;
                }
            }
        }

        public virtual void Consume(PendingEvent pendingEvent)
        {
            var applyToAggregates = applyTo(pendingEvent.Event).ToList();

            foreach (var aggregateInfo in applyToAggregates)
            {
                //If it's not tracked, then select it from the domain.
                if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                    selectAggregate(aggregateInfo);

                try
                {
                    try
                    {
                        //If we haven't found it in the domain, then create it, otherwise consume the event.
                        if (aggregateInfo.Lifestate == AggregateLifestate.Untracked)
                            AggregateFactory.Create(aggregateInfo, pendingEvent.Event);
                        else
                            aggregateInfo.Instance.AsDynamic().Receive(pendingEvent);
                    }
                    catch (ApplicationException e)
                    {
                        if (e.Source == "ReflectionMagic")
                            throw new MissingMethodException(
                                aggregateInfo.Type.FullName,
                                string.Format(
                                    "{0}({1})",
                                    aggregateInfo.Lifestate == AggregateLifestate.Untracked ? "Receive" : "Accept",
                                    pendingEvent.GetType()));

                        throw;
                    }
                }
                catch (Exception e)
                {
                    pendingEvent.RecordConsumptionFailure(aggregateInfo, e);
                }
                finally
                {
                    pendingEvent.RecordConsumptionComplete(aggregateInfo);
                }
            }
        }

        public virtual void Raise(Aggregate source, Event @event)
        {
            if (source != null && AggregateTracker[source].Lifestate == AggregateLifestate.Building) return;

            TransactionTracker[Transaction.Current].RecordEvent(@event);
        }

        private void selectAggregate(AggregateInfo aggregateInfo)
        {
            var replayEvents = Store.EventsForAggregate(aggregateInfo.Type, aggregateInfo.Key);
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