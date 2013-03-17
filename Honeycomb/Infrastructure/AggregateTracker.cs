namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AggregateTracker
    {
        private readonly Dictionary<Tuple<Type, object>, AggregateInfo> trackedAggregate =
            new Dictionary<Tuple<Type, object>, AggregateInfo>();

        public AggregateInfo this[Type aggregateType, object key]
        {
            get
            {
                var id = new Tuple<Type, object>(aggregateType, key);

                if (!trackedAggregate.ContainsKey(id))
                    trackedAggregate[id] =
                        new AggregateInfo(aggregateType, key);

                return trackedAggregate[id];
            }
        }

        public AggregateInfo this[Aggregate aggregate]
        {
            get { return trackedAggregate.Values.SingleOrDefault(_ => _.Instance == aggregate); }
        }
    }
}