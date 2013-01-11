namespace Honeycomb
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateSelectorAttribute : Attribute
    {
        public SelectAggregate AggregrateSelector { get; private set; }

        public AggregateSelectorAttribute(Type aggregrateSelector)
        {
            AggregrateSelector = Activator.CreateInstance(aggregrateSelector) as SelectAggregate;
        }
    }
}