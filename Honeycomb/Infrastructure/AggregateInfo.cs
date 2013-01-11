namespace Honeycomb.Infrastructure
{
    using System;

    public class AggregateInfo
    {
        public Type Type { get; private set; }
        public object Key { get; set; }
        public AggregateLifestate Lifestate { get; set; }
        public AggregateResourceManager ResourceManager { get; set; }
        public Aggregate Instance { get; set; }

        public AggregateInfo(Type aggregateType, AggregateResourceManager aggregateResourceManager)
        {
            Type = aggregateType;
            ResourceManager = aggregateResourceManager;
            Lifestate = AggregateLifestate.Untracked;
        }
    }
}