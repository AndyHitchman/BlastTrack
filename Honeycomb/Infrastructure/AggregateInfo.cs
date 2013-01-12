namespace Honeycomb.Infrastructure
{
    using System;

    public class AggregateInfo
    {
        public Type Type { get; private set; }
        public object Key { get; private set; }
        public AggregateResourceManager ResourceManager { get; private set; }
        public AggregateLifestate Lifestate { get; set; }
        public Aggregate Instance { get; set; }

        public AggregateInfo(Type aggregateType, object key, AggregateResourceManager aggregateResourceManager)
        {
            Type = aggregateType;
            Key = key;
            ResourceManager = aggregateResourceManager;
            Lifestate = AggregateLifestate.Untracked;
        }
    }
}