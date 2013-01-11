namespace Honeycomb.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;

    public class AggregateTracker
    {
        private readonly Dictionary<Aggregate, AggregateInfo> trackedAggregate = new Dictionary<Aggregate, AggregateInfo>();

        public AggregateInfo this[Type aggregateType, object key]
        {
            get
            {
                return trackedAggregate.Values.SingleOrDefault(_ => _.Type == aggregateType && _.Key == key);
            }
        }

        public AggregateInfo this[Aggregate aggregate]
        {
            get
            {
                if (!trackedAggregate.ContainsKey(aggregate))
                    trackedAggregate[aggregate] =
                        new AggregateInfo(aggregate.GetType(), new AggregateResourceManager(Transaction.Current));

                return trackedAggregate[aggregate];
            }
        }
    }
}