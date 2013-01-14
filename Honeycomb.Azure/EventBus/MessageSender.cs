namespace Honeycomb.Azure.EventBus
{
    /// <summary>
    /// Expose core message client functions to enable abstraction.
    /// </summary>
    public interface MessageSender
    {
        void Send(BrokeredMessage message);
    }
}