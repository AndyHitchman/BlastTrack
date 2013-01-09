namespace Honeycomb
{
    using System;

    public class UniqueAggregate
    {
        public UniqueAggregate(Guid identity, Aggregate aggregate)
        {
            Identity = identity;
            UntypedAggregate = aggregate;
            AggregateType = @aggregate.GetType();
        }

        /// <summary>
        /// The unique identity of the aggregate
        /// </summary>
        public Guid Identity { get; private set; }

        /// <summary>
        /// The aggregate
        /// </summary>
        public Aggregate UntypedAggregate { get; set; }

        /// <summary>
        /// Type of the aggregate
        /// </summary>
        public Type AggregateType { get; set; }

    }
}