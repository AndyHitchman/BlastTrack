namespace Test.Honeycomb
{
    using global::Honeycomb;
    using global::Honeycomb.Infrastructure;

    public class TestableDomain : Domain
    {
        public TestableDomain(EventStore eventStore) : base(eventStore)
        {
        }

        public new AggregateTracker AggregateTracker
        {
            get { return base.AggregateTracker; }
        }

        public new EventStore EventStore
        {
            get { return base.EventStore; }
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