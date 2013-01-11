namespace Honeycomb
{
    public interface SelectAggregate
    {
    }

    public interface SelectAggregate<TAggregate, TMessage> : SelectAggregate where TAggregate : Aggregate where TMessage : Message
    {
        TAggregate Select(TMessage message);
    }
}