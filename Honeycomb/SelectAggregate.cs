namespace Honeycomb
{
    public interface SelectAggregate<TAggregate, TMessage> where TAggregate : Aggregate where TMessage : Message
    {
        TAggregate Select(TMessage message);
    }
}