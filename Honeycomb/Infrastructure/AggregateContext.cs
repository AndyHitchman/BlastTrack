namespace Honeycomb.Infrastructure
{
    using System;
    using ReflectionMagic;

    public static class AggregateContext
    {
        public static readonly AggregateTracker AggregateTracker = new AggregateTracker();
        public static readonly SelectorMap Selectors = new SelectorMap();

        public static void AggregateIsReplaying(Aggregate aggregate, object key)
        {
            var trackedAggregate = AggregateTracker[aggregate];
            trackedAggregate.Instance = aggregate;
            trackedAggregate.Key = key;
            trackedAggregate.Lifestate = AggregateLifestate.Replaying;
        }

        public static void AggregateIsLive(Aggregate aggregate, object key)
        {
            var trackedAggregate = AggregateTracker[aggregate];
            trackedAggregate.Lifestate = AggregateLifestate.Live;
        }

        public static AggregateLifestate GetAggregateLifestate(Aggregate aggregate)
        {
            return AggregateTracker[aggregate].Lifestate;
        }

//        private static bool isInstanceReplaying(Aggregate aggregate)
//        {
//            var lifestate = GetAggregateLifestate(aggregate);
//
//            //If we aren't tracking this, then we are creating, so declare live.
//            if (lifestate == AggregateLifestate.Untracked)
//                AggregateIsLive(aggregate);
//
//            return lifestate == AggregateLifestate.Replaying;
//        }

        public static void Raise(Event @event)
        {
            foreach (var selector in Selectors[@event])
            {
                var consumer = selector.AsDynamic().Select(@event);
                var aggregateInfo = AggregateTracker[(Aggregate)consumer];

                if (aggregateInfo.Lifestate != AggregateLifestate.Live) continue;

                aggregateInfo.ResourceManager.RecordChange(@event);
                consumer.Receive(@event);
            }
        }
    }
}