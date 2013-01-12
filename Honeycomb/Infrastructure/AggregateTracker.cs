namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;

    public class AggregateTracker
    {
        private readonly Dictionary<Tuple<Type, object>, AggregateInfo> trackedAggregate = new Dictionary<Tuple<Type, object>, AggregateInfo>();

        public AggregateInfo this[Type aggregateType, object key]
        {
            get
            {
                var id = new Tuple<Type, object>(aggregateType, key);

                if (!trackedAggregate.ContainsKey(id))
                    trackedAggregate[id] =
                        new AggregateInfo(aggregateType, key, new AggregateResourceManager(Transaction.Current));

                return trackedAggregate[id];
            }
        }

        public AggregateInfo this[Aggregate aggregate]
        {
            get
            {
                return trackedAggregate.Values.SingleOrDefault(_ => _.Instance == aggregate);
            }
        }
    }
}