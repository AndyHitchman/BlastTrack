namespace Honeycomb.Azure.Bus.Infrastructure
{
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    ///   Events are mapped to registered consumers, which are then queued up by consumer group.
    /// </summary>
    /// <returns> </returns>
    public delegate QueueClient ConsumerQueueClient();
}