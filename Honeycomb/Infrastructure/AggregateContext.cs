namespace Honeycomb.Infrastructure
{
    using System.Collections.Generic;
    using System.Transactions;

    public static class AggregateContext
    {
        private static readonly Dictionary<Aggregate,AggregateLifestate> trackedAggregateLifestates;
        private static readonly Dictionary<Aggregate, AggregateResourceManager> trackedAggregateResourceManager;

        static AggregateContext()
        {
            trackedAggregateLifestates = new Dictionary<Aggregate, AggregateLifestate>();
            trackedAggregateResourceManager = new Dictionary<Aggregate, AggregateResourceManager>();
        }

        public static void RestoreInProgress(Aggregate aggregate)
        {
            trackedAggregateLifestates[aggregate] = AggregateLifestate.Restoring;
        }

        public static void BuildComplete(Aggregate aggregate)
        {
            trackedAggregateLifestates[aggregate] = AggregateLifestate.Live;
            trackedAggregateResourceManager[aggregate] = new AggregateResourceManager(Transaction.Current);
        }

        public static bool IsLive(Aggregate aggregate)
        {
            AggregateLifestate lifestate;
            return trackedAggregateLifestates.TryGetValue(aggregate, out lifestate) && lifestate == AggregateLifestate.Live;
        }

        public static void RecordChange(Aggregate aggregate, Event @event)
        {
            if (!IsLive(aggregate)) return;
            trackedAggregateResourceManager[aggregate].RecordChange(@event);
        }
    }
}