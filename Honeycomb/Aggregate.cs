namespace Honeycomb
{
    public interface Aggregate
    {
    }

    public static class AggregateExtensions
    {
        public static void Raise(this Aggregate aggregate, Event @event)
        {
            Domain.Current.Raise(@event);
        }
    }
}