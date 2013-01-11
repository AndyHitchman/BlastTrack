namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;

    public static class AggregateContext
    {
        private static readonly Dictionary<Tuple<Type, object>, Aggregate> trackedAggregateKeyMap;
        private static readonly Dictionary<Aggregate, AggregateLifestate> trackedAggregateLifestates;
        private static readonly Dictionary<Aggregate, AggregateResourceManager> trackedAggregateResourceManager;

        static AggregateContext()
        {
            trackedAggregateKeyMap = new Dictionary<Tuple<Type, object>, Aggregate>();
            trackedAggregateLifestates = new Dictionary<Aggregate, AggregateLifestate>();
            trackedAggregateResourceManager = new Dictionary<Aggregate, AggregateResourceManager>();
        }

        public static void RestoreRequired(Type aggregateType, object key)
        {
            trackedAggregateKeyMap[new Tuple<Type, object>(aggregateType, key)] = null;
        }

        public static void RestoreInProgress(object key, Aggregate aggregate)
        {
            trackedAggregateKeyMap[new Tuple<Type, object>(aggregate.GetType(), key)] = aggregate;
            trackedAggregateLifestates[aggregate] = AggregateLifestate.Replaying;
        }

        public static void BuildComplete(Aggregate aggregate)
        {
            trackedAggregateLifestates[aggregate] = AggregateLifestate.Live;
            trackedAggregateResourceManager[aggregate] = new AggregateResourceManager(Transaction.Current);
        }

        public static AggregateLifestate GetAggregateLifestate(Aggregate aggregate)
        {
            if(!trackedAggregateLifestates.ContainsKey(aggregate))
                trackedAggregateLifestates[aggregate] = AggregateLifestate.Untracked;

            return trackedAggregateLifestates[aggregate];
        }

        public static bool IsInstanceReplaying(Aggregate aggregate)
        {
            var lifestate = GetAggregateLifestate(aggregate);

            //If we aren't tracking this, then we are creating, so declare live.
            if (lifestate == AggregateLifestate.Untracked)
                BuildComplete(aggregate);

            return lifestate == AggregateLifestate.Replaying;
        }

        public static void RecordChange(Aggregate aggregate, Event @event)
        {
            if (IsInstanceReplaying(aggregate)) return;

//            var selectorTypes =
//                aggregate.GetType().FindInterfaces(
//                    (iface, criteria) => iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(SelectedWith<>),
//                    null);
//
//            aggregate.AsDynamic().Receive(@event);
            trackedAggregateResourceManager[aggregate].RecordChange(@event);
        }
    }
}