namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using ReflectionMagic;

    public static class AggregateContext
    {
        private static readonly Dictionary<Tuple<Type,object>,Aggregate> trackedAggregateKeyMap;
        private static readonly Dictionary<Aggregate,AggregateLifestate> trackedAggregateLifestates;
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
            trackedAggregateLifestates[aggregate] = AggregateLifestate.Restoring;
        }

        public static void BuildComplete(Aggregate aggregate)
        {
            trackedAggregateLifestates[aggregate] = AggregateLifestate.Live;
            trackedAggregateResourceManager[aggregate] = new AggregateResourceManager(Transaction.Current);
        }

        public static bool IsLive(Aggregate aggregate)
        {
            //If we aren't tracking this, then we are creating, so declare live.
            if(!trackedAggregateLifestates.ContainsKey(aggregate)) BuildComplete(aggregate);

            return trackedAggregateLifestates[aggregate] == AggregateLifestate.Live;
        }

        public static void RecordChange(Aggregate aggregate, Event @event)
        {
            if (!IsLive(aggregate)) return;

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