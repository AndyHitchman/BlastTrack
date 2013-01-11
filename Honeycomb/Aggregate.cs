namespace Honeycomb
{
    using Infrastructure;

    public interface Aggregate
    {
    }

    public static class AggregateExtensions
    {
        public static void Raise(this Aggregate aggregate, Event @event)
        {
            AggregateContext.RecordChange(aggregate, @event);
        }
    }
}