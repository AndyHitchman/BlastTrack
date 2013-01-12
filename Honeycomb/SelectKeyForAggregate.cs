namespace Honeycomb
{
    public interface SelectKeyForAggregate
    {
    }

    public interface SelectKeyForAggregate<TAggregate, TMessage> : SelectKeyForAggregate where TAggregate : Aggregate
                                                                                         where TMessage : Message
    {
        object SelectKey(TMessage message);
    }
}