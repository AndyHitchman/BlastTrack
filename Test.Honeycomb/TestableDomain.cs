namespace Test.Honeycomb
{
    using global::Honeycomb;
    using global::Honeycomb.Infrastructure;
    using global::Honeycomb.Plumbing;

    public class TestableDomain : Domain
    {
        public TestableDomain(EventEmitter emitter, EventCollector collector, EventStore store) : base(emitter, collector, store)
        {
        }

        public new AggregateTracker AggregateTracker
        {
            get { return base.AggregateTracker; }
        }

        public new EventEmitter Emitter
        {
            get { return base.Emitter; }
        }

        public new EventCollector Collector
        {
            get { return base.Collector; }
        }

        public new EventStore Store
        {
            get { return base.Store; }
        }

        public new SelectorMap Selectors
        {
            get { return base.Selectors; }
        }

        public new TransactionTracker TransactionTracker
        {
            get { return base.TransactionTracker; }
        }
    }
}