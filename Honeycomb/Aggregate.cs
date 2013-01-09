namespace Honeycomb
{
    public interface Aggregate
    {
    }

    public static class AggregateExtensions
    {
        public static void Raise<TAggregate, TEvent>(this TAggregate aggregate, TEvent @event) where TAggregate : Aggregate
        {
            
        }
    }
}