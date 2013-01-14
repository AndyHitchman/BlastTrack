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

        public void Raise(Event @event)
        {
            Raise(null, @event);
        }

        public void Raise(Aggregate source, Event @event)
        {
            if (source != null && AggregateTracker[source].Lifestate == AggregateLifestate.Building) return;

            var applyToAggregates = applyTo(@event).ToList();

            //Record before consuming to ensure order is preserved.
            var receipt = TransactionTracker[Transaction.Current].RecordEvent(@event, applyToAggregates);

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
                            AggregateFactory.Create(aggregateInfo, @event);
                        else
                            aggregateInfo.Instance.AsDynamic().Receive(@event);
                    }
                    catch (ApplicationException e)
                    {
                        if (e.Source == "ReflectionMagic")
                            throw new MissingMethodException(
                                aggregateInfo.Type.FullName,
                                string.Format(
                                    "{0}({1})",
                                    aggregateInfo.Lifestate == AggregateLifestate.Untracked ? "Receive" : "Accept",
                                    @event.GetType()));

                        throw;
                    }
                }
                catch(Exception e)
                {
                    TransactionTracker[Transaction.Current].RecordConsumptionFailure(receipt, aggregateInfo, e);
                }
                finally
                {
                    TransactionTracker[Transaction.Current].RecordConsumptionComplete(receipt, aggregateInfo);
                }
            }
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