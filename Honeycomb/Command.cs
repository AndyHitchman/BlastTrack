namespace Honeycomb
{
    public interface Command : Message
    {
    }

    public static class CommandExtensions
    {
        public static void Apply<TAggregate>(this Command cmd) where TAggregate : Aggregate
        {
            Domain.Current.Apply(cmd);
        }
    }
}